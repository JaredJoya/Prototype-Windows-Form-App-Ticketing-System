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
        public TicketDetails(string selectedTicketId, Func<Task> refreshParent)
        {
            InitializeComponent();
            ticketId = selectedTicketId;
            this.refreshParent = refreshParent;
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
                txtbox_status.Text = ticket.Status;
                rtxtbox_description.Text = ticket.Description;

                canModifyTicket = string.Equals(ticket.CreatedBy,
                    Environment.UserName,
                    StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(ticket.ComputerName,
                    Environment.MachineName,
                    StringComparison.OrdinalIgnoreCase);

                btnModify.Enabled = canModifyTicket;
                btnUpdate.Enabled = canModifyTicket;
                btnDelete.Enabled = canModifyTicket;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load ticket: " + ex.Message,
                    "Ticket Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
            txtbox_status.ReadOnly = false;
            rtxtbox_description.ReadOnly = false;
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string tempTitle = Convert.ToString(txtbox_title.Text);
            string tempStatus = Convert.ToString(txtbox_status.Text);
            string Description = Convert.ToString(rtxtbox_description.Text);
            btnUpdate.Enabled = false;
            try
            {
                Ticket updatedTicket = new Ticket
                {
                    Id = ticketId,
                    Title = txtbox_title.Text,
                    Description = rtxtbox_description.Text,
                    Status = txtbox_status.Text,

                    CreatedBy = Environment.UserName,
                    ComputerName = Environment.MachineName
                };

                await api.UpdateTicketAsync(ticketId, updatedTicket);
                await refreshParent();
               

                txtbox_title.ReadOnly = true;
                txtbox_status.ReadOnly = true;
                rtxtbox_description.ReadOnly = true;

            } catch (Exception ex) {
                MessageBox.Show("Failed to update Ticket: " + ex.Message,
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnUpdate.Enabled = true;
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
