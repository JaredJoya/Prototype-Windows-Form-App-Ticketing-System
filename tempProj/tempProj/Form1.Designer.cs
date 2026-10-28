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
            this.ticketDataGrid = new System.Windows.Forms.DataGridView();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.btnCreateTicket = new System.Windows.Forms.Button();
            this.completeTicketDataGrid = new System.Windows.Forms.DataGridView();
            this.btnExportCompleted = new System.Windows.Forms.Button();
            this.btnReopenTicket = new System.Windows.Forms.Button();
            this.rbtnAscending = new System.Windows.Forms.RadioButton();
            this.rbtnDescending = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.ticketDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.completeTicketDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ticketDataGrid
            // 
            this.ticketDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ticketDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ticketDataGrid.Location = new System.Drawing.Point(43, 27);
            this.ticketDataGrid.Name = "ticketDataGrid";
            this.ticketDataGrid.Size = new System.Drawing.Size(704, 203);
            this.ticketDataGrid.TabIndex = 0;
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.Location = new System.Drawing.Point(43, 260);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(146, 28);
            this.btnViewDetails.TabIndex = 1;
            this.btnViewDetails.Text = "View Active Tickets";
            this.btnViewDetails.UseVisualStyleBackColor = true;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // btnCreateTicket
            // 
            this.btnCreateTicket.Location = new System.Drawing.Point(210, 260);
            this.btnCreateTicket.Name = "btnCreateTicket";
            this.btnCreateTicket.Size = new System.Drawing.Size(146, 28);
            this.btnCreateTicket.TabIndex = 2;
            this.btnCreateTicket.Text = "Create Ticket";
            this.btnCreateTicket.UseVisualStyleBackColor = true;
            this.btnCreateTicket.Click += new System.EventHandler(this.btnCreateTicket_Click);
            // 
            // completeTicketDataGrid
            // 
            this.completeTicketDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.completeTicketDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.completeTicketDataGrid.Location = new System.Drawing.Point(43, 309);
            this.completeTicketDataGrid.Name = "completeTicketDataGrid";
            this.completeTicketDataGrid.Size = new System.Drawing.Size(704, 129);
            this.completeTicketDataGrid.TabIndex = 3;
            // 
            // btnExportCompleted
            // 
            this.btnExportCompleted.Location = new System.Drawing.Point(601, 260);
            this.btnExportCompleted.Name = "btnExportCompleted";
            this.btnExportCompleted.Size = new System.Drawing.Size(146, 28);
            this.btnExportCompleted.TabIndex = 4;
            this.btnExportCompleted.Text = "Export Completed Tickets";
            this.btnExportCompleted.UseVisualStyleBackColor = true;
            this.btnExportCompleted.Click += new System.EventHandler(this.btnExportCompleted_Click);
            // 
            // btnReopenTicket
            // 
            this.btnReopenTicket.Location = new System.Drawing.Point(488, 260);
            this.btnReopenTicket.Name = "btnReopenTicket";
            this.btnReopenTicket.Size = new System.Drawing.Size(98, 28);
            this.btnReopenTicket.TabIndex = 5;
            this.btnReopenTicket.Text = "Reopen Ticket";
            this.btnReopenTicket.UseVisualStyleBackColor = true;
            this.btnReopenTicket.Click += new System.EventHandler(this.btnReopenTicket_Click);
            // 
            // rbtnAscending
            // 
            this.rbtnAscending.AutoSize = true;
            this.rbtnAscending.Location = new System.Drawing.Point(753, 160);
            this.rbtnAscending.Name = "rbtnAscending";
            this.rbtnAscending.Size = new System.Drawing.Size(111, 17);
            this.rbtnAscending.TabIndex = 6;
            this.rbtnAscending.TabStop = true;
            this.rbtnAscending.Text = "Sort by Ascending";
            this.rbtnAscending.UseVisualStyleBackColor = true;
            this.rbtnAscending.CheckedChanged += new System.EventHandler(this.rbtnAscending_CheckedChanged);
            // 
            // rbtnDescending
            // 
            this.rbtnDescending.AutoSize = true;
            this.rbtnDescending.Location = new System.Drawing.Point(753, 183);
            this.rbtnDescending.Name = "rbtnDescending";
            this.rbtnDescending.Size = new System.Drawing.Size(118, 17);
            this.rbtnDescending.TabIndex = 7;
            this.rbtnDescending.TabStop = true;
            this.rbtnDescending.Text = "Sort by Descending";
            this.rbtnDescending.UseVisualStyleBackColor = true;
            this.rbtnDescending.CheckedChanged += new System.EventHandler(this.rbtnDescending_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 450);
            this.Controls.Add(this.rbtnDescending);
            this.Controls.Add(this.rbtnAscending);
            this.Controls.Add(this.btnReopenTicket);
            this.Controls.Add(this.btnExportCompleted);
            this.Controls.Add(this.completeTicketDataGrid);
            this.Controls.Add(this.btnCreateTicket);
            this.Controls.Add(this.btnViewDetails);
            this.Controls.Add(this.ticketDataGrid);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ticketDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.completeTicketDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ticketDataGrid;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Button btnCreateTicket;
        private System.Windows.Forms.DataGridView completeTicketDataGrid;
        private System.Windows.Forms.Button btnExportCompleted;
        private System.Windows.Forms.Button btnReopenTicket;
        private System.Windows.Forms.RadioButton rbtnAscending;
        private System.Windows.Forms.RadioButton rbtnDescending;
    }
}

