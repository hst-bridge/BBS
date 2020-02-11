namespace BudFileTransfer
{
    partial class BudFileTransfer
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BudFileTransfer));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblMonitorSList = new System.Windows.Forms.Label();
            this.dgrdMain = new System.Windows.Forms.DataGridView();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThreadTimer = new System.Windows.Forms.Timer(this.components);
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMonitorServerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBackupGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStart = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colStop = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMain)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblMonitorSList);
            this.panel3.Controls.Add(this.dgrdMain);
            this.panel3.Location = new System.Drawing.Point(31, 27);
            this.panel3.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(728, 439);
            this.panel3.TabIndex = 3;
            // 
            // lblMonitorSList
            // 
            this.lblMonitorSList.AutoSize = true;
            this.lblMonitorSList.Location = new System.Drawing.Point(41, 17);
            this.lblMonitorSList.Name = "lblMonitorSList";
            this.lblMonitorSList.Size = new System.Drawing.Size(149, 12);
            this.lblMonitorSList.TabIndex = 1;
            this.lblMonitorSList.Text = "【バックアップ元一覧】";
            // 
            // dgrdMain
            // 
            this.dgrdMain.AllowUserToDeleteRows = false;
            this.dgrdMain.AllowUserToResizeRows = false;
            this.dgrdMain.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgrdMain.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgrdMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrdMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colNo,
            this.colMonitorServerName,
            this.colBackupGroup,
            this.colStatus,
            this.colStart,
            this.colStop});
            this.dgrdMain.Location = new System.Drawing.Point(43, 39);
            this.dgrdMain.Name = "dgrdMain";
            this.dgrdMain.ReadOnly = true;
            this.dgrdMain.RowHeadersVisible = false;
            this.dgrdMain.RowTemplate.Height = 23;
            this.dgrdMain.Size = new System.Drawing.Size(634, 328);
            this.dgrdMain.TabIndex = 0;
            this.dgrdMain.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrdMain_CellContentClick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "BudFileTransfer(転送)";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開くToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(95, 48);
            // 
            // 開くToolStripMenuItem
            // 
            this.開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            this.開くToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.開くToolStripMenuItem.Text = "開く";
            this.開くToolStripMenuItem.Click += new System.EventHandler(this.開くToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // ThreadTimer
            // 
            this.ThreadTimer.Interval = 5000;
            this.ThreadTimer.Tick += new System.EventHandler(this.ThreadTimer_Tick);
            // 
            // colId
            // 
            this.colId.HeaderText = "colId";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colId.Visible = false;
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No.";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colMonitorServerName
            // 
            this.colMonitorServerName.HeaderText = "バックアップ元名称";
            this.colMonitorServerName.Name = "colMonitorServerName";
            this.colMonitorServerName.ReadOnly = true;
            this.colMonitorServerName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMonitorServerName.Width = 150;
            // 
            // colBackupGroup
            // 
            this.colBackupGroup.HeaderText = "バックアップ先　　対象グループ";
            this.colBackupGroup.Name = "colBackupGroup";
            this.colBackupGroup.ReadOnly = true;
            this.colBackupGroup.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colBackupGroup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colBackupGroup.Width = 150;
            // 
            // colStatus
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Red;
            this.colStatus.DefaultCellStyle = dataGridViewCellStyle1;
            this.colStatus.HeaderText = "状況";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colStatus.Width = 90;
            // 
            // colStart
            // 
            this.colStart.HeaderText = "始める";
            this.colStart.Name = "colStart";
            this.colStart.ReadOnly = true;
            this.colStart.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colStart.Text = "始める";
            // 
            // colStop
            // 
            this.colStop.HeaderText = "停止";
            this.colStop.Name = "colStop";
            this.colStop.ReadOnly = true;
            this.colStop.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colStop.Text = "停止";
            // 
            // BudFileTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 500);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BudFileTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BudFileTransfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BudFileTransfer_FormClosing);
            this.Load += new System.EventHandler(this.BudFileTransfer_Load);
            this.SizeChanged += new System.EventHandler(this.BudFileTransfer_SizeChanged);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdMain)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblMonitorSList;
        private System.Windows.Forms.DataGridView dgrdMain;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開くToolStripMenuItem;
        private System.Windows.Forms.Timer ThreadTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMonitorServerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBackupGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewButtonColumn colStart;
        private System.Windows.Forms.DataGridViewButtonColumn colStop;
    }
}

