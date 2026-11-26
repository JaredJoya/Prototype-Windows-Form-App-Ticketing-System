using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace tempProj
{
    public partial class CreateTicket : Form
    {
        private readonly Func<Task> refreshParent;
        private readonly ApiService api = new ApiService();
        private int statusDropDownClosedAt;
        private TextBox txtEmail;
        private static readonly string EmailFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tempProj", "receipt_email.txt");
        public CreateTicket(Func<Task> refreshParent)
        {
            InitializeComponent();
            this.refreshParent = refreshParent;

            // Each card is taller than the control inside it, so make the whole card clickable.
            pnlTitleBox.Click += (s, e) => txtbox_title.Focus();
            pnlDescBox.Click += (s, e) => rtxtbox_description.Focus();
            pnlStatusBox.Click += pnlStatusBox_Click;
            cmbStatus.DropDownClosed += (s, e) => statusDropDownClosedAt = Environment.TickCount;
            cmbStatus.SelectedIndex = 0;
            BuildEmailField();
        }

        // Email card next to the Submit button. Built from the same controls as the Title box
        // (RoundedPanel + HintTextBox) so it matches the rest of the form; remembered per PC.
        private void BuildEmailField()
        {
            var slate = Color.FromArgb(100, 116, 139);
            var ink = Color.FromArgb(15, 23, 42);

            var pnlEmailBox = new RoundedPanel
            {
                BackColor = Color.White,
                BorderColor = Color.FromArgb(203, 213, 225),
                FocusBorderColor = Color.FromArgb(153, 27, 27),
                CornerRadius = 8,
                Cursor = Cursors.IBeam,
                Location = new Point(145, 24),
                Size = new Size(430, 42),
                Padding = new Padding(14, 11, 14, 0)
            };

            txtEmail = new HintTextBox
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = ink,
                HintText = "name@gmail.com"
            };

            var lblEmail = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 92,
                Text = "Receipt to",
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = slate,
                BackColor = Color.White
            };

            // Add order matters for docking: Fill first, then the Left label.
            pnlEmailBox.Controls.Add(txtEmail);
            pnlEmailBox.Controls.Add(lblEmail);
            pnlEmailBox.Click += (s, e) => txtEmail.Focus();
            lblEmail.Click += (s, e) => txtEmail.Focus();

            try { if (File.Exists(EmailFile)) txtEmail.Text = File.ReadAllText(EmailFile).Trim(); } catch { }
            pnlButtons.Controls.Add(pnlEmailBox);
        }

        private static bool IsValidEmail(string email)
        {
            try { return new System.Net.Mail.MailAddress(email).Address == email.Trim(); }
            catch { return false; }
        }

        private void CreateTicket_Load(object sender, EventArgs e)
        {

        }

        // Clicking anywhere on the status card (not just the combo box strip) opens the list.
        private void pnlStatusBox_Click(object sender, EventArgs e)
        {
            // If the list was just closed by this same click, don't re-open it.
            if (Environment.TickCount - statusDropDownClosedAt < 250)
                return;

            cmbStatus.Focus();
            cmbStatus.DroppedDown = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string tempTitle = Convert.ToString(txtbox_title.Text);
            string tempStatus = cmbStatus.SelectedItem != null ? cmbStatus.SelectedItem.ToString() : "";
            string Description = Convert.ToString(rtxtbox_description.Text);

            if (string.IsNullOrWhiteSpace(tempTitle) ||
                string.IsNullOrWhiteSpace(tempStatus) ||
                string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show("Please fill in the title, description, and status.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string email = txtEmail.Text.Trim();
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address to receive your ticket receipt.",
                    "Email Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            btnSubmit.Enabled = false;
            lblReady.Text = "Submitting...";

            try
            {
                Ticket newTicket = new Ticket
                {
                    Title = tempTitle,
                    Description = Description,
                    Status = tempStatus,
                    CreatedBy = Environment.UserName,
                    ComputerName = Environment.MachineName,
                    ReceiptEmail = email
                };

                await api.CreateTicketAsync(newTicket);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(EmailFile));
                    File.WriteAllText(EmailFile, email);
                }
                catch { }
                await refreshParent();

                // Clear the form only after a successful submit, so a failed
                // attempt doesn't lose what the user typed.
                txtbox_title.Text = "";
                rtxtbox_description.Text = "";
                cmbStatus.SelectedIndex = 0;
                lblReady.Text = "Submitted";

                MessageBox.Show("Ticket created successfully! The admin has been notified, and a receipt is being emailed to " + email + ".",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblReady.Text = "Submit failed";
                MessageBox.Show("Unable to submit ticket: " + ex.Message,
                    "Ticket Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally { btnSubmit.Enabled = true; }
        }
    }
}