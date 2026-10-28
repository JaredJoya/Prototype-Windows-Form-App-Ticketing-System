using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        private readonly Image urgentIcon = SystemIcons.Warning.ToBitmap();
        private readonly System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();
        private bool isLoadingTickets = false;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            DialogResult choice = MessageBox.Show("Choose which API server this computer should use:\n\n"+
                "Yes = Network 1 (192.168.1.*)\n\n"+
                "No = Network 2 (192.168.2.*)\n\n"+
                "Cancel = Exit the application", "Select Network",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (choice == DialogResult.Yes)
            {
                ApiService.SetBaseUrl("http://192.168.1.101:5075");
            }
            else if (choice == DialogResult.No)
            {
                ApiService.SetBaseUrl("http://192.168.2.55:5075");
            }
            else
            {
                Application.Exit();
                return;
            }
             
            ticketDataGrid.AutoGenerateColumns = true;
            ticketDataGrid.ReadOnly = true;
            ticketDataGrid.AllowUserToAddRows = false;
            ticketDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ticketDataGrid.MultiSelect = false;

            completeTicketDataGrid.AutoGenerateColumns = true;
            completeTicketDataGrid.ReadOnly = true;
            completeTicketDataGrid.AllowUserToAddRows= false;
            completeTicketDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            completeTicketDataGrid.MultiSelect= false;

            try
            {
                btnReopenTicket.Enabled = await api.IsAdminAsync();
            }
            catch (Exception ex)
            {
                btnReopenTicket.Enabled = false;
                MessageBox.Show("Unable to verify administrator permissions.\n\n"+
                    ex.Message,
                    "Permission Check Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            await LoadTicketsAsync();
            refreshTimer.Interval = 300000;
            refreshTimer.Tick += async (s, args) =>
            {
                await LoadTicketsAsync(false);
            };
            refreshTimer.Start();
        }


        private async Task LoadTicketsAsync(bool showErrors = true)
        {
            if (isLoadingTickets)
                return;
            isLoadingTickets = true;
            try
            {
                List<Ticket> tickets = (await api.GetTicketsAsync())
                    .OrderBy(t => ParseCreatedAt(t.CreatedAt)).ToList();

                List<Ticket> activeTickets = tickets.Where(t => !string.Equals(t.Status?.Trim(),
                    "Complete", StringComparison.OrdinalIgnoreCase)).ToList();

                if (rbtnDescending.Checked)
                {
                    activeTickets = activeTickets
                        .OrderByDescending(t => ParseCreatedAt(t.CreatedAt)).ToList();
                }
                else
                {
                    activeTickets = activeTickets
                        .OrderBy(t => ParseCreatedAt(t.CreatedAt)).ToList();
                }

                List<Ticket> completedTickets = tickets.Where(t => string.Equals(t.Status?.Trim(),
                    "Complete", StringComparison.OrdinalIgnoreCase)).ToList();

                ticketDataGrid.DataSource = null;
                ticketDataGrid.DataSource = activeTickets;
                ApplyUrgentTicketStyling();

                completeTicketDataGrid.DataSource = null;
                completeTicketDataGrid.DataSource = completedTickets;
            }
            catch (Exception ex) {
                if (showErrors)
                {
                    MessageBox.Show("Failed to load tickets: " + ex.Message,
                        "Ticket Loading Error",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Error);
                }
            }
            finally { isLoadingTickets = false; }
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

            TicketDetails ticketD = new TicketDetails(selectedTicketId, () => LoadTicketsAsync());
            ticketD.ShowDialog(this);
            await LoadTicketsAsync();
        }

        private async void btnCreateTicket_Click(object sender, EventArgs e)
        {
            CreateTicket CreateT = new CreateTicket(() => LoadTicketsAsync());
            CreateT.ShowDialog(this);
            await LoadTicketsAsync();
        }

        private void btnExportCompleted_Click(object sender, EventArgs e)
        {
            var rows = completeTicketDataGrid.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow).ToList();
            if (rows.Count == 0)
            {
                MessageBox.Show("There are no completed tickets to export.",
                    "Export Completed Tickets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Exported Completed Tickets";
                saveDialog.Filter = "CSV file (*.csv)|*.csv";
                saveDialog.FileName = $"CompletedTickets_{DateTime.Now:yyyyMMdd_hhmmss}.csv";
                saveDialog.DefaultExt = "csv";
                saveDialog.AddExtension = true;
                saveDialog.OverwritePrompt = true;

                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                try
                {
                    var columns = completeTicketDataGrid.Columns.Cast<DataGridViewColumn>()
                        .Where(column => column.Visible).OrderBy(column => column.DisplayIndex).ToList();

                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine(string.Join(",", columns.Select(column => EscapeCsv(column.HeaderText))));

                        foreach(DataGridViewRow row in rows)
                        {
                            string line = string.Join(",", columns.Select(column => EscapeCsv(Convert.ToString(row.Cells[column.Index].FormattedValue) ?? "")));
                            writer.WriteLine(line);
                        }
                    }
                    MessageBox.Show($"Successfully exported {rows.Count} completed tickets.\n\n" + 
                        $"Saved to:\n{saveDialog.FileName}",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to export completed tickets:\n" + ex.Message,
                        "Export Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static string EscapeCsv(string value)
        {
            value = value ?? "";

            string trimmed = value.TrimStart();

            if (trimmed.StartsWith("=") ||
                trimmed.StartsWith("+") ||
                trimmed.StartsWith("-") ||
                trimmed.StartsWith("@"))
            {
                value = "'" + value;
            }


            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private void ApplyUrgentTicketStyling()
        {
            if (!ticketDataGrid.Columns.Contains("UrgentIconColumn"))
            {
                DataGridViewImageColumn iconColumn = new DataGridViewImageColumn();
                iconColumn.Name = "UrgentIconColumn";
                iconColumn.HeaderText = "";
                iconColumn.Width = 30;
                iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                iconColumn.ReadOnly = true;
                iconColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                iconColumn.DefaultCellStyle.NullValue = null;
                ticketDataGrid.Columns.Insert(0, iconColumn);
            }

            foreach (DataGridViewRow row in ticketDataGrid.Rows)
            {
                Ticket ticket = row.DataBoundItem as Ticket;
                if (ticket == null)
                {
                    continue;
                }
                bool hasValidDate = DateTime.TryParseExact(ticket.CreatedAt,
                    "yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime createdAt);
                bool isUrgent = !string.Equals(ticket.Status?.Trim(), "Complete",
                    StringComparison.OrdinalIgnoreCase) && hasValidDate && DateTime.Now -
                    createdAt >= TimeSpan.FromDays(7);
                row.Cells["UrgentIconColumn"].Value = isUrgent ? urgentIcon : null;
                if (isUrgent)
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.DefaultCellStyle.SelectionBackColor = Color.Firebrick;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Empty;
                    row.DefaultCellStyle.ForeColor = Color.Empty;
                    row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                    row.DefaultCellStyle.SelectionForeColor = Color.Empty;
                }
            }
        }
        private async void btnReopenTicket_Click(object sender, EventArgs e)
        {
            if (completeTicketDataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a ticket first.");
                return;
            }

            DataGridViewRow selectedRow = completeTicketDataGrid.SelectedRows[0];
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

            TicketDetails ticketD = new TicketDetails(selectedTicketId, () => LoadTicketsAsync());
            ticketD.ShowDialog(this);
            await LoadTicketsAsync();
        }

        private async void rbtnAscending_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnAscending.Checked)
            {
                await LoadTicketsAsync(false);
            }
        }

        private async void rbtnDescending_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDescending.Checked)
            {
                await LoadTicketsAsync(false);
            }
        }

        private DateTime ParseCreatedAt(string createdAt)
        {
            bool validDate = DateTime.TryParseExact(createdAt, "yyyy-MM-dd hh:mm tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
            return validDate ? parsedDate : DateTime.MinValue;
        }
    }
}
