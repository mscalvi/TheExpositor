using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace HardStopGuardian
{
    public class OverlayForm : Form
    {
        private int _seconds;
        private readonly Label _label = new();
        private readonly WinFormsTimer _tick = new() { Interval = 1000 };

        public OverlayForm(int minutes)
        {
            _seconds = Math.Max(1, minutes) * 60;

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            BackColor = Color.Black;
            Opacity = 0.85;

            _label.Dock = DockStyle.Fill;
            _label.TextAlign = ContentAlignment.MiddleCenter;
            _label.Font = new Font(FontFamily.GenericSansSerif, 48, FontStyle.Bold);
            _label.ForeColor = Color.White;

            Controls.Add(_label);

            _tick.Tick += (s, e) =>
            {
                _seconds--;
                UpdateLabel();
                if (_seconds <= 0)
                {
                    _tick.Stop();
                    Close();
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateLabel();
            _tick.Start();
        }

        private void UpdateLabel()
        {
            var ts = TimeSpan.FromSeconds(Math.Max(0, _seconds));
            _label.Text = $"Bloqueado: {ts:mm\\:ss}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_seconds > 0 && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }
    }
}
