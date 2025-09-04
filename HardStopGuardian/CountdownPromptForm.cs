using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace HardStopGuardian
{
    public class CountdownPromptForm : Form
    {
        private readonly Label _msg = new() { AutoSize = true };
        private readonly Label _cd = new() { AutoSize = true };
        private readonly Button _primary = new() { Text = "OK", Enabled = false };
        private readonly Button _secondary = new() { Text = "Cancelar", Enabled = false };
        private readonly WinFormsTimer _tick = new() { Interval = 1000 };
        private readonly bool _insistent;
        private bool _allowClose;
        private int _remaining;

        public CountdownPromptForm(string title, string message,
                                   string primaryText, string secondaryText,
                                   int enableAfterSeconds,
                                   bool insistent = false)
        {
            _insistent = insistent;
            _remaining = Math.Max(0, enableAfterSeconds);

            // Dimensões e fontes maiores
            const int W = 560;                 // largura fixa para leitura
            const int PAD = 16;

            Text = title;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            MaximizeBox = false; MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = false;
            ClientSize = new Size(W, 200);
            MinimumSize = new Size(W, 180);
            Padding = new Padding(PAD);
            KeyPreview = true;

            _msg.MaximumSize = new Size(W - PAD * 2, 0);
            _msg.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
            _msg.Text = message;

            _cd.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic);
            _cd.Margin = new Padding(0, 8, 0, 12);

            _primary.Text = primaryText;
            _secondary.Text = secondaryText;
            _primary.Width = _secondary.Width = 110;
            _primary.Font = _secondary.Font = new Font(FontFamily.GenericSansSerif, 10);

            _primary.Click += (s, e) => { _allowClose = true; DialogResult = DialogResult.OK; Close(); };
            _secondary.Click += (s, e) => { _allowClose = true; DialogResult = DialogResult.Cancel; Close(); };

            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Padding = new Padding(0)
            };
            buttons.Controls.Add(_primary);
            buttons.Controls.Add(_secondary);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = false
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // mensagem
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // contador
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // espaço
            layout.Controls.Add(_msg, 0, 0);
            layout.Controls.Add(_cd, 0, 1);

            Controls.Add(layout);
            Controls.Add(buttons);

            UpdateCountdownLabel();
            _tick.Tick += (s, e) =>
            {
                _remaining--;
                if (_remaining <= 0)
                {
                    _tick.Stop();
                    _primary.Enabled = true;
                    _secondary.Enabled = true;
                    AcceptButton = _primary;
                    CancelButton = _secondary;
                    _cd.Text = "Pronto.";
                }
                else
                {
                    UpdateCountdownLabel();
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Activate(); BringToFront(); Focus(); // enfatiza foco
            if (_remaining > 0) _tick.Start();
            else { _primary.Enabled = _secondary.Enabled = true; _cd.Text = "Pronto."; }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (_insistent)
            {
                TopMost = true;
                Activate(); BringToFront(); Focus();
                System.Media.SystemSounds.Beep.Play();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Bloqueia Esc e Alt+F4 se insistente
            if (_insistent && (keyData == Keys.Escape || keyData == (Keys.Alt | Keys.F4)))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_insistent && !_allowClose && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }
            _tick.Stop();
            base.OnFormClosing(e);
        }

        private void UpdateCountdownLabel() => _cd.Text = $"Aguarde {_remaining}s...";
    }
}
