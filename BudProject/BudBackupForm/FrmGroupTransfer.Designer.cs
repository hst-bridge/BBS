namespace BudBackupSystem
{
    partial class FrmGroupTransfer
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
            this.tcMonitorObj = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.cobMonitorServer = new System.Windows.Forms.ComboBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtMonitorId = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMonitorMemo = new System.Windows.Forms.Label();
            this.txtMonitorMemo = new System.Windows.Forms.TextBox();
            this.txtMonitorName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.tcMonitorObj.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMonitorObj
            // 
            this.tcMonitorObj.Controls.Add(this.tabPage1);
            this.tcMonitorObj.Location = new System.Drawing.Point(17, 24);
            this.tcMonitorObj.Name = "tcMonitorObj";
            this.tcMonitorObj.SelectedIndex = 0;
            this.tcMonitorObj.Size = new System.Drawing.Size(483, 261);
            this.tcMonitorObj.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cobMonitorServer);
            this.tabPage1.Controls.Add(this.lblServer);
            this.tabPage1.Controls.Add(this.txtMonitorId);
            this.tabPage1.Controls.Add(this.btnSave);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.lblMonitorMemo);
            this.tabPage1.Controls.Add(this.txtMonitorMemo);
            this.tabPage1.Controls.Add(this.txtMonitorName);
            this.tabPage1.Controls.Add(this.lblName);
            this.tabPage1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 235);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "転送先グループ設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(18, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "*";
            // 
            // cobMonitorServer
            // 
            this.cobMonitorServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobMonitorServer.FormattingEnabled = true;
            this.cobMonitorServer.Location = new System.Drawing.Point(185, 115);
            this.cobMonitorServer.Name = "cobMonitorServer";
            this.cobMonitorServer.Size = new System.Drawing.Size(220, 20);
            this.cobMonitorServer.TabIndex = 12;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(35, 118);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 12);
            this.lblServer.TabIndex = 11;
            this.lblServer.Text = "転送元";
            // 
            // txtMonitorId
            // 
            this.txtMonitorId.Location = new System.Drawing.Point(445, 29);
            this.txtMonitorId.Name = "txtMonitorId";
            this.txtMonitorId.Size = new System.Drawing.Size(40, 19);
            this.txtMonitorId.TabIndex = 10;
            this.txtMonitorId.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(273, 167);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(151, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblMonitorMemo
            // 
            this.lblMonitorMemo.AutoSize = true;
            this.lblMonitorMemo.Location = new System.Drawing.Point(35, 73);
            this.lblMonitorMemo.Name = "lblMonitorMemo";
            this.lblMonitorMemo.Size = new System.Drawing.Size(29, 12);
            this.lblMonitorMemo.TabIndex = 3;
            this.lblMonitorMemo.Text = "メモ";
            // 
            // txtMonitorMemo
            // 
            this.txtMonitorMemo.Location = new System.Drawing.Point(185, 70);
            this.txtMonitorMemo.Name = "txtMonitorMemo";
            this.txtMonitorMemo.Size = new System.Drawing.Size(221, 19);
            this.txtMonitorMemo.TabIndex = 2;
            // 
            // txtMonitorName
            // 
            this.txtMonitorName.Location = new System.Drawing.Point(185, 29);
            this.txtMonitorName.Name = "txtMonitorName";
            this.txtMonitorName.Size = new System.Drawing.Size(221, 19);
            this.txtMonitorName.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(35, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(113, 12);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "転送先グループ名称";
            // 
            // FrmGroupTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 325);
            this.Controls.Add(this.tcMonitorObj);
            this.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.MaximizeBox = false;
            this.Name = "FrmGroupTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmGroupTransfer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmGroupTransfer_FormClosed);
            this.Load += new System.EventHandler(this.FrmGroupTransfer_Load);
            this.tcMonitorObj.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcMonitorObj;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMonitorMemo;
        private System.Windows.Forms.TextBox txtMonitorMemo;
        private System.Windows.Forms.TextBox txtMonitorName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtMonitorId;
        private System.Windows.Forms.ComboBox cobMonitorServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label label1;
    }
}