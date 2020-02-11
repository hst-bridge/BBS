namespace BudBackupSystem
{
    partial class FrmGroupDetail
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
            this.lblBKServerGroup = new System.Windows.Forms.Label();
            this.cobBKServerGroup = new System.Windows.Forms.ComboBox();
            this.dgrdMonitorServer = new System.Windows.Forms.DataGridView();
            this.dgrdBackupServer = new System.Windows.Forms.DataGridView();
            this.btnGroupDetailSave = new System.Windows.Forms.Button();
            this.btnGroupDetailList = new System.Windows.Forms.Button();
            this.btnGroupDetailClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.colGroupDetailId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroupDetailName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colServerGroupId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colServerGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMonitorServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBackupServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBKServerGroup
            // 
            this.lblBKServerGroup.AutoSize = true;
            this.lblBKServerGroup.Location = new System.Drawing.Point(20, 34);
            this.lblBKServerGroup.Name = "lblBKServerGroup";
            this.lblBKServerGroup.Size = new System.Drawing.Size(113, 12);
            this.lblBKServerGroup.TabIndex = 0;
            this.lblBKServerGroup.Text = "転送先グループ名称";
            // 
            // cobBKServerGroup
            // 
            this.cobBKServerGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobBKServerGroup.FormattingEnabled = true;
            this.cobBKServerGroup.Location = new System.Drawing.Point(176, 31);
            this.cobBKServerGroup.Name = "cobBKServerGroup";
            this.cobBKServerGroup.Size = new System.Drawing.Size(136, 20);
            this.cobBKServerGroup.TabIndex = 1;
            this.cobBKServerGroup.SelectedIndexChanged += new System.EventHandler(this.cobBKServerGroup_SelectedIndexChanged);
            // 
            // dgrdMonitorServer
            // 
            this.dgrdMonitorServer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrdMonitorServer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGroupDetailId,
            this.colGroupDetailName});
            this.dgrdMonitorServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdMonitorServer.Location = new System.Drawing.Point(13, 96);
            this.dgrdMonitorServer.Name = "dgrdMonitorServer";
            this.dgrdMonitorServer.RowHeadersVisible = false;
            this.dgrdMonitorServer.RowTemplate.Height = 23;
            this.dgrdMonitorServer.Size = new System.Drawing.Size(243, 265);
            this.dgrdMonitorServer.TabIndex = 2;
            this.dgrdMonitorServer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdMonitorServer_CellContentClick);
            this.dgrdMonitorServer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdMonitorServer_CellDoubleClick);
            // 
            // dgrdBackupServer
            // 
            this.dgrdBackupServer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrdBackupServer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colServerGroupId,
            this.colServerGroupName});
            this.dgrdBackupServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgrdBackupServer.Location = new System.Drawing.Point(453, 100);
            this.dgrdBackupServer.Name = "dgrdBackupServer";
            this.dgrdBackupServer.RowHeadersVisible = false;
            this.dgrdBackupServer.RowTemplate.Height = 23;
            this.dgrdBackupServer.Size = new System.Drawing.Size(243, 265);
            this.dgrdBackupServer.TabIndex = 3;
            this.dgrdBackupServer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdBackupServer_CellDoubleClick);
            // 
            // btnGroupDetailSave
            // 
            this.btnGroupDetailSave.Location = new System.Drawing.Point(137, 384);
            this.btnGroupDetailSave.Name = "btnGroupDetailSave";
            this.btnGroupDetailSave.Size = new System.Drawing.Size(119, 23);
            this.btnGroupDetailSave.TabIndex = 6;
            this.btnGroupDetailSave.Text = "保存";
            this.btnGroupDetailSave.UseVisualStyleBackColor = true;
            this.btnGroupDetailSave.Click += new System.EventHandler(this.btnGroupDetailSave_Click);
            // 
            // btnGroupDetailList
            // 
            this.btnGroupDetailList.Location = new System.Drawing.Point(500, 385);
            this.btnGroupDetailList.Name = "btnGroupDetailList";
            this.btnGroupDetailList.Size = new System.Drawing.Size(196, 23);
            this.btnGroupDetailList.TabIndex = 7;
            this.btnGroupDetailList.Text = "転送先サーバー一覧へ";
            this.btnGroupDetailList.UseVisualStyleBackColor = true;
            this.btnGroupDetailList.Visible = false;
            this.btnGroupDetailList.Click += new System.EventHandler(this.btnGroupDetailList_Click);
            // 
            // btnGroupDetailClose
            // 
            this.btnGroupDetailClose.Location = new System.Drawing.Point(13, 384);
            this.btnGroupDetailClose.Name = "btnGroupDetailClose";
            this.btnGroupDetailClose.Size = new System.Drawing.Size(119, 23);
            this.btnGroupDetailClose.TabIndex = 8;
            this.btnGroupDetailClose.Text = "キャンセル";
            this.btnGroupDetailClose.UseVisualStyleBackColor = true;
            this.btnGroupDetailClose.Click += new System.EventHandler(this.btnGroupDetailClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BudBackupSystem.Properties.Resources.addBtn;
            this.pictureBox1.Location = new System.Drawing.Point(288, 161);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(129, 44);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.btnLeft_Click);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::BudBackupSystem.Properties.Resources.removeBtn;
            this.pictureBox2.Location = new System.Drawing.Point(288, 250);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(129, 44);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.btnRight_Click);
            this.pictureBox2.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            // 
            // colGroupDetailId
            // 
            this.colGroupDetailId.HeaderText = "colGroupDetailId";
            this.colGroupDetailId.Name = "colGroupDetailId";
            this.colGroupDetailId.Visible = false;
            // 
            // colGroupDetailName
            // 
            this.colGroupDetailName.HeaderText = "転送先（対象）";
            this.colGroupDetailName.Name = "colGroupDetailName";
            this.colGroupDetailName.Width = 240;
            // 
            // colServerGroupId
            // 
            this.colServerGroupId.HeaderText = "colServerGroupId";
            this.colServerGroupId.Name = "colServerGroupId";
            this.colServerGroupId.Visible = false;
            // 
            // colServerGroupName
            // 
            this.colServerGroupName.HeaderText = "転送先（全て）";
            this.colServerGroupName.Name = "colServerGroupName";
            this.colServerGroupName.Width = 240;
            // 
            // FrmGroupDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(715, 420);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnGroupDetailClose);
            this.Controls.Add(this.btnGroupDetailList);
            this.Controls.Add(this.btnGroupDetailSave);
            this.Controls.Add(this.dgrdBackupServer);
            this.Controls.Add(this.dgrdMonitorServer);
            this.Controls.Add(this.cobBKServerGroup);
            this.Controls.Add(this.lblBKServerGroup);
            this.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.MaximizeBox = false;
            this.Name = "FrmGroupDetail";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmGroupDetail_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmGroupDetail_FormClosed);
            this.Load += new System.EventHandler(this.FrmGroupDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMonitorServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdBackupServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBKServerGroup;
        private System.Windows.Forms.ComboBox cobBKServerGroup;
        private System.Windows.Forms.DataGridView dgrdMonitorServer;
        private System.Windows.Forms.DataGridView dgrdBackupServer;
        private System.Windows.Forms.Button btnGroupDetailSave;
        private System.Windows.Forms.Button btnGroupDetailList;
        private System.Windows.Forms.Button btnGroupDetailClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupDetailId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupDetailName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colServerGroupId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colServerGroupName;
    }
}