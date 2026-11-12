namespace tempProj
{
    partial class CreateTicket
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTicket));
            this.pnlContent = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlTitleBox = new tempProj.RoundedPanel();
            this.txtbox_title = new tempProj.HintTextBox();
            this.pnlStatusBox = new tempProj.RoundedPanel();
            this.cmbStatus = new tempProj.ModernComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlDescBox = new tempProj.RoundedPanel();
            this.rtxtbox_description = new tempProj.HintRichTextBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnSubmit = new tempProj.RoundedButton();
            this.pnlStatusBar = new System.Windows.Forms.Panel();
            this.lblReady = new System.Windows.Forms.Label();
            this.lblTicketInfo = new System.Windows.Forms.Label();
            this.lblStatusDot = new System.Windows.Forms.Label();
            this.pnlStatusBorder = new System.Windows.Forms.Panel();
            this.pnlContent.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.pnlTitleBox.SuspendLayout();
            this.pnlStatusBox.SuspendLayout();
            this.pnlDescBox.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Controls.Add(this.tlpMain);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(50, 30, 50, 16);
            this.pnlContent.Size = new System.Drawing.Size(960, 566);
            this.pnlContent.TabIndex = 0;
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.label1, 0, 0);
            this.tlpMain.Controls.Add(this.label2, 1, 0);
            this.tlpMain.Controls.Add(this.pnlTitleBox, 0, 1);
            this.tlpMain.Controls.Add(this.pnlStatusBox, 1, 1);
            this.tlpMain.Controls.Add(this.label3, 0, 2);
            this.tlpMain.Controls.Add(this.pnlDescBox, 0, 3);
            this.tlpMain.Controls.Add(this.pnlButtons, 0, 4);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(50, 30);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tlpMain.Size = new System.Drawing.Size(860, 520);
            this.tlpMain.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 4);
            this.label1.Size = new System.Drawing.Size(430, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.label2.Location = new System.Drawing.Point(442, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(2, 0, 0, 4);
            this.label2.Size = new System.Drawing.Size(418, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Status";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // pnlTitleBox
            // 
            this.pnlTitleBox.BackColor = System.Drawing.Color.White;
            this.pnlTitleBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.pnlTitleBox.Controls.Add(this.txtbox_title);
            this.pnlTitleBox.CornerRadius = 8;
            this.pnlTitleBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pnlTitleBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTitleBox.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.pnlTitleBox.Location = new System.Drawing.Point(0, 34);
            this.pnlTitleBox.Margin = new System.Windows.Forms.Padding(0, 2, 12, 2);
            this.pnlTitleBox.Name = "pnlTitleBox";
            this.pnlTitleBox.Padding = new System.Windows.Forms.Padding(14, 15, 14, 0);
            this.pnlTitleBox.Size = new System.Drawing.Size(418, 52);
            this.pnlTitleBox.TabIndex = 2;
            // 
            // txtbox_title
            // 
            this.txtbox_title.BackColor = System.Drawing.Color.White;
            this.txtbox_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtbox_title.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtbox_title.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbox_title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtbox_title.HintText = "Enter ticket title...";
            this.txtbox_title.Location = new System.Drawing.Point(14, 15);
            this.txtbox_title.Name = "txtbox_title";
            this.txtbox_title.Size = new System.Drawing.Size(390, 19);
            this.txtbox_title.TabIndex = 0;
            // 
            // pnlStatusBox
            // 
            this.pnlStatusBox.BackColor = System.Drawing.Color.White;
            this.pnlStatusBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.pnlStatusBox.Controls.Add(this.cmbStatus);
            this.pnlStatusBox.CornerRadius = 8;
            this.pnlStatusBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlStatusBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatusBox.Location = new System.Drawing.Point(442, 34);
            this.pnlStatusBox.Margin = new System.Windows.Forms.Padding(12, 2, 0, 2);
            this.pnlStatusBox.Name = "pnlStatusBox";
            this.pnlStatusBox.Padding = new System.Windows.Forms.Padding(1, 12, 1, 0);
            this.pnlStatusBox.Size = new System.Drawing.Size(418, 52);
            this.pnlStatusBox.TabIndex = 3;
            // 
            // cmbStatus
            // 
            this.cmbStatus.BackColor = System.Drawing.Color.White;
            this.cmbStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "Requesting",
            "In Processing",
            "Outsourcing",
            "Complete"});
            this.cmbStatus.Location = new System.Drawing.Point(1, 12);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(416, 27);
            this.cmbStatus.TabIndex = 0;
            // 
            // label3
            // 
            this.tlpMain.SetColumnSpan(this.label3, 2);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.label3.Location = new System.Drawing.Point(0, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(2, 0, 0, 4);
            this.label3.Size = new System.Drawing.Size(860, 44);
            this.label3.TabIndex = 4;
            this.label3.Text = "Description";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // pnlDescBox
            // 
            this.pnlDescBox.BackColor = System.Drawing.Color.White;
            this.pnlDescBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.tlpMain.SetColumnSpan(this.pnlDescBox, 2);
            this.pnlDescBox.Controls.Add(this.rtxtbox_description);
            this.pnlDescBox.CornerRadius = 8;
            this.pnlDescBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pnlDescBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDescBox.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.pnlDescBox.Location = new System.Drawing.Point(0, 134);
            this.pnlDescBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.pnlDescBox.Name = "pnlDescBox";
            this.pnlDescBox.Padding = new System.Windows.Forms.Padding(14, 12, 14, 12);
            this.pnlDescBox.Size = new System.Drawing.Size(860, 308);
            this.pnlDescBox.TabIndex = 5;
            // 
            // rtxtbox_description
            // 
            this.rtxtbox_description.BackColor = System.Drawing.Color.White;
            this.rtxtbox_description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtbox_description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtbox_description.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtbox_description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.rtxtbox_description.HintText = "Provide details about the request or issue...";
            this.rtxtbox_description.Location = new System.Drawing.Point(14, 12);
            this.rtxtbox_description.Name = "rtxtbox_description";
            this.rtxtbox_description.Size = new System.Drawing.Size(832, 284);
            this.rtxtbox_description.TabIndex = 0;
            this.rtxtbox_description.Text = "";
            // 
            // pnlButtons
            // 
            this.tlpMain.SetColumnSpan(this.pnlButtons, 2);
            this.pnlButtons.Controls.Add(this.btnSubmit);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(0, 442);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 24, 0, 12);
            this.pnlButtons.Size = new System.Drawing.Size(860, 78);
            this.pnlButtons.TabIndex = 6;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.White;
            this.btnSubmit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.btnSubmit.BorderSize = 1;
            this.btnSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.btnSubmit.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnSubmit.Location = new System.Drawing.Point(0, 24);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.btnSubmit.Size = new System.Drawing.Size(125, 42);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // pnlStatusBar
            // 
            this.pnlStatusBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlStatusBar.Controls.Add(this.lblReady);
            this.pnlStatusBar.Controls.Add(this.lblTicketInfo);
            this.pnlStatusBar.Controls.Add(this.lblStatusDot);
            this.pnlStatusBar.Controls.Add(this.pnlStatusBorder);
            this.pnlStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatusBar.Location = new System.Drawing.Point(0, 566);
            this.pnlStatusBar.Name = "pnlStatusBar";
            this.pnlStatusBar.Size = new System.Drawing.Size(960, 34);
            this.pnlStatusBar.TabIndex = 1;
            // 
            // lblReady
            // 
            this.lblReady.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblReady.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblReady.Location = new System.Drawing.Point(780, 1);
            this.lblReady.Name = "lblReady";
            this.lblReady.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblReady.Size = new System.Drawing.Size(180, 33);
            this.lblReady.TabIndex = 3;
            this.lblReady.Text = "Ready";
            this.lblReady.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTicketInfo
            // 
            this.lblTicketInfo.AutoSize = true;
            this.lblTicketInfo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTicketInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTicketInfo.Location = new System.Drawing.Point(34, 10);
            this.lblTicketInfo.Name = "lblTicketInfo";
            this.lblTicketInfo.Size = new System.Drawing.Size(63, 13);
            this.lblTicketInfo.TabIndex = 2;
            this.lblTicketInfo.Text = "New Ticket";
            // 
            // lblStatusDot
            // 
            this.lblStatusDot.AutoSize = true;
            this.lblStatusDot.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusDot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblStatusDot.Location = new System.Drawing.Point(18, 10);
            this.lblStatusDot.Name = "lblStatusDot";
            this.lblStatusDot.Size = new System.Drawing.Size(14, 13);
            this.lblStatusDot.TabIndex = 1;
            this.lblStatusDot.Text = "●";
            // 
            // pnlStatusBorder
            // 
            this.pnlStatusBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.pnlStatusBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatusBorder.Location = new System.Drawing.Point(0, 0);
            this.pnlStatusBorder.Name = "pnlStatusBorder";
            this.pnlStatusBorder.Size = new System.Drawing.Size(960, 1);
            this.pnlStatusBorder.TabIndex = 0;
            // 
            // CreateTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 600);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlStatusBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(720, 520);
            this.Name = "CreateTicket";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateTicket";
            this.Load += new System.EventHandler(this.CreateTicket_Load);
            this.pnlContent.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.pnlTitleBox.ResumeLayout(false);
            this.pnlTitleBox.PerformLayout();
            this.pnlStatusBox.ResumeLayout(false);
            this.pnlDescBox.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlStatusBar.ResumeLayout(false);
            this.pnlStatusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private tempProj.RoundedPanel pnlTitleBox;
        private tempProj.HintTextBox txtbox_title;
        private tempProj.RoundedPanel pnlStatusBox;
        private tempProj.ModernComboBox cmbStatus;
        private System.Windows.Forms.Label label3;
        private tempProj.RoundedPanel pnlDescBox;
        private tempProj.HintRichTextBox rtxtbox_description;
        private System.Windows.Forms.Panel pnlButtons;
        private tempProj.RoundedButton btnSubmit;
        private System.Windows.Forms.Panel pnlStatusBar;
        private System.Windows.Forms.Label lblReady;
        private System.Windows.Forms.Label lblTicketInfo;
        private System.Windows.Forms.Label lblStatusDot;
        private System.Windows.Forms.Panel pnlStatusBorder;
    }
}