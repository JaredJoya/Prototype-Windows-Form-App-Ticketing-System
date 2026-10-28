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
        public CreateTicket(Func<Task> refreshParent)
        {
            InitializeComponent();
            this.refreshParent = refreshParent;
        }

        private void CreateTicket_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string tempTitle = Convert.ToString(txtbox_title.Text);
            string tempStatus = cmbStatus.SelectedItem.ToString();
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

                MessageBox.Show("Ticket created successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show("Unable to submit ticket: " + ex.Message,
                    "Ticket Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            } finally { btnSubmit.Enabled = true; }

            txtbox_title.Text = "";
            cmbStatus.Items.Clear();
            rtxtbox_description.Text = "";

        }
    }
}
