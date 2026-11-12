using Google.Cloud.Firestore;
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
    public partial class TicketDetails : Form
    {
        private readonly string ticketId;
        private readonly Func<Task> refreshParent;
        private bool canModifyTicket;
        private readonly ApiService api = new ApiService();
        private int statusDropDownClosedAt;
        public TicketDetails(string selectedTicketId, Func<Task> refreshParent)
        {
            InitializeComponent();
            ticketId = selectedTicketId;
            this.refreshParent = refreshParent;

            // The status card is taller than the combo box itself, so make the whole card clickable.
            pnlStatusBox.Click += pnlStatusBox_Click;
            cmbStatus.EnabledChanged += (s, e) => UpdateStatusCursor();
            cmbStatus.DropDownClosed += (s, e) => statusDropDownClosedAt = Environment.TickCount;
            UpdateStatusCursor();
        }

        private async void TicketDetails_Load(object sender, EventArgs e)
        {
            try
            {
                Ticket ticket = await api.GetTicketAsync(ticketId);
                if (ticket == null)
                {
                    MessageBox.Show("This Ticket no longer exists.");
                    Close();
                    return;
                }

                txtbox_title.Text = ticket.Title;
                cmbStatus.SelectedItem = ticket.Status;
                rtxtbox_description.Text = ticket.Description;
                UpdateStatusLine(ticket.Status);

                bool isCreator = string.Equals(ticket.CreatedBy, Environment.UserName,
                    StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(ticket.ComputerName, Environment.MachineName,
                    StringComparison.OrdinalIgnoreCase);

                bool isAdmin = false;
                try
                {
                    isAdmin = await api.IsAdminAsync();
                }
                catch
                {

                }

                canModifyTicket = isAdmin || isCreator;

                btnModify.Enabled = canModifyTicket;
                btnDelete.Enabled = canModifyTicket;
                btnUpdate.Enabled = false;
            }
            catch (Exception ex)
            {
                lblReady.Text = "Load failed";
                MessageBox.Show("Unable to load ticket: " + ex.Message,
                    "Ticket Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Updates the status bar line ("Ticket <id> | Status: <status>") and its dot colour.
        private void UpdateStatusLine(string status)
        {
            lblTicketInfo.Text = "Ticket " + ticketId + "  |  Status: " + status;
            lblStatusDot.ForeColor = string.Equals(status, "Complete", StringComparison.OrdinalIgnoreCase)
                ? Color.FromArgb(16, 185, 129)
                : Color.FromArgb(245, 158, 11);
        }

        // Hand cursor while the status box can be used, normal arrow while it is locked.
        private void UpdateStatusCursor()
        {
            Cursor c = cmbStatus.Enabled ? Cursors.Hand : Cursors.Default;
            cmbStatus.Cursor = c;
            pnlStatusBox.Cursor = c;
        }

        // Clicking anywhere on the status card (not just the combo box strip) opens the list.
        private void pnlStatusBox_Click(object sender, EventArgs e)
        {
            if (!cmbStatus.Enabled)
                return;

            // If the list was just closed by this same click, don't re-open it.
            if (Environment.TickCount - statusDropDownClosedAt < 250)
                return;

            cmbStatus.Focus();
            cmbStatus.DroppedDown = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            txtbox_title.ReadOnly = false;
            rtxtbox_description.ReadOnly = false;
            cmbStatus.Enabled = true;
            btnUpdate.Enabled = true;
            lblReady.Text = "Editing";
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string tempTitle = Convert.ToString(txtbox_title.Text);
            string tempStatus = cmbStatus.SelectedItem.ToString();
            string Description = Convert.ToString(rtxtbox_description.Text);

            if (tempStatus == "Complete")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to mark this ticket as Complete?\n\n" +
                    "Once completed, this ticket will be moved to the " +
                    "Completed Tickets list and will no longer be editable. \n\n" +
                    "Please make sure all work has been finished before continuing.",
                    "Comfirm Ticket Completion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            btnUpdate.Enabled = false;
            lblReady.Text = "Saving...";
            try
            {
                Ticket updatedTicket = new Ticket
                {
                    Id = ticketId,
                    Title = tempTitle,
                    Description = Description,
                    Status = tempStatus,

                    CreatedBy = Environment.UserName,
                    ComputerName = Environment.MachineName
                };

                await api.UpdateTicketAsync(ticketId, updatedTicket);
                await refreshParent();


                txtbox_title.ReadOnly = true;
                rtxtbox_description.ReadOnly = true;
                UpdateStatusLine(tempStatus);
                lblReady.Text = "Saved";

            }
            catch (Exception ex)
            {
                lblReady.Text = "Update failed";
                MessageBox.Show("Failed to update Ticket: " + ex.Message,
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnUpdate.Enabled = false;
                cmbStatus.Enabled = false;
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Are you sure you want to delete ticket {ticketId}?",
                "Confirm Ticket Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            btnDelete.Enabled = false;


            try
            {
                await api.DeleteTicketAsync(ticketId, Environment.UserName, Environment.MachineName);
                MessageBox.Show($"Ticket {ticketId} was deleted successfully.");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete ticket: \n {ex.Message}",
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnDelete.Enabled = true;
            }


        }
    }
}