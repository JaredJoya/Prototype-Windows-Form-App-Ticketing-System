using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Logging;
using Google.Cloud.Firestore;

namespace tempProj
{
    public partial class Form1 : Form
    {
        private readonly ApiService api = new ApiService();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            ticketDataGrid.AutoGenerateColumns = true;
            ticketDataGrid.ReadOnly = true;
            ticketDataGrid.AllowUserToAddRows = false;
            ticketDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ticketDataGrid.MultiSelect = false;

            await LoadTicketsAsync();
        }


        private async Task LoadTicketsAsync()
        {
            try
            {
                List<Ticket> tickets = await api.GetTicketsAsync();

                ticketDataGrid.DataSource = null;
                ticketDataGrid.DataSource = tickets;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load tickets: " + ex.Message,
                    "Ticket Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private async void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (ticketDataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a ticket first.");
                return;
            }

            DataGridViewRow selectedRow = ticketDataGrid.SelectedRows[0];
            Ticket selectedTicket = selectedRow.DataBoundItem as Ticket;
            
            if (selectedTicket == null)
            {
                MessageBox.Show("Unable to retrieve the selected ticket." + "Please refresh the ticket list and try again.");
                return;
            }

            string selectedTicketId = selectedTicket.Id;
            if (string.IsNullOrWhiteSpace(selectedTicketId))
            {
                MessageBox.Show("The selected ticket has no Id.");
                return;
            }

            TicketDetails ticketD = new TicketDetails(selectedTicketId, LoadTicketsAsync);
            ticketD.ShowDialog(this);
            await LoadTicketsAsync();
        }

        private async void btnCreateTicket_Click(object sender, EventArgs e)
        {
            CreateTicket CreateT = new CreateTicket(LoadTicketsAsync);
            CreateT.ShowDialog(this);
            await LoadTicketsAsync();
        }
    }
}
