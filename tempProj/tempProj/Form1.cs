using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private readonly Font statusPillFont = new Font("Segoe UI", 8.25F, FontStyle.Bold);
        private bool isLoadingTickets = false;

        public Form1()
        {
            InitializeComponent();
            lblVersion.Text = "Form1 v" + Application.ProductVersion;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            DialogResult choice = MessageBox.Show("Choose which API server this computer should use:\n\n" +
                "Yes = Network 1 (192.168.1.*)\n\n" +
                "No = Network 2 (192.168.2.*)\n\n" +
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
            completeTicketDataGrid.AllowUserToAddRows = false;
            completeTicketDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            completeTicketDataGrid.MultiSelect = false;

            try
            {
                btnReopenTicket.Enabled = await api.IsAdminAsync();
            }
            catch (Exception ex)
            {
                btnReopenTicket.Enabled = false;
                MessageBox.Show("Unable to verify administrator permissions.\n\n" +
                    ex.Message,
                    "Permission Check Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // The new design shows "Sort by Descending" selected by default.
            // Detach the handler so selecting it here does not trigger an extra reload.
            rbtnDescending.CheckedChanged -= rbtnDescending_CheckedChanged;
            rbtnDescending.Checked = true;
            rbtnDescending.CheckedChanged += rbtnDescending_CheckedChanged;

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
                ApplyColumnSizing(ticketDataGrid);
                ApplyUrgentTicketStyling();

                completeTicketDataGrid.DataSource = null;
                completeTicketDataGrid.DataSource = completedTickets;
                ApplyColumnSizing(completeTicketDataGrid);

                UpdateSummary(activeTickets.Count, completedTickets.Count);
                SetConnectionState(true);
            }
            catch (Exception ex)
            {
                SetConnectionState(false);
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

        // ---------- New design helpers ----------

        private void UpdateSummary(int active, int completed)
        {
            pillActiveCount.Text = active == 1 ? "1 Item" : active + " Items";
            pillCompletedCount.Text = completed == 1 ? "1 Item" : completed + " Items";
            lblCounts.Text = "•   Active: " + active + "   •   Completed: " + completed;
        }

        private void SetConnectionState(bool online)
        {
            Color color = online ? Color.FromArgb(16, 185, 129) : Color.FromArgb(220, 38, 38);
            lblConnDot.ForeColor = color;
            lblSystemDot.ForeColor = color;
            lblConnText.Text = online ? "Connected" : "Disconnected";
            lblSystemText.Text = online ? "System Online" : "System Offline";
        }

        private static void ApplyColumnSizing(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                // Extra right padding on Status leaves room for the badge.
                if (string.Equals(column.Name, "Status", StringComparison.OrdinalIgnoreCase))
                {
                    column.DefaultCellStyle.Padding = new Padding(8, 0, 36, 0);
                }
            }
        }

        /// <summary>
        /// Shared CellPainting handler for both grids: upper-case column headers
        /// and "pill" style badges in the Status column.
        /// </summary>
        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (e.ColumnIndex < 0)
            {
                return;
            }

            if (e.RowIndex == -1)
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                string header = (Convert.ToString(e.FormattedValue) ?? "").ToUpperInvariant();
                Rectangle textBounds = Rectangle.FromLTRB(
                    e.CellBounds.Left + e.CellStyle.Padding.Left,
                    e.CellBounds.Top,
                    e.CellBounds.Right - 4,
                    e.CellBounds.Bottom);

                TextRenderer.DrawText(e.Graphics, header, e.CellStyle.Font, textBounds,
                    e.CellStyle.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                e.Handled = true;
                return;
            }

            if (e.RowIndex < 0 ||
                !string.Equals(grid.Columns[e.ColumnIndex].Name, "Status", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Paint everything (selection colour, borders) except the plain text.
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

            string status = (Convert.ToString(e.FormattedValue) ?? "").Trim();
            if (status.Length == 0)
            {
                e.Handled = true;
                return;
            }

            Color back, border, fore;
            if (string.Equals(status, "Complete", StringComparison.OrdinalIgnoreCase))
            {
                back = Color.FromArgb(209, 250, 229);
                border = Color.FromArgb(110, 231, 183);
                fore = Color.FromArgb(4, 120, 87);
            }
            else if (string.Equals(status, "In Progress", StringComparison.OrdinalIgnoreCase))
            {
                back = Color.FromArgb(254, 243, 199);
                border = Color.FromArgb(252, 211, 77);
                fore = Color.FromArgb(146, 64, 14);
            }
            else
            {
                back = Color.FromArgb(226, 232, 240);
                border = Color.FromArgb(203, 213, 225);
                fore = Color.FromArgb(51, 65, 85);
            }

            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            Size textSize = TextRenderer.MeasureText(e.Graphics, status, statusPillFont,
                new Size(int.MaxValue, int.MaxValue), flags);

            int pillHeight = 26;
            int pillWidth = Math.Min(textSize.Width + 24, e.CellBounds.Width - 12);
            if (pillWidth < 10)
            {
                e.Handled = true;
                return;
            }

            Rectangle pill = new Rectangle(
                e.CellBounds.Left + 8,
                e.CellBounds.Top + (e.CellBounds.Height - pillHeight) / 2,
                pillWidth - 1,
                pillHeight - 1);

            SmoothingMode oldMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GraphicsHelper.RoundedRect(pill, pill.Height / 2))
            using (SolidBrush brush = new SolidBrush(back))
            using (Pen pen = new Pen(border, 1))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            e.Graphics.SmoothingMode = oldMode;

            TextRenderer.DrawText(e.Graphics, status, statusPillFont,
                new Rectangle(pill.X, pill.Y, pill.Width + 1, pill.Height + 1), fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            e.Handled = true;
        }

        // ---------- Existing logic ----------

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

                        foreach (DataGridViewRow row in rows)
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
                iconColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                iconColumn.Width = 44;
                iconColumn.MinimumWidth = 44;
                iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                iconColumn.ReadOnly = true;
                iconColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                iconColumn.DefaultCellStyle.NullValue = null;
                ticketDataGrid.Columns.Insert(0, iconColumn);
            }

            // In the new design urgent tickets are marked by the warning icon only;
            // row colours come from the grid styles set in the designer.
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