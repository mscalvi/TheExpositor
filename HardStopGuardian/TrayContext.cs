using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace HardStopGuardian
{
    // App SEM janela-base (sem Alt-Tab): tudo gerido por ApplicationContext
    public sealed class TrayContext : ApplicationContext
    {
        // ---- Infra UI thread ----
        private readonly SynchronizationContext _ui = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
        private Icon _iconOn, _iconOff; // ícones

        // ---- Bandeja e menu ----
        private NotifyIcon _tray;
        private ContextMenuStrip _menu;

        // ---- Timers do fluxo principal ----
        private WinFormsTimer _startupTimer;   // pop-up inicial
        private WinFormsTimer _tMinus10Timer;  // aviso T-10
        private WinFormsTimer _t0Timer;        // T0 (bloqueio)

        // ---- Estado do hard-stop ----
        private DateTime _nextHardStop;
        private bool _armed;
        private bool _pendingOverlay;
        private int _blockMinutes = 5;         // 5→30 (+5 a cada “Manter”)

        // ---- Checkups diários ----
        private List<CheckupItem> _checkups = new();
        private WinFormsTimer _checkTimer;
        private bool _checkDialogOpen;

        // ---- Auto-start (HKCU\Run) ----
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "HardStopGuardian";

        // ---- WinAPI ----
        [DllImport("user32.dll")] private static extern bool LockWorkStation();

        public TrayContext()
        {
            SetupTray();                 // ícone + menu
            SetupStartupPopup();         // pop-up inicial (StartupConfigForm)

            // Retomar estado salvo (confirma antes)
            var resumed = TryResumeFromState();
            if (resumed) _startupTimer?.Stop();

            // Checkups
            _checkups = CheckupStore.Load();
            StartCheckScheduler();

            // Ouvir desbloqueio de sessão (para abrir overlay após LockWorkStation)
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        // =========================
        // BANDEJA / MENU
        // =========================
        private void SetupTray()
        {
            _menu = new ContextMenuStrip();

            var auto = new ToolStripMenuItem("Iniciar com Windows")
            {
                Checked = IsAutoStartEnabled(),
                CheckOnClick = true
            };
            auto.CheckedChanged += (s, e) => SetAutoStart(auto.Checked);

            _menu.Items.Add(auto);
            _menu.Items.Add(new ToolStripMenuItem("Editar checkups…", null, (s, e) => OpenCheckupsDialog()));
            _menu.Items.Add(new ToolStripMenuItem("Ajuda rápida", null, (s, e) => ShowQuickHelp()));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("Ligar/Editar", null, (s, e) => PromptAndArm()));
            _menu.Items.Add(new ToolStripMenuItem("Standby", null, (s, e) => Standby()));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("Sair", null, (s, e) => ExitThread()));

            _tray = new NotifyIcon
            {
                Icon = LoadIcon("Assets\\icon.ico"),
                Visible = true,
                Text = "Hard-Stop Guardian",
                ContextMenuStrip = _menu
            };

            _iconOn = LoadIcon("Assets\\iconon.ico");
            _iconOff = LoadIcon("Assets\\iconoff.ico");
            _tray.Icon = _iconOff; // inicial
        }

        private static bool IsAutoStartEnabled()
        {
            using var rk = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            var val = rk?.GetValue(RunValueName) as string;
            return !string.IsNullOrEmpty(val);
        }

        private static void SetAutoStart(bool enable)
        {
            var exe = Application.ExecutablePath;
            using var rk = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                         ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (enable) rk.SetValue(RunValueName, $"\"{exe}\"");
            else rk.DeleteValue(RunValueName, throwOnMissingValue: false);
        }

        private static void ShowQuickHelp()
        {
            const string msg =
@"HardStop Guardian — Ajuda rápida

• Standby: ícone da bandeja → Standby.
• Sair: ícone da bandeja → Sair.
• Emergência: Ctrl+Shift+Esc → finalize HardStopGuardian.exe.

• Auto-start: ícone da bandeja → Iniciar com Windows.
• Retomar após reinício: o app pergunta “Retomar?”.

• Arquivos:
  %APPDATA%\HardStopGuardian\state.json     (estado)
  %APPDATA%\HardStopGuardian\checkups.json  (checkups)";
            MessageBox.Show(msg, "Ajuda rápida", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Helper para Ícone
        private static Icon LoadIcon(string relativePath)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, relativePath);
                if (File.Exists(path)) return new Icon(path);

                MessageBox.Show("ICO não encontrado:\n" + path, "HardStop Guardian",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return SystemIcons.Application;
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        // =========================
        // POP-UP INICIAL (CONFIG)
        // =========================
        private void SetupStartupPopup()
        {
            _startupTimer = new WinFormsTimer { Interval = 20_000 };
            _startupTimer.Tick += (s, e) =>
            {
                _startupTimer.Stop();
                ShowStartupPopup();
            };
            _startupTimer.Start();
        }

        private void ShowStartupPopup()
        {
            using var dlg = new StartupConfigForm(); // Hard-stop + lista dinâmica de checkups
            var r = dlg.ShowDialog();
            if (r != DialogResult.OK) return;

            Arm(dlg.SelectedHardStop);
            if (dlg.SelectedCheckups.Count > 0)
                ApplyCheckupsFromStartup(dlg.SelectedCheckups);
        }

        private void ApplyCheckupsFromStartup(List<(string label, TimeSpan time)> items)
        {
            foreach (var (label, time) in items)
            {
                var i = _checkups.FindIndex(x => string.Equals(x.Label, label, StringComparison.OrdinalIgnoreCase));
                if (i >= 0)
                {
                    _checkups[i].Time = time;
                    _checkups[i].Enabled = true;
                }
                else
                {
                    _checkups.Add(new CheckupItem { Label = label, Time = time, Enabled = true });
                }
            }
            CheckupStore.Save(_checkups);
            ReloadCheckups();
        }

        // =========================
        // ARMAR / EDITAR HARD-STOP
        // =========================
        private void PromptAndArm()
        {
            using (var confirm = new CountdownPromptForm(
                "Hard-Stop Guardian",
                _armed ? $"Reagendar de {_nextHardStop:HH:mm} para…" : "Definir um hard-stop para hoje:",
                "Continuar", "Cancelar",
                3, insistent: true))
            {
                if (confirm.ShowDialog() != DialogResult.OK) return;
            }

            using var dlg = new TimePromptForm { StartPosition = FormStartPosition.CenterScreen };
            if (_armed) dlg.SetInitialTime(_nextHardStop);
            if (dlg.ShowDialog() == DialogResult.OK) Arm(dlg.SelectedTime);
        }

        private void Arm(DateTime chosenTime)
        {
            _nextHardStop = NormalizeToNextOccurrence(chosenTime);

            _tray.Icon = _iconOn;

            _tray.BalloonTipTitle = "Hard-Stop Configurado";
            _tray.BalloonTipText = $"T0: {_nextHardStop:HH:mm}  •  Aviso: {(_nextHardStop - TimeSpan.FromMinutes(10)):HH:mm}";
            _tray.ShowBalloonTip(4000);

            ScheduleTimers();
            _armed = true;
            SaveState();
        }

        private static DateTime NormalizeToNextOccurrence(DateTime t)
        {
            var now = DateTime.Now;
            var todayAt = new DateTime(now.Year, now.Month, now.Day, t.Hour, t.Minute, 0);
            return (todayAt <= now) ? todayAt.AddDays(1) : todayAt;
        }

        // =========================
        // AGENDAMENTO (T-10 / T0)
        // =========================
        private void ScheduleTimers()
        {
            var now = DateTime.Now;
            var tMinus10 = _nextHardStop - TimeSpan.FromMinutes(10);

            int ms10 = (int)Math.Max((tMinus10 - now).TotalMilliseconds, 1000);
            int ms0 = (int)Math.Max((_nextHardStop - now).TotalMilliseconds, 1000);

            _tMinus10Timer?.Stop();
            _t0Timer?.Stop();

            _tMinus10Timer = new WinFormsTimer { Interval = ms10 };
            _t0Timer = new WinFormsTimer { Interval = ms0 };

            _tMinus10Timer.Tick += (s, e) => { _tMinus10Timer.Stop(); ShowTMinus10Toast(); };
            _t0Timer.Tick += (s, e) => { _t0Timer.Stop(); ExecuteHardStop(); };

            _tMinus10Timer.Start();
            _t0Timer.Start();
        }

        private void ShowTMinus10Toast()
        {
            _tray.BalloonTipTitle = "Faltam 10min";
            _tray.BalloonTipText = "Salve seu progresso.";
            _tray.ShowBalloonTip(5000);
        }

        private void ExecuteHardStop()
        {
            _pendingOverlay = true;
            LockWorkStation();
        }

        // =========================
        // OVERLAY / PÓS-BLOQUEIO
        // =========================
        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock && _pendingOverlay)
            {
                _pendingOverlay = false;
                _ui.Post(_ => ShowOverlay(), null);
            }
        }

        private void ShowOverlay()
        {
            var overlay = new OverlayForm(_blockMinutes);
            overlay.FormClosed += (s, e) => AfterBlock();
            overlay.Show();
        }

        private void AfterBlock()
        {
            using var dlg = new CountdownPromptForm(
                "Hard-Stop Guardian",
                "Manter ativado?",
                "Sim", "Não",
                10, insistent: true);

            var r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                _blockMinutes = Math.Min(_blockMinutes + 5, 30);
                _nextHardStop = DateTime.Now.AddMinutes(5);
                _armed = true;
                ScheduleTimers();
                SaveState();
            }
            else
            {
                _tMinus10Timer?.Stop();
                _tray.Icon = _iconOff;
                _t0Timer?.Stop();
                _armed = false;
                SaveState();
            }
        }

        // =========================
        // PERSISTÊNCIA
        // =========================
        private void SaveState()
        {
            Persistence.Save(new StateModel
            {
                Armed = _armed,
                NextHardStop = _nextHardStop,
                BlockMinutes = _blockMinutes
            });
        }

        private bool TryResumeFromState()
        {
            var st = Persistence.Load();
            if (st == null) return false;

            _blockMinutes = Math.Clamp(st.BlockMinutes, 5, 30);

            if (st.Armed)
            {
                var t0 = NormalizeToNextOccurrence(st.NextHardStop);
                var ans = MessageBox.Show(
                    $"Retomar Hard-Stop para hoje às {t0:HH:mm}?",
                    "Retomar?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (ans == DialogResult.Yes)
                {
                    _armed = true;
                    _nextHardStop = t0;
                    ScheduleTimers();

                    _tray.BalloonTipTitle = "Hard-Stop retomado";
                    _tray.BalloonTipText = $"T0: {_nextHardStop:HH:mm}";
                    _tray.ShowBalloonTip(3000);
                    return true;
                }
                else
                {
                    _armed = false;
                    SaveState();
                }
            }
            return false;
        }

        private void Standby()
        {
            _tMinus10Timer?.Stop();
            _tray.Icon = _iconOff;
            _t0Timer?.Stop();
            _armed = false;
            SaveState();

            _tray.BalloonTipTitle = "Standby";
            _tray.BalloonTipText = "Hard-Stop Desativado.";
            _tray.ShowBalloonTip(3000);
        }

        // =========================
        // CHECKUPS
        // =========================
        private void StartCheckScheduler()
        {
            _checkTimer = new WinFormsTimer { Interval = 15_000 };
            _checkTimer.Tick += (s, e) => PollCheckups();
            _checkTimer.Start();
        }

        private void PollCheckups()
        {
            if (_checkDialogOpen) return;
            if (!_armed) return; // ← bloqueia checkups em Standby/HS desligado

            var now = DateTime.Now;
            foreach (var it in _checkups)
            {
                if (!it.Enabled) continue;

                var due = new DateTime(now.Year, now.Month, now.Day, it.Time.Hours, it.Time.Minutes, 0);
                if (it.LastDoneDate.HasValue && it.LastDoneDate.Value.Date == now.Date) continue;

                if (now >= due && (now - due).TotalSeconds <= 60)
                {
                    _checkDialogOpen = true;
                    _ui.Post(_ => FireCheckup(it), null);
                    return;
                }
            }
        }

        private void FireCheckup(CheckupItem it)
        {
            _checkDialogOpen = true;
            ShowCheckupPrompt(it);
        }

        private void ShowCheckupPrompt(CheckupItem it)
        {
            try
            {
                using var dlg = new CountdownPromptForm(
                    "Checklist",
                    $"{it.Label}",
                    "Pronto", "Ainda Não",
                    0, insistent: true);

                var r = dlg.ShowDialog();

                if (r == DialogResult.OK) // Sim
                {
                    it.LastDoneDate = DateTime.Now.Date;
                    CheckupStore.Save(_checkups);
                    _tray.BalloonTipTitle = "Anotado";
                    _tray.BalloonTipText = $"\"{it.Label}\" marcado como feito.";
                    _tray.ShowBalloonTip(2000);
                    _checkDialogOpen = false;
                }
                else // Não → bloqueia 1 min e repete
                {
                    BlockAndRepeatCheckup(it);
                }
            }
            catch
            {
                _checkDialogOpen = false;
            }
        }

        private void BlockAndRepeatCheckup(CheckupItem it)
        {
            var overlay = new OverlayForm(1);
            overlay.FormClosed += (s, e) => ShowCheckupPrompt(it);
            overlay.Show();
        }

        private void OpenCheckupsFile()
        {
            try
            {
                _ = CheckupStore.Load(); // garante existência

                var psi = new ProcessStartInfo("notepad.exe", CheckupStore.PathForUser) { UseShellExecute = false };
                var p = Process.Start(psi);
                if (p != null)
                {
                    p.EnableRaisingEvents = true;
                    p.Exited += (s, e) => _ui.Post(_ => ReloadCheckups(), null);
                }
            }
            catch { /* silencioso */ }
        }

        private void ReloadCheckups()
        {
            try
            {
                _checkups = CheckupStore.Load();
                _tray.BalloonTipTitle = "Checkups Recarregados";
                _tray.BalloonTipText = $"{_checkups.Count} item(ns) carregado(s).";
                _tray.ShowBalloonTip(2000);
            }
            catch (Exception ex)
            {
                _tray.BalloonTipTitle = "Erro ao recarregar checkups";
                _tray.BalloonTipText = ex.Message;
                _tray.ShowBalloonTip(4000);
            }
        }

        private void OpenCheckupsDialog()
        {
            // Hard-Stop travado: mostra a hora atual (se armado) só para contexto
            var initialHS = _armed ? _nextHardStop : DateTime.Now;

            using var dlg = new StartupConfigForm(
                initialHardStop: initialHS,
                initialCheckups: _checkups,
                lockHardStop: true
            );

            if (dlg.ShowDialog() != DialogResult.OK) return;

            ApplyCheckupEditResult(dlg.ResultRows);
        }

        private void ApplyCheckupEditResult(List<(string label, TimeSpan time, bool enabled)> rows)
        {
            // Mapa rápido por label (case-insensitive)
            var map = new Dictionary<string, (TimeSpan time, bool enabled)>(StringComparer.OrdinalIgnoreCase);
            foreach (var (label, time, enabled) in rows)
                map[label] = (time, enabled);

            // Atualiza existentes; desativa os que sumiram
            foreach (var it in _checkups)
            {
                if (map.TryGetValue(it.Label ?? "", out var v))
                {
                    it.Time = v.time;
                    it.Enabled = v.enabled;
                }
                else
                {
                    // não veio no formulário → desativar (não apagar)
                    it.Enabled = false;
                }
            }

            // Adiciona novos marcados que não existiam
            foreach (var (label, time, enabled) in rows)
            {
                if (!enabled) continue;
                if (_checkups.Exists(x => string.Equals(x.Label, label, StringComparison.OrdinalIgnoreCase))) continue;

                _checkups.Add(new CheckupItem
                {
                    Label = label,
                    Time = time,
                    Enabled = true
                });
            }

            CheckupStore.Save(_checkups);
            ReloadCheckups();
        }

        // =========================
        // SAÍDA / LIMPEZA
        // =========================
        protected override void ExitThreadCore()
        {
            try
            {
                SaveState();
                SystemEvents.SessionSwitch -= OnSessionSwitch;

                _startupTimer?.Stop();
                _tMinus10Timer?.Stop();
                _t0Timer?.Stop();
                _checkTimer?.Stop();
                _tray?.Dispose();
            }
            finally
            {
                base.ExitThreadCore();
            }
        }
    }
}
