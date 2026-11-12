using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tempProj
{
    public partial class CreateTicket : Form
    {
        private readonly Func<Task> refreshParent;
        private readonly ApiService api = new ApiService();
        private int statusDropDownClosedAt;
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
                    ComputerName = Environment.MachineName
                };

                await api.CreateTicketAsync(newTicket);
                await refreshParent();

                // Clear the form only after a successful submit, so a failed
                // attempt doesn't lose what the user typed.
                txtbox_title.Text = "";
                rtxtbox_description.Text = "";
                cmbStatus.SelectedIndex = 0;
                lblReady.Text = "Submitted";

                MessageBox.Show("Ticket created successfully!",
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