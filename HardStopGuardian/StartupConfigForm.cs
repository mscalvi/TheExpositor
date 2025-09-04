using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HardStopGuardian
{
    public class StartupConfigForm : Form
    {
        public DateTime SelectedHardStop { get; private set; }
        public List<(string label, TimeSpan time)> SelectedCheckups { get; } = new(); // apenas marcados
        public List<(string label, TimeSpan time, bool enabled)> ResultRows { get; } = new(); // todas as linhas visíveis

        private readonly DateTimePicker _dtHardStop = new()
        {
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true,
            Width = 100,
            Value = DateTime.Today.AddHours(11)
        };
        private Label _lblHard = new() { Text = "Hora:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };

        private const int MAX_ROWS = 5;
        private readonly TableLayoutPanel _grid = new()
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            Padding = new Padding(2)
        };

        private readonly List<Row> _rows = new();
        private readonly Button _ok = new() { Text = "OK", Width = 120 };
        private readonly Button _cancel = new() { Text = "Cancelar", Width = 120 };

        private readonly bool _lockHardStop;

        // Novo: permite inicializar checkups e travar hard-stop
        internal StartupConfigForm(
            DateTime? initialHardStop = null,
            IEnumerable<CheckupItem>? initialCheckups = null,
            bool lockHardStop = false)
        {
            _lockHardStop = lockHardStop;

            Text = "Configuração";
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            MaximizeBox = false; MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(640, 360);
            Padding = new Padding(14);
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            if (initialHardStop.HasValue)
                _dtHardStop.Value = DateTime.Today.AddHours(initialHardStop.Value.Hour)
                                               .AddMinutes(initialHardStop.Value.Minute);

            if (_lockHardStop)
            {
                _lblHard.Text = "Hora (travada):";
                _dtHardStop.Enabled = false;
            }

            // ----- Hard-Stop -----
            var gHard = new GroupBox { Text = "Hard-Stop (HH:mm)", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            var hardRow = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
            hardRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hardRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hardRow.Controls.Add(_lblHard, 0, 0);
            hardRow.Controls.Add(_dtHardStop, 1, 0);
            gHard.Controls.Add(hardRow);

            // ----- Checkups -----
            var gChk = new GroupBox { Text = "Checkups (opcionais)", Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // checkbox
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // textbox
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // "Hora:"
            _grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // time picker
            gChk.Controls.Add(_grid);

            BuildRows();

            // Pré-preencher com checkups existentes (edição)
            if (initialCheckups != null)
                PrefillRows(initialCheckups);

            // ----- Botões -----
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            buttons.Controls.Add(_ok);
            buttons.Controls.Add(_cancel);
            _ok.Click += OnOk;
            _cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // ----- Layout raiz -----
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            root.Controls.Add(gHard, 0, 0);
            root.Controls.Add(gChk, 0, 1);

            Controls.Add(buttons);
            Controls.Add(root);
        }

        private void BuildRows()
        {
            var defaults = new[]
            {
                new TimeSpan(8, 0, 0),
                new TimeSpan(8, 15, 0),
                new TimeSpan(11, 0, 0),
                new TimeSpan(13, 0, 0),
                new TimeSpan(18, 0, 0),
            };

            _grid.RowCount = MAX_ROWS;
            for (int i = 0; i < MAX_ROWS; i++)
            {
                _grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                var r = new Row(i, defaults[i % defaults.Length]);
                r.Check.CheckedChanged += (s, e) => OnRowToggled(r.Index);
                _rows.Add(r);

                _grid.Controls.Add(r.Check, 0, i);
                _grid.Controls.Add(r.Text, 1, i);
                _grid.Controls.Add(r.Lbl, 2, i);
                _grid.Controls.Add(r.Time, 3, i);
            }

            // só a 1ª visível por padrão
            SetRowVisible(0, true);
            for (int i = 1; i < MAX_ROWS; i++) SetRowVisible(i, false);
        }

        private void PrefillRows(IEnumerable<CheckupItem> items)
        {
            var list = items.ToList();
            for (int i = 0; i < Math.Min(list.Count, MAX_ROWS); i++)
            {
                var it = list[i];
                var r = _rows[i];

                r.Text.Text = it.Label ?? "";
                r.Check.Checked = it.Enabled;
                r.Time.Value = DateTime.Today.Add(it.Time);

                // garantir visibilidade encadeada
                SetRowVisible(i, true);
                if (i + 1 < MAX_ROWS) SetRowVisible(i + 1, true);
            }
        }

        private void OnRowToggled(int index)
        {
            if (_rows[index].Check.Checked)
            {
                // Marcou → revela a próxima (se existir)
                if (index + 1 < MAX_ROWS) SetRowVisible(index + 1, true);
            }
            else
            {
                // Desmarcou → NÃO esconda linhas já preenchidas;
                // só esconda as vazias após esta.
                for (int i = index + 1; i < MAX_ROWS; i++)
                {
                    var hasText = !string.IsNullOrWhiteSpace(_rows[i].Text.Text);
                    _rows[i].Check.Checked = false;

                    if (hasText)
                    {
                        // Mantém visível (apenas desmarca)
                        SetRowVisible(i, true);
                    }
                    else
                    {
                        // Sem texto → some
                        SetRowVisible(i, false);
                    }
                }
            }
        }


        private void SetRowVisible(int row, bool visible)
        {
            var r = _rows[row];
            r.Check.Visible = r.Text.Visible = r.Lbl.Visible = r.Time.Visible = visible;
            _grid.RowStyles[row].SizeType = visible ? SizeType.AutoSize : SizeType.Absolute;
            _grid.RowStyles[row].Height = visible ? 0 : 0;
        }

        private void OnOk(object? sender, EventArgs e)
        {
            SelectedHardStop = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                _dtHardStop.Value.Hour, _dtHardStop.Value.Minute, 0);

            SelectedCheckups.Clear();
            ResultRows.Clear();

            foreach (var r in _rows.Where(r => r.Check.Visible))
            {
                var label = (r.Text.Text ?? "").Trim();
                var ts = new TimeSpan(r.Time.Value.Hour, r.Time.Value.Minute, 0); // segundos = 00
                var enabled = r.Check.Checked;

                if (enabled && label.Length > 0)
                    SelectedCheckups.Add((label, ts)); // compatibilidade

                if (label.Length > 0) // só coleta linhas nomeadas
                    ResultRows.Add((label, ts, enabled));
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private sealed class Row
        {
            public int Index { get; }
            public CheckBox Check { get; }
            public TextBox Text { get; }
            public Label Lbl { get; }
            public DateTimePicker Time { get; }

            public Row(int index, TimeSpan initialTime)
            {
                Index = index;

                Check = new CheckBox { AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
                Text = new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    Width = 360,
                    PlaceholderText = index switch
                    {
                        0 => "Nome do checkup (ex.: Tomar remédio)",
                        1 => "Nome do checkup (ex.: Regar plantas)",
                        2 => "Nome do checkup (ex.: Fim do trabalho — lembrete)",
                        _ => "Outro checkup (opcional)"
                    }
                };
                Lbl = new Label { Text = "Hora:", AutoSize = true, Margin = new Padding(8, 8, 6, 0) };
                Time = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Width = 100,
                    Value = DateTime.Today.Add(initialTime)
                };
            }
        }
    }
}
