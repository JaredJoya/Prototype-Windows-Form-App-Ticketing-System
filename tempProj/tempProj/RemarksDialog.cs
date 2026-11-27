using System;
using System.Drawing;
using System.Windows.Forms;

namespace tempProj
{
    /// <summary>
    /// Asks the user for the reason / remarks behind a ticket change (status change, reopen, delete).
    /// The text is saved in the ticket history log that the admin portal shows.
    /// </summary>
    public sealed class RemarksDialog : Form
    {
        public const int MaxLength = 500;

        private readonly TextBox txtRemarks;
        private readonly Button btnOk;
        private readonly Label lblCounter;
        private readonly bool required;

        /// <summary>The remarks the user typed (trimmed). Empty when remarks are optional and left blank.</summary>
        public string Remarks { get; private set; }

        public RemarksDialog(string title, string prompt, bool required)
        {
            this.required = required;
            Remarks = "";

            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(440, 250);
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;

            Label lblPrompt = new Label
            {
                Text = prompt,
                AutoSize = false,
                Location = new Point(16, 14),
                Size = new Size(408, 44),
                ForeColor = Color.FromArgb(30, 42, 71)
            };

            Label lblLabel = new Label
            {
                Text = required ? "Reason (required)" : "Remarks (optional)",
                AutoSize = true,
                Location = new Point(16, 64),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            txtRemarks = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = MaxLength,
                Location = new Point(16, 86),
                Size = new Size(408, 100),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtRemarks.TextChanged += (s, e) => RefreshState();

            lblCounter = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(264, 188),
                Size = new Size(160, 18),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            btnOk = new Button
            {
                Text = "Continue",
                DialogResult = DialogResult.None,
                Location = new Point(244, 210),
                Size = new Size(88, 30)
            };
            btnOk.Click += (s, e) =>
            {
                Remarks = txtRemarks.Text.Trim();
                DialogResult = DialogResult.OK;
                Close();
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(338, 210),
                Size = new Size(88, 30)
            };

            Controls.AddRange(new Control[] { lblPrompt, lblLabel, txtRemarks, lblCounter, btnOk, btnCancel });
            AcceptButton = null; // Enter adds a new line in the box; use the buttons.
            CancelButton = btnCancel;
            RefreshState();
        }

        private void RefreshState()
        {
            int length = txtRemarks.Text.Trim().Length;
            btnOk.Enabled = !required || length > 0;
            lblCounter.Text = txtRemarks.Text.Length + " / " + MaxLength;
        }

        /// <summary>Shows the dialog. Returns null if the user cancelled, otherwise the remarks text.</summary>
        public static string Ask(IWin32Window owner, string title, string prompt, bool required)
        {
            using (RemarksDialog dialog = new RemarksDialog(title, prompt, required))
            {
                return dialog.ShowDialog(owner) == DialogResult.OK ? dialog.Remarks : null;
            }
        }
    }
}
