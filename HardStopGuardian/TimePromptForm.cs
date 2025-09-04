using System;
using System.Windows.Forms;

namespace HardStopGuardian
{
    public class TimePromptForm : Form
    {
        private readonly DateTimePicker _time = new()
        {
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true,
            Width = 100,
            Value = DateTime.Now
        };
        private readonly Button _ok = new() { Text = "OK", DialogResult = DialogResult.OK };
        private readonly Button _cancel = new() { Text = "Cancelar", DialogResult = DialogResult.Cancel };
        public void SetInitialTime(DateTime dt) => _time.Value = dt;

        public DateTime SelectedTime => _time.Value;

        public TimePromptForm()
        {
            Text = "Hora do hard-stop";
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            layout.Controls.Add(new Label { Text = "Escolha a hora (HH:mm):", AutoSize = true });
            layout.Controls.Add(_time);
            layout.Controls.Add(_ok);
            layout.Controls.Add(_cancel);
            Controls.Add(layout);

            AcceptButton = _ok;
            CancelButton = _cancel;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
        }
    }
}
