using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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

        private List<Ticket> allTickets = new List<Ticket>();
        private readonly System.Windows.Forms.Timer searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 250 };

        private Font pcNameFont;

        public Form1()
        {
            InitializeComponent();
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }   // app icon (Project > Properties > Application > Icon)
            lblCopyright.Text = "© 2026 Radyo Pilipino Media Group. All rights reserved.   |   RPMG Ticketing System v" + Application.ProductVersion;
            SetPcName(Environment.MachineName);

            searchDebounceTimer.Tick += (s, e) =>
            {
                searchDebounceTimer.Stop();
                ApplyFiltersAndBind();
            };


            // Placeholder text for the search box (the styling itself lives in Form1.Designer.cs).
            EventHandler setHint = (s, e) => SendMessage(txtsearch.Handle, EM_SETCUEBANNER, (IntPtr)1, "Search tickets...");
            if (txtsearch.IsHandleCreated) setHint(null, EventArgs.Empty);
            txtsearch.HandleCreated += setHint;
        }

        // ---------- Search box helpers (layout/colours are set in Form1.Designer.cs) ----------

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        // Clicking anywhere on the card (or its icon) puts the cursor in the search box.
        private void pnlSearchCard_Click(object sender, EventArgs e)
        {
            txtsearch.Focus();
        }

        // Draws the magnifier icon.
        private void pnlSearchIcon_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.FromArgb(100, 116, 139), 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                e.Graphics.DrawEllipse(pen, 4, 2, 11, 11);
                e.Graphics.DrawLine(pen, 13, 12, 18, 17);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Work out which network this PC is on automatically (default gateway / IP address, then
            // which API server answers). The selection dialog only appears when that can't be decided.
            NetworkProfile network = await NetworkDetector.DetectAsync();

            if (network == null)
            {
                using (NetworkSelectDialog dialog = new NetworkSelectDialog(
                    NetworkDetector.Networks, NetworkDetector.DescribeLocalNetwork()))
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        Application.Exit();
                        return;
                    }

                    network = dialog.SelectedNetwork;
                }
            }

            // Make sure the chosen server actually answers before loading anything, so a stopped API
            // gives one clear dialog (retry / choose another network / exit) instead of several errors.
            while (!await NetworkDetector.IsReachableAsync(network, 3000))
            {
                using (ServerUnreachableDialog unreachable = new ServerUnreachableDialog(
                    network, NetworkDetector.DescribeLocalNetwork()))
                {
                    unreachable.ShowDialog();

                    if (unreachable.Choice == ServerDialogChoice.Exit)
                    {
                        Application.Exit();
                        return;
                    }

                    if (unreachable.Choice == ServerDialogChoice.ChooseNetwork)
                    {
                        using (NetworkSelectDialog picker = new NetworkSelectDialog(
                            NetworkDetector.Networks, NetworkDetector.DescribeLocalNetwork(), "Back"))
                        {
                            if (picker.ShowDialog() == DialogResult.OK)
                            {
                                network = picker.SelectedNetwork;
                            }
                        }
                    }
                }
            }

            ApiService.SetBaseUrl(network.ServerUrl);

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
                allTickets = (await api.GetTicketsAsync()).ToList();
                ApplyFiltersAndBind();
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

        private void ApplyFiltersAndBind()
        {
            string term = GetSearchTerm();

            IEnumerable<Ticket> source = allTickets;
            if (term.Length > 0)
            {
                source = source.Where(t => TicketMatches(t, term));
            }

            List<Ticket> filtered = source.ToList();

            List<Ticket> activeTickets = filtered
                .Where(t => !string.Equals(t.Status?.Trim(), "Complete", StringComparison.OrdinalIgnoreCase))
                .ToList();

            activeTickets = rbtnDescending.Checked
                ? activeTickets.OrderByDescending(t => ParseCreatedAt(t.CreatedAt)).ToList()
                : activeTickets.OrderBy(t => ParseCreatedAt(t.CreatedAt)).ToList();

            List<Ticket> completedTickets = filtered
                .Where(t => string.Equals(t.Status?.Trim(), "Complete", StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => ParseCreatedAt(t.CreatedAt))
                .ToList();

            ticketDataGrid.DataSource = null;
            ticketDataGrid.DataSource = activeTickets;
            ApplyColumnSizing(ticketDataGrid);
            ApplyUrgentTicketStyling();

            completeTicketDataGrid.DataSource = null;
            completeTicketDataGrid.DataSource = completedTickets;
            ApplyColumnSizing(completeTicketDataGrid);

            UpdateSummary(activeTickets.Count, completedTickets.Count);
        }

        private string GetSearchTerm()
        {
            
            return txtsearch.Text.Trim();
        }

        private static bool TicketMatches(Ticket t, string term)
        {
            return Contains(t.Id, term)
                || Contains(t.Title, term)
                || Contains(t.Description, term)
                || Contains(t.Status, term)
                || Contains(t.ComputerName, term)
                || Contains(t.CreatedBy, term)
                || Contains(t.CreatedAt, term)
                || Contains(t.UpdatedAt, term);
        }

        private static bool Contains(string value, string term)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
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

        // Upper limit (px) for the content-based width of columns that can hold long free text.
        // Anything longer is cut with "..." in the cell and shown in full in the cell tooltip.
        private static readonly Dictionary<string, int> MaxColumnWidth =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Title",       320 },
                { "Description", 480 },
            };

        private const int MinColumnWidth = 60;
        private const int HeaderExtraWidth = 28; // header padding + room for the sort glyph

        private static void ApplyColumnSizing(DataGridView grid)
        {
            // Step 1: measure. Let every column size itself to its widest cell (including the
            // Status badge padding), so we know how much room each column needs to show its text.
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (string.Equals(column.Name, "ReceiptEmail", StringComparison.OrdinalIgnoreCase))
                {
                    // The requester's email is only used by the API for receipts; keep it out of the
                    // grids (and therefore out of the CSV export, which only writes visible columns).
                    column.Visible = false;
                    continue;
                }

                if (string.Equals(column.Name, "UrgentIconColumn", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                column.MinimumWidth = 20;

                // Extra right padding on Status leaves room for the badge.
                if (string.Equals(column.Name, "Status", StringComparison.OrdinalIgnoreCase))
                {
                    column.DefaultCellStyle.Padding = new Padding(8, 0, 36, 0);
                }
            }

            grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Step 2: fill. Each column's measured width becomes its minimum (so its text is
            // never cut unless it hit the cap above) and its weight, so any extra space - e.g.
            // when the form is maximized - is shared out in proportion to the content. If the
            // grid is too narrow for all minimums, a horizontal scrollbar appears instead.
            Font headerFont = grid.ColumnHeadersDefaultCellStyle.Font ?? grid.Font;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (!column.Visible)
                {
                    continue;
                }

                // The urgent-ticket icon column stays a fixed width instead of stretching.
                if (string.Equals(column.Name, "UrgentIconColumn", StringComparison.OrdinalIgnoreCase))
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    column.Width = 44;
                    column.MinimumWidth = 44;
                    column.Resizable = DataGridViewTriState.False;
                    continue;
                }

                // Headers are painted in upper case, which is wider than the stored header text.
                int headerWidth = TextRenderer.MeasureText(
                    (column.HeaderText ?? "").ToUpperInvariant(), headerFont).Width + HeaderExtraWidth;

                int preferred = Math.Max(Math.Max(column.Width, headerWidth), MinColumnWidth);

                if (MaxColumnWidth.TryGetValue(column.Name, out int max))
                {
                    preferred = Math.Min(preferred, Math.Max(max, headerWidth));
                }

                column.MinimumWidth = preferred;
                column.FillWeight = preferred;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.ToolTipText = null;
            }

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ShowCellToolTips = true; // truncated cells show their full text on hover
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
                iconColumn.Resizable = DataGridViewTriState.False;
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
                ApplyFiltersAndBind();
            }
        }

        private async void rbtnDescending_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDescending.Checked)
            {
                ApplyFiltersAndBind();
            }
        }

        private DateTime ParseCreatedAt(string createdAt)
        {
            bool validDate = DateTime.TryParseExact(createdAt, "yyyy-MM-dd hh:mm tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
            return validDate ? parsedDate : DateTime.MinValue;
        }

        private void SetPcName(string name)
        {
            string text = string.IsNullOrWhiteSpace(name) ? "Unknown PC" : name.Trim();
            int maxWidth = pcname.Width - 6;

            float size = 10F;
            Font font = new Font("Segoe UI", size, FontStyle.Bold);

            // Shrink the font (down to 7pt) until the name fits the label width.
            while (size > 7F && TextRenderer.MeasureText(text, font).Width > maxWidth)
            {
                font.Dispose();
                size -= 0.5F;
                font = new Font("Segoe UI", size, FontStyle.Bold);
            }

            Font old = pcNameFont;
            pcNameFont = font;
            pcname.Font = font;
            pcname.Text = text;
            old?.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
        }

       
        private void txtsearch_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void pcname_Click(object sender, EventArgs e)
        {

        }

        private void pnlSidebar_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}