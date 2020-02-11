namespace BudBackupLogSee
{
    partial class BudBackupLogSee
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BudBackupLogSee));
            this.dateTimePickerLog = new System.Windows.Forms.DateTimePicker();
            this.labelDate = new System.Windows.Forms.Label();
            this.buttonSerach = new System.Windows.Forms.Button();
            this.groupBoxSerachObject = new System.Windows.Forms.GroupBox();
            this.listBoxObject = new System.Windows.Forms.ListBox();
            this.groupBoxLogForCopy = new System.Windows.Forms.GroupBox();
            this.tabControlLog = new System.Windows.Forms.TabControl();
            this.tabPageAll = new System.Windows.Forms.TabPage();
            this.richTextBoxLogAll = new System.Windows.Forms.RichTextBox();
            this.radioButtonCopy = new System.Windows.Forms.RadioButton();
            this.radioButtonTransfer = new System.Windows.Forms.RadioButton();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.labelLogPattorn = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.logtimer = new System.Timers.Timer();
            this.groupBoxSerachObject.SuspendLayout();
            this.groupBoxLogForCopy.SuspendLayout();
            this.tabControlLog.SuspendLayout();
            this.tabPageAll.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logtimer)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePickerLog
            // 
            this.dateTimePickerLog.Location = new System.Drawing.Point(56, 22);
            this.dateTimePickerLog.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.dateTimePickerLog.Name = "dateTimePickerLog";
            this.dateTimePickerLog.Size = new System.Drawing.Size(120, 19);
            this.dateTimePickerLog.TabIndex = 0;
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(15, 23);
            this.labelDate.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(31, 12);
            this.labelDate.TabIndex = 1;
            this.labelDate.Text = "日付:";
            // 
            // buttonSerach
            // 
            this.buttonSerach.Location = new System.Drawing.Point(458, 18);
            this.buttonSerach.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.buttonSerach.Name = "buttonSerach";
            this.buttonSerach.Size = new System.Drawing.Size(67, 22);
            this.buttonSerach.TabIndex = 3;
            this.buttonSerach.Text = "検索";
            this.buttonSerach.UseVisualStyleBackColor = true;
            this.buttonSerach.Click += new System.EventHandler(this.buttonSerach_Click);
            // 
            // groupBoxSerachObject
            // 
            this.groupBoxSerachObject.Controls.Add(this.listBoxObject);
            this.groupBoxSerachObject.Location = new System.Drawing.Point(13, 62);
            this.groupBoxSerachObject.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxSerachObject.Name = "groupBoxSerachObject";
            this.groupBoxSerachObject.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxSerachObject.Size = new System.Drawing.Size(217, 338);
            this.groupBoxSerachObject.TabIndex = 4;
            this.groupBoxSerachObject.TabStop = false;
            this.groupBoxSerachObject.Text = "検索結果";
            // 
            // listBoxObject
            // 
            this.listBoxObject.FormattingEnabled = true;
            this.listBoxObject.ItemHeight = 12;
            this.listBoxObject.Location = new System.Drawing.Point(8, 24);
            this.listBoxObject.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.listBoxObject.Name = "listBoxObject";
            this.listBoxObject.Size = new System.Drawing.Size(205, 304);
            this.listBoxObject.TabIndex = 0;
            this.listBoxObject.SelectedIndexChanged += new System.EventHandler(this.listBoxObject_SelectedIndexChanged);
            // 
            // groupBoxLogForCopy
            // 
            this.groupBoxLogForCopy.Controls.Add(this.tabControlLog);
            this.groupBoxLogForCopy.Location = new System.Drawing.Point(233, 62);
            this.groupBoxLogForCopy.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxLogForCopy.Name = "groupBoxLogForCopy";
            this.groupBoxLogForCopy.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxLogForCopy.Size = new System.Drawing.Size(659, 338);
            this.groupBoxLogForCopy.TabIndex = 5;
            this.groupBoxLogForCopy.TabStop = false;
            this.groupBoxLogForCopy.Text = "関連のバックアップログ";
            // 
            // tabControlLog
            // 
            this.tabControlLog.Controls.Add(this.tabPageAll);
            this.tabControlLog.Location = new System.Drawing.Point(8, 15);
            this.tabControlLog.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabControlLog.Name = "tabControlLog";
            this.tabControlLog.SelectedIndex = 0;
            this.tabControlLog.Size = new System.Drawing.Size(643, 313);
            this.tabControlLog.TabIndex = 0;
            // 
            // tabPageAll
            // 
            this.tabPageAll.Controls.Add(this.richTextBoxLogAll);
            this.tabPageAll.Location = new System.Drawing.Point(4, 22);
            this.tabPageAll.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPageAll.Name = "tabPageAll";
            this.tabPageAll.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPageAll.Size = new System.Drawing.Size(635, 287);
            this.tabPageAll.TabIndex = 0;
            this.tabPageAll.Text = "ログ全体";
            this.tabPageAll.UseVisualStyleBackColor = true;
            // 
            // richTextBoxLogAll
            // 
            this.richTextBoxLogAll.Location = new System.Drawing.Point(3, 10);
            this.richTextBoxLogAll.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.richTextBoxLogAll.Name = "richTextBoxLogAll";
            this.richTextBoxLogAll.Size = new System.Drawing.Size(630, 278);
            this.richTextBoxLogAll.TabIndex = 0;
            this.richTextBoxLogAll.Text = "";
            this.richTextBoxLogAll.WordWrap = false;
            // 
            // radioButtonCopy
            // 
            this.radioButtonCopy.AutoSize = true;
            this.radioButtonCopy.Checked = true;
            this.radioButtonCopy.Location = new System.Drawing.Point(276, 22);
            this.radioButtonCopy.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButtonCopy.Name = "radioButtonCopy";
            this.radioButtonCopy.Size = new System.Drawing.Size(68, 16);
            this.radioButtonCopy.TabIndex = 6;
            this.radioButtonCopy.TabStop = true;
            this.radioButtonCopy.Text = "コピーログ";
            this.radioButtonCopy.UseVisualStyleBackColor = true;
            // 
            // radioButtonTransfer
            // 
            this.radioButtonTransfer.AutoSize = true;
            this.radioButtonTransfer.Location = new System.Drawing.Point(356, 22);
            this.radioButtonTransfer.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButtonTransfer.Name = "radioButtonTransfer";
            this.radioButtonTransfer.Size = new System.Drawing.Size(65, 16);
            this.radioButtonTransfer.TabIndex = 7;
            this.radioButtonTransfer.Text = "転送ログ";
            this.radioButtonTransfer.UseVisualStyleBackColor = true;
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.labelLogPattorn);
            this.groupBoxSearch.Controls.Add(this.dateTimePickerLog);
            this.groupBoxSearch.Controls.Add(this.radioButtonTransfer);
            this.groupBoxSearch.Controls.Add(this.radioButtonCopy);
            this.groupBoxSearch.Controls.Add(this.buttonSerach);
            this.groupBoxSearch.Controls.Add(this.labelDate);
            this.groupBoxSearch.Location = new System.Drawing.Point(13, 12);
            this.groupBoxSearch.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBoxSearch.Size = new System.Drawing.Size(589, 46);
            this.groupBoxSearch.TabIndex = 8;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "ログ検索";
            // 
            // labelLogPattorn
            // 
            this.labelLogPattorn.AutoSize = true;
            this.labelLogPattorn.Location = new System.Drawing.Point(217, 23);
            this.labelLogPattorn.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.labelLogPattorn.Name = "labelLogPattorn";
            this.labelLogPattorn.Size = new System.Drawing.Size(49, 12);
            this.labelLogPattorn.TabIndex = 8;
            this.labelLogPattorn.Text = "ログ選択:";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "ログ";
            this.notifyIcon.Visible = true;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOpen,
            this.toolStripMenuItemClose});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(99, 48);
            // 
            // toolStripMenuItemOpen
            // 
            this.toolStripMenuItemOpen.Name = "toolStripMenuItemOpen";
            this.toolStripMenuItemOpen.Size = new System.Drawing.Size(98, 22);
            this.toolStripMenuItemOpen.Text = "開く";
            this.toolStripMenuItemOpen.Click += new System.EventHandler(this.toolStripMenuItemOpen_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(98, 22);
            this.toolStripMenuItemClose.Text = "退出";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // logtimer
            // 
            this.logtimer.Enabled = true;
            this.logtimer.SynchronizingObject = this;
            // 
            // BudBackupLogSee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 406);
            this.Controls.Add(this.groupBoxSearch);
            this.Controls.Add(this.groupBoxSerachObject);
            this.Controls.Add(this.groupBoxLogForCopy);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Name = "BudBackupLogSee";
            this.Text = "BudBackupLogSee";
            this.Load += new System.EventHandler(this.BudBackupLogSee_Load);
            this.groupBoxSerachObject.ResumeLayout(false);
            this.groupBoxLogForCopy.ResumeLayout(false);
            this.tabControlLog.ResumeLayout(false);
            this.tabPageAll.ResumeLayout(false);
            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logtimer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerLog;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Button buttonSerach;
        private System.Windows.Forms.GroupBox groupBoxSerachObject;
        private System.Windows.Forms.GroupBox groupBoxLogForCopy;
        private System.Windows.Forms.ListBox listBoxObject;
        private System.Windows.Forms.RadioButton radioButtonCopy;
        private System.Windows.Forms.RadioButton radioButtonTransfer;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.Label labelLogPattorn;
        private System.Windows.Forms.TabControl tabControlLog;
        private System.Windows.Forms.TabPage tabPageAll;
        private System.Windows.Forms.RichTextBox richTextBoxLogAll;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClose;
        private System.Timers.Timer logtimer;
    }
}