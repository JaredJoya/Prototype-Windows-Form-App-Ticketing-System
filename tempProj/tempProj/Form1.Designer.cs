using System.Drawing;
using System.Windows.Forms;

namespace tempProj
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.cardActive = new tempProj.RoundedPanel();
            this.ticketDataGrid = new System.Windows.Forms.DataGridView();
            this.pnlActiveHeader = new System.Windows.Forms.Panel();
            this.lblActiveHint = new System.Windows.Forms.Label();
            this.pillActiveCount = new tempProj.PillLabel();
            this.lblActiveTitle = new System.Windows.Forms.Label();
            this.lblActiveDot = new System.Windows.Forms.Label();
            this.cardCompleted = new tempProj.RoundedPanel();
            this.completeTicketDataGrid = new System.Windows.Forms.DataGridView();
            this.pnlCompletedHeader = new System.Windows.Forms.Panel();
            this.lblCompletedHint = new System.Windows.Forms.Label();
            this.pillCompletedCount = new tempProj.PillLabel();
            this.lblCompletedTitle = new System.Windows.Forms.Label();
            this.lblCompletedDot = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlSort = new tempProj.RoundedPanel();
            this.rbtnDescending = new System.Windows.Forms.RadioButton();
            this.rbtnAscending = new System.Windows.Forms.RadioButton();
            this.lblSortOptions = new System.Windows.Forms.Label();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.pnlSidebarBorder = new System.Windows.Forms.Panel();
            this.pnlSystem = new System.Windows.Forms.Panel();
            this.lblSystemText = new System.Windows.Forms.Label();
            this.lblSystemDot = new System.Windows.Forms.Label();
            this.btnExportCompleted = new tempProj.RoundedButton();
            this.btnReopenTicket = new tempProj.RoundedButton();
            this.btnViewDetails = new tempProj.RoundedButton();
            this.btnCreateTicket = new tempProj.RoundedButton();
            this.lblWorkspaceSub = new System.Windows.Forms.Label();
            this.lblWorkspaceTitle = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlStatusBar = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCounts = new System.Windows.Forms.Label();
            this.lblConnText = new System.Windows.Forms.Label();
            this.lblConnDot = new System.Windows.Forms.Label();
            this.pnlStatusBorder = new System.Windows.Forms.Panel();
            this.pnlMain.SuspendLayout();
            this.tlpCards.SuspendLayout();
            this.cardActive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ticketDataGrid)).BeginInit();
            this.pnlActiveHeader.SuspendLayout();
            this.cardCompleted.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.completeTicketDataGrid)).BeginInit();
            this.pnlCompletedHeader.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlSort.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlStatusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlMain
            //
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(247)))), ((int)(((byte)(251)))));
            this.pnlMain.Controls.Add(this.tlpCards);
            this.pnlMain.Controls.Add(this.pnlTop);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(240, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.pnlMain.Size = new System.Drawing.Size(880, 650);
            this.pnlMain.TabIndex = 0;
            //
            // tlpCards
            //
            this.tlpCards.ColumnCount = 1;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCards.Controls.Add(this.cardActive, 0, 0);
            this.tlpCards.Controls.Add(this.cardCompleted, 0, 1);
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCards.Location = new System.Drawing.Point(24, 76);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.RowCount = 2;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tlpCards.Size = new System.Drawing.Size(832, 558);
            this.tlpCards.TabIndex = 1;
            //
            // cardActive
            //
            this.cardActive.BackColor = System.Drawing.Color.White;
            this.cardActive.CornerRadius = 12;
            this.cardActive.Controls.Add(this.ticketDataGrid);
            this.cardActive.Controls.Add(this.pnlActiveHeader);
            this.cardActive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardActive.Location = new System.Drawing.Point(0, 0);
            this.cardActive.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.cardActive.Name = "cardActive";
            this.cardActive.Padding = new System.Windows.Forms.Padding(1, 1, 1, 10);
            this.cardActive.Size = new System.Drawing.Size(832, 307);
            this.cardActive.TabIndex = 0;
            //
            // ticketDataGrid
            //
            this.ticketDataGrid.AllowUserToAddRows = false;
            this.ticketDataGrid.AllowUserToDeleteRows = false;
            this.ticketDataGrid.AllowUserToResizeRows = false;
            this.ticketDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ticketDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.ticketDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ticketDataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.ticketDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(8, 0, 28, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ticketDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ticketDataGrid.ColumnHeadersHeight = 40;
            this.ticketDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(8, 0, 16, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ticketDataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.ticketDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ticketDataGrid.EnableHeadersVisualStyles = false;
            this.ticketDataGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.ticketDataGrid.Location = new System.Drawing.Point(1, 53);
            this.ticketDataGrid.MultiSelect = false;
            this.ticketDataGrid.Name = "ticketDataGrid";
            this.ticketDataGrid.ReadOnly = true;
            this.ticketDataGrid.RowHeadersVisible = false;
            this.ticketDataGrid.RowTemplate.Height = 44;
            this.ticketDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ticketDataGrid.Size = new System.Drawing.Size(830, 243);
            this.ticketDataGrid.TabIndex = 1;
            this.ticketDataGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Grid_CellPainting);
            //
            // pnlActiveHeader
            //
            this.pnlActiveHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlActiveHeader.Controls.Add(this.lblActiveHint);
            this.pnlActiveHeader.Controls.Add(this.pillActiveCount);
            this.pnlActiveHeader.Controls.Add(this.lblActiveTitle);
            this.pnlActiveHeader.Controls.Add(this.lblActiveDot);
            this.pnlActiveHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActiveHeader.Location = new System.Drawing.Point(1, 1);
            this.pnlActiveHeader.Name = "pnlActiveHeader";
            this.pnlActiveHeader.Size = new System.Drawing.Size(830, 52);
            this.pnlActiveHeader.TabIndex = 0;
            //
            // lblActiveHint
            //
            this.lblActiveHint.BackColor = System.Drawing.Color.Transparent;
            this.lblActiveHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblActiveHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblActiveHint.Location = new System.Drawing.Point(680, 0);
            this.lblActiveHint.Name = "lblActiveHint";
            this.lblActiveHint.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lblActiveHint.Size = new System.Drawing.Size(150, 52);
            this.lblActiveHint.TabIndex = 3;
            this.lblActiveHint.Text = "Data Grid";
            this.lblActiveHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pillActiveCount
            //
            this.pillActiveCount.AutoSize = true;
            this.pillActiveCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(199)))));
            this.pillActiveCount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(211)))), ((int)(((byte)(77)))));
            this.pillActiveCount.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pillActiveCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(64)))), ((int)(((byte)(14)))));
            this.pillActiveCount.Location = new System.Drawing.Point(164, 14);
            this.pillActiveCount.Name = "pillActiveCount";
            this.pillActiveCount.Size = new System.Drawing.Size(66, 23);
            this.pillActiveCount.TabIndex = 2;
            this.pillActiveCount.Text = "0 Items";
            //
            // lblActiveTitle
            //
            this.lblActiveTitle.AutoSize = true;
            this.lblActiveTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblActiveTitle.Location = new System.Drawing.Point(38, 17);
            this.lblActiveTitle.Name = "lblActiveTitle";
            this.lblActiveTitle.Size = new System.Drawing.Size(114, 17);
            this.lblActiveTitle.TabIndex = 1;
            this.lblActiveTitle.Text = "ACTIVE TICKETS";
            //
            // lblActiveDot
            //
            this.lblActiveDot.AutoSize = true;
            this.lblActiveDot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveDot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.lblActiveDot.Location = new System.Drawing.Point(20, 18);
            this.lblActiveDot.Name = "lblActiveDot";
            this.lblActiveDot.Size = new System.Drawing.Size(16, 15);
            this.lblActiveDot.TabIndex = 0;
            this.lblActiveDot.Text = "●";
            //
            // cardCompleted
            //
            this.cardCompleted.BackColor = System.Drawing.Color.White;
            this.cardCompleted.CornerRadius = 12;
            this.cardCompleted.Controls.Add(this.completeTicketDataGrid);
            this.cardCompleted.Controls.Add(this.pnlCompletedHeader);
            this.cardCompleted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardCompleted.Location = new System.Drawing.Point(0, 323);
            this.cardCompleted.Margin = new System.Windows.Forms.Padding(0);
            this.cardCompleted.Name = "cardCompleted";
            this.cardCompleted.Padding = new System.Windows.Forms.Padding(1, 1, 1, 10);
            this.cardCompleted.Size = new System.Drawing.Size(832, 235);
            this.cardCompleted.TabIndex = 1;
            //
            // completeTicketDataGrid
            //
            this.completeTicketDataGrid.AllowUserToAddRows = false;
            this.completeTicketDataGrid.AllowUserToDeleteRows = false;
            this.completeTicketDataGrid.AllowUserToResizeRows = false;
            this.completeTicketDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.completeTicketDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.completeTicketDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.completeTicketDataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.completeTicketDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(8, 0, 28, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.completeTicketDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.completeTicketDataGrid.ColumnHeadersHeight = 40;
            this.completeTicketDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(8, 0, 16, 0);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(136)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.completeTicketDataGrid.DefaultCellStyle = dataGridViewCellStyle4;
            this.completeTicketDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.completeTicketDataGrid.EnableHeadersVisualStyles = false;
            this.completeTicketDataGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.completeTicketDataGrid.Location = new System.Drawing.Point(1, 53);
            this.completeTicketDataGrid.MultiSelect = false;
            this.completeTicketDataGrid.Name = "completeTicketDataGrid";
            this.completeTicketDataGrid.ReadOnly = true;
            this.completeTicketDataGrid.RowHeadersVisible = false;
            this.completeTicketDataGrid.RowTemplate.Height = 44;
            this.completeTicketDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.completeTicketDataGrid.Size = new System.Drawing.Size(830, 172);
            this.completeTicketDataGrid.TabIndex = 1;
            this.completeTicketDataGrid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Grid_CellPainting);
            //
            // pnlCompletedHeader
            //
            this.pnlCompletedHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlCompletedHeader.Controls.Add(this.lblCompletedHint);
            this.pnlCompletedHeader.Controls.Add(this.pillCompletedCount);
            this.pnlCompletedHeader.Controls.Add(this.lblCompletedTitle);
            this.pnlCompletedHeader.Controls.Add(this.lblCompletedDot);
            this.pnlCompletedHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCompletedHeader.Location = new System.Drawing.Point(1, 1);
            this.pnlCompletedHeader.Name = "pnlCompletedHeader";
            this.pnlCompletedHeader.Size = new System.Drawing.Size(830, 52);
            this.pnlCompletedHeader.TabIndex = 0;
            //
            // lblCompletedHint
            //
            this.lblCompletedHint.BackColor = System.Drawing.Color.Transparent;
            this.lblCompletedHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCompletedHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletedHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblCompletedHint.Location = new System.Drawing.Point(680, 0);
            this.lblCompletedHint.Name = "lblCompletedHint";
            this.lblCompletedHint.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lblCompletedHint.Size = new System.Drawing.Size(150, 52);
            this.lblCompletedHint.TabIndex = 3;
            this.lblCompletedHint.Text = "Resolved Archive";
            this.lblCompletedHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pillCompletedCount
            //
            this.pillCompletedCount.AutoSize = true;
            this.pillCompletedCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(250)))), ((int)(((byte)(229)))));
            this.pillCompletedCount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(231)))), ((int)(((byte)(183)))));
            this.pillCompletedCount.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pillCompletedCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(120)))), ((int)(((byte)(87)))));
            this.pillCompletedCount.Location = new System.Drawing.Point(194, 14);
            this.pillCompletedCount.Name = "pillCompletedCount";
            this.pillCompletedCount.Size = new System.Drawing.Size(66, 23);
            this.pillCompletedCount.TabIndex = 2;
            this.pillCompletedCount.Text = "0 Items";
            //
            // lblCompletedTitle
            //
            this.lblCompletedTitle.AutoSize = true;
            this.lblCompletedTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletedTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblCompletedTitle.Location = new System.Drawing.Point(38, 17);
            this.lblCompletedTitle.Name = "lblCompletedTitle";
            this.lblCompletedTitle.Size = new System.Drawing.Size(143, 17);
            this.lblCompletedTitle.TabIndex = 1;
            this.lblCompletedTitle.Text = "COMPLETED TICKETS";
            //
            // lblCompletedDot
            //
            this.lblCompletedDot.AutoSize = true;
            this.lblCompletedDot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletedDot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblCompletedDot.Location = new System.Drawing.Point(20, 18);
            this.lblCompletedDot.Name = "lblCompletedDot";
            this.lblCompletedDot.Size = new System.Drawing.Size(16, 15);
            this.lblCompletedDot.TabIndex = 0;
            this.lblCompletedDot.Text = "●";
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.pnlSort);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(24, 16);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.pnlTop.Size = new System.Drawing.Size(832, 60);
            this.pnlTop.TabIndex = 0;
            //
            // pnlSort
            //
            this.pnlSort.BackColor = System.Drawing.Color.White;
            this.pnlSort.Controls.Add(this.rbtnDescending);
            this.pnlSort.Controls.Add(this.rbtnAscending);
            this.pnlSort.Controls.Add(this.lblSortOptions);
            this.pnlSort.CornerRadius = 10;
            this.pnlSort.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSort.Location = new System.Drawing.Point(402, 0);
            this.pnlSort.Name = "pnlSort";
            this.pnlSort.Size = new System.Drawing.Size(430, 44);
            this.pnlSort.TabIndex = 0;
            //
            // rbtnDescending
            //
            this.rbtnDescending.AutoSize = true;
            this.rbtnDescending.BackColor = System.Drawing.Color.Transparent;
            this.rbtnDescending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnDescending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.rbtnDescending.Location = new System.Drawing.Point(278, 13);
            this.rbtnDescending.Name = "rbtnDescending";
            this.rbtnDescending.Size = new System.Drawing.Size(136, 19);
            this.rbtnDescending.TabIndex = 2;
            this.rbtnDescending.Text = "Sort by Descending";
            this.rbtnDescending.UseVisualStyleBackColor = false;
            this.rbtnDescending.CheckedChanged += new System.EventHandler(this.rbtnDescending_CheckedChanged);
            //
            // rbtnAscending
            //
            this.rbtnAscending.AutoSize = true;
            this.rbtnAscending.BackColor = System.Drawing.Color.Transparent;
            this.rbtnAscending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnAscending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.rbtnAscending.Location = new System.Drawing.Point(140, 13);
            this.rbtnAscending.Name = "rbtnAscending";
            this.rbtnAscending.Size = new System.Drawing.Size(129, 19);
            this.rbtnAscending.TabIndex = 1;
            this.rbtnAscending.Text = "Sort by Ascending";
            this.rbtnAscending.UseVisualStyleBackColor = false;
            this.rbtnAscending.CheckedChanged += new System.EventHandler(this.rbtnAscending_CheckedChanged);
            //
            // lblSortOptions
            //
            this.lblSortOptions.AutoSize = true;
            this.lblSortOptions.BackColor = System.Drawing.Color.Transparent;
            this.lblSortOptions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSortOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSortOptions.Location = new System.Drawing.Point(18, 15);
            this.lblSortOptions.Name = "lblSortOptions";
            this.lblSortOptions.Size = new System.Drawing.Size(93, 13);
            this.lblSortOptions.TabIndex = 0;
            this.lblSortOptions.Text = "SORT OPTIONS:";
            //
            // pnlSidebar
            //
            this.pnlSidebar.BackColor = System.Drawing.Color.White;
            this.pnlSidebar.Controls.Add(this.pnlSystem);
            this.pnlSidebar.Controls.Add(this.btnExportCompleted);
            this.pnlSidebar.Controls.Add(this.btnReopenTicket);
            this.pnlSidebar.Controls.Add(this.btnViewDetails);
            this.pnlSidebar.Controls.Add(this.btnCreateTicket);
            this.pnlSidebar.Controls.Add(this.lblWorkspaceSub);
            this.pnlSidebar.Controls.Add(this.lblWorkspaceTitle);
            this.pnlSidebar.Controls.Add(this.picLogo);
            this.pnlSidebar.Controls.Add(this.pnlSidebarBorder);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(240, 650);
            this.pnlSidebar.TabIndex = 1;
            //
            // pnlSidebarBorder
            //
            this.pnlSidebarBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.pnlSidebarBorder.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSidebarBorder.Location = new System.Drawing.Point(239, 0);
            this.pnlSidebarBorder.Name = "pnlSidebarBorder";
            this.pnlSidebarBorder.Size = new System.Drawing.Size(1, 650);
            this.pnlSidebarBorder.TabIndex = 8;
            //
            // pnlSystem
            //
            this.pnlSystem.BackColor = System.Drawing.Color.White;
            this.pnlSystem.Controls.Add(this.lblSystemText);
            this.pnlSystem.Controls.Add(this.lblSystemDot);
            this.pnlSystem.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSystem.Location = new System.Drawing.Point(0, 602);
            this.pnlSystem.Name = "pnlSystem";
            this.pnlSystem.Size = new System.Drawing.Size(239, 48);
            this.pnlSystem.TabIndex = 7;
            //
            // lblSystemText
            //
            this.lblSystemText.AutoSize = true;
            this.lblSystemText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSystemText.Location = new System.Drawing.Point(38, 16);
            this.lblSystemText.Name = "lblSystemText";
            this.lblSystemText.Size = new System.Drawing.Size(86, 15);
            this.lblSystemText.TabIndex = 1;
            this.lblSystemText.Text = "System Online";
            //
            // lblSystemDot
            //
            this.lblSystemDot.AutoSize = true;
            this.lblSystemDot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblSystemDot.Location = new System.Drawing.Point(20, 16);
            this.lblSystemDot.Name = "lblSystemDot";
            this.lblSystemDot.Size = new System.Drawing.Size(16, 15);
            this.lblSystemDot.TabIndex = 0;
            this.lblSystemDot.Text = "●";
            //
            // btnExportCompleted
            //
            this.btnExportCompleted.BackColor = System.Drawing.Color.White;
            this.btnExportCompleted.CornerRadius = 8;
            this.btnExportCompleted.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportCompleted.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnExportCompleted.Glyph = tempProj.ButtonGlyph.Download;
            this.btnExportCompleted.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnExportCompleted.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnExportCompleted.Location = new System.Drawing.Point(14, 260);
            this.btnExportCompleted.Name = "btnExportCompleted";
            this.btnExportCompleted.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnExportCompleted.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnExportCompleted.Size = new System.Drawing.Size(212, 42);
            this.btnExportCompleted.TabIndex = 6;
            this.btnExportCompleted.Text = "Export Completed Tickets";
            this.btnExportCompleted.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportCompleted.UseVisualStyleBackColor = false;
            this.btnExportCompleted.Click += new System.EventHandler(this.btnExportCompleted_Click);
            //
            // btnReopenTicket
            //
            this.btnReopenTicket.BackColor = System.Drawing.Color.White;
            this.btnReopenTicket.CornerRadius = 8;
            this.btnReopenTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReopenTicket.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnReopenTicket.Glyph = tempProj.ButtonGlyph.Refresh;
            this.btnReopenTicket.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnReopenTicket.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnReopenTicket.Location = new System.Drawing.Point(14, 212);
            this.btnReopenTicket.Name = "btnReopenTicket";
            this.btnReopenTicket.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnReopenTicket.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnReopenTicket.Size = new System.Drawing.Size(212, 42);
            this.btnReopenTicket.TabIndex = 5;
            this.btnReopenTicket.Text = "Reopen Ticket";
            this.btnReopenTicket.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReopenTicket.UseVisualStyleBackColor = false;
            this.btnReopenTicket.Click += new System.EventHandler(this.btnReopenTicket_Click);
            //
            // btnViewDetails
            //
            this.btnViewDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnViewDetails.CornerRadius = 8;
            this.btnViewDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnViewDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnViewDetails.Glyph = tempProj.ButtonGlyph.Document;
            this.btnViewDetails.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(241)))));
            this.btnViewDetails.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.btnViewDetails.Location = new System.Drawing.Point(14, 164);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnViewDetails.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnViewDetails.Size = new System.Drawing.Size(212, 42);
            this.btnViewDetails.TabIndex = 4;
            this.btnViewDetails.Text = "View Active Tickets";
            this.btnViewDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnViewDetails.UseVisualStyleBackColor = false;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            //
            // btnCreateTicket
            //
            this.btnCreateTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(45)))));
            this.btnCreateTicket.CornerRadius = 8;
            this.btnCreateTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateTicket.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateTicket.ForeColor = System.Drawing.Color.White;
            this.btnCreateTicket.Glyph = tempProj.ButtonGlyph.Add;
            this.btnCreateTicket.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(22)))), ((int)(((byte)(38)))));
            this.btnCreateTicket.Location = new System.Drawing.Point(20, 92);
            this.btnCreateTicket.Name = "btnCreateTicket";
            this.btnCreateTicket.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(18)))), ((int)(((byte)(32)))));
            this.btnCreateTicket.Size = new System.Drawing.Size(200, 46);
            this.btnCreateTicket.TabIndex = 3;
            this.btnCreateTicket.Text = "Create Ticket";
            this.btnCreateTicket.UseVisualStyleBackColor = false;
            this.btnCreateTicket.Click += new System.EventHandler(this.btnCreateTicket_Click);
            //
            // lblWorkspaceSub
            //
            this.lblWorkspaceSub.AutoSize = true;
            this.lblWorkspaceSub.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkspaceSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblWorkspaceSub.Location = new System.Drawing.Point(74, 58);
            this.lblWorkspaceSub.Name = "lblWorkspaceSub";
            this.lblWorkspaceSub.Size = new System.Drawing.Size(93, 13);
            this.lblWorkspaceSub.TabIndex = 2;
            this.lblWorkspaceSub.Text = "Ticket Management";
            //
            // lblWorkspaceTitle
            // 
            this.lblWorkspaceTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkspaceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblWorkspaceTitle.Location = new System.Drawing.Point(74, 18);
            this.lblWorkspaceTitle.Name = "lblWorkspaceTitle";
            this.lblWorkspaceTitle.Size = new System.Drawing.Size(158, 38);
            this.lblWorkspaceTitle.TabIndex = 1;
            this.lblWorkspaceTitle.Text = "Radyo Pilipino Media Group";
            this.lblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // picLogo
            //
            this.picLogo.Image = global::tempProj.Properties.Resources.Company_Logo;
            this.picLogo.Location = new System.Drawing.Point(20, 22);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(46, 46);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            //
            // pnlStatusBar
            //
            this.pnlStatusBar.BackColor = System.Drawing.Color.White;
            this.pnlStatusBar.Controls.Add(this.lblVersion);
            this.pnlStatusBar.Controls.Add(this.lblCounts);
            this.pnlStatusBar.Controls.Add(this.lblConnText);
            this.pnlStatusBar.Controls.Add(this.lblConnDot);
            this.pnlStatusBar.Controls.Add(this.pnlStatusBorder);
            this.pnlStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Location = new System.Drawing.Point(0, 650);
            this.pnlStatusBar.Name = "pnlStatusBar";
            this.pnlStatusBar.Size = new System.Drawing.Size(1120, 30);
            this.pnlStatusBar.TabIndex = 2;
            //
            // lblVersion
            //
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblVersion.Location = new System.Drawing.Point(940, 1);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Padding = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.lblVersion.Size = new System.Drawing.Size(180, 29);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Form1 v1.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCounts
            //
            this.lblCounts.AutoSize = true;
            this.lblCounts.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCounts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCounts.Location = new System.Drawing.Point(106, 8);
            this.lblCounts.Name = "lblCounts";
            this.lblCounts.Size = new System.Drawing.Size(160, 13);
            this.lblCounts.TabIndex = 3;
            this.lblCounts.Text = "•   Active: 0   •   Completed: 0";
            //
            // lblConnText
            //
            this.lblConnText.AutoSize = true;
            this.lblConnText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblConnText.Location = new System.Drawing.Point(32, 8);
            this.lblConnText.Name = "lblConnText";
            this.lblConnText.Size = new System.Drawing.Size(60, 13);
            this.lblConnText.TabIndex = 2;
            this.lblConnText.Text = "Connected";
            //
            // lblConnDot
            //
            this.lblConnDot.AutoSize = true;
            this.lblConnDot.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnDot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblConnDot.Location = new System.Drawing.Point(16, 8);
            this.lblConnDot.Name = "lblConnDot";
            this.lblConnDot.Size = new System.Drawing.Size(15, 13);
            this.lblConnDot.TabIndex = 1;
            this.lblConnDot.Text = "●";
            //
            // pnlStatusBorder
            //
            this.pnlStatusBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.pnlStatusBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatusBorder.Location = new System.Drawing.Point(0, 0);
            this.pnlStatusBorder.Name = "pnlStatusBorder";
            this.pnlStatusBorder.Size = new System.Drawing.Size(1120, 1);
            this.pnlStatusBorder.TabIndex = 0;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(247)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(1120, 680);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlStatusBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(980, 640);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Radyo Pilipino Media Group";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlMain.ResumeLayout(false);
            this.tlpCards.ResumeLayout(false);
            this.cardActive.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ticketDataGrid)).EndInit();
            this.pnlActiveHeader.ResumeLayout(false);
            this.pnlActiveHeader.PerformLayout();
            this.cardCompleted.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.completeTicketDataGrid)).EndInit();
            this.pnlCompletedHeader.ResumeLayout(false);
            this.pnlCompletedHeader.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlSort.ResumeLayout(false);
            this.pnlSort.PerformLayout();
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            this.pnlSystem.ResumeLayout(false);
            this.pnlSystem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlStatusBar.ResumeLayout(false);
            this.pnlStatusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private tempProj.RoundedPanel cardActive;
        private System.Windows.Forms.DataGridView ticketDataGrid;
        private System.Windows.Forms.Panel pnlActiveHeader;
        private System.Windows.Forms.Label lblActiveHint;
        private tempProj.PillLabel pillActiveCount;
        private System.Windows.Forms.Label lblActiveTitle;
        private System.Windows.Forms.Label lblActiveDot;
        private tempProj.RoundedPanel cardCompleted;
        private System.Windows.Forms.DataGridView completeTicketDataGrid;
        private System.Windows.Forms.Panel pnlCompletedHeader;
        private System.Windows.Forms.Label lblCompletedHint;
        private tempProj.PillLabel pillCompletedCount;
        private System.Windows.Forms.Label lblCompletedTitle;
        private System.Windows.Forms.Label lblCompletedDot;
        private System.Windows.Forms.Panel pnlTop;
        private tempProj.RoundedPanel pnlSort;
        private System.Windows.Forms.RadioButton rbtnDescending;
        private System.Windows.Forms.RadioButton rbtnAscending;
        private System.Windows.Forms.Label lblSortOptions;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlSidebarBorder;
        private System.Windows.Forms.Panel pnlSystem;
        private System.Windows.Forms.Label lblSystemText;
        private System.Windows.Forms.Label lblSystemDot;
        private tempProj.RoundedButton btnExportCompleted;
        private tempProj.RoundedButton btnReopenTicket;
        private tempProj.RoundedButton btnViewDetails;
        private tempProj.RoundedButton btnCreateTicket;
        private System.Windows.Forms.Label lblWorkspaceSub;
        private System.Windows.Forms.Label lblWorkspaceTitle;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel pnlStatusBar;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCounts;
        private System.Windows.Forms.Label lblConnText;
        private System.Windows.Forms.Label lblConnDot;
        private System.Windows.Forms.Panel pnlStatusBorder;
    }
}