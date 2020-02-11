namespace BudFileListen
{
    partial class BudFileListen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BudFileListen));
            this.tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblMonitorSList = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerForRoboCopy = new System.Timers.Timer();
            this.labelBatchDisplay = new System.Windows.Forms.Label();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerForRoboCopy)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel
            // 
            this.tablePanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tablePanel.ColumnCount = 2;
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.Location = new System.Drawing.Point(34, 48);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.RowCount = 2;
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.Size = new System.Drawing.Size(300, 60);
            this.tablePanel.TabIndex = 0;
            this.tablePanel.Visible = false;
            // 
            // lblMonitorSList
            // 
            this.lblMonitorSList.AutoSize = true;
            this.lblMonitorSList.Location = new System.Drawing.Point(30, 30);
            this.lblMonitorSList.Name = "lblMonitorSList";
            this.lblMonitorSList.Size = new System.Drawing.Size(137, 12);
            this.lblMonitorSList.TabIndex = 2;
            this.lblMonitorSList.Text = "【バックアップ元一覧】";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "BudFileListen(バックアップ)";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.CloseToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(95, 48);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.OpenToolStripMenuItem.Text = "開く";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // CloseToolStripMenuItem
            // 
            this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            this.CloseToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.CloseToolStripMenuItem.Text = "退出";
            this.CloseToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // timerForRoboCopy
            // 
            this.timerForRoboCopy.SynchronizingObject = this;
            // 
            // labelBatchDisplay
            // 
            this.labelBatchDisplay.AutoSize = true;
            this.labelBatchDisplay.Location = new System.Drawing.Point(221, 30);
            this.labelBatchDisplay.Name = "labelBatchDisplay";
            this.labelBatchDisplay.Size = new System.Drawing.Size(0, 12);
            this.labelBatchDisplay.TabIndex = 3;
            // 
            // BudFileListen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 478);
            this.Controls.Add(this.labelBatchDisplay);
            this.Controls.Add(this.lblMonitorSList);
            this.Controls.Add(this.tablePanel);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BudFileListen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BudFileListen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BudFileListen_FormClosing);
            this.Load += new System.EventHandler(this.BudFileListen_Load);
            this.SizeChanged += new System.EventHandler(this.BudFileListen_SizeChanged);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timerForRoboCopy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePanel;
        private System.Windows.Forms.Label lblMonitorSList;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Timers.Timer timerForRoboCopy;
        private System.Windows.Forms.Label labelBatchDisplay;


    }
}

