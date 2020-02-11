namespace BudBackupCopy
{
    partial class BudBackupCopy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BudBackupCopy));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.labelObject = new System.Windows.Forms.Label();
            this.comboBoxObject = new System.Windows.Forms.ComboBox();
            this.labelLocalPath = new System.Windows.Forms.Label();
            this.textBoxLocalPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.labelUserID = new System.Windows.Forms.Label();
            this.textBoxUserID = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.buttonCopyStart = new System.Windows.Forms.Button();
            this.labelJobPath = new System.Windows.Forms.Label();
            this.textBoxJobPath = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.timer1 = new System.Timers.Timer();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOpen,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(141, 76);
            // 
            // toolStripMenuItemOpen
            // 
            this.toolStripMenuItemOpen.Name = "toolStripMenuItemOpen";
            this.toolStripMenuItemOpen.Size = new System.Drawing.Size(140, 36);
            this.toolStripMenuItemOpen.Text = "開く";
            this.toolStripMenuItemOpen.Click += new System.EventHandler(this.toolStripMenuItemOpen_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(140, 36);
            this.toolStripMenuItemClose.Text = "退出";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // labelObject
            // 
            this.labelObject.AutoSize = true;
            this.labelObject.Location = new System.Drawing.Point(96, 82);
            this.labelObject.Name = "labelObject";
            this.labelObject.Size = new System.Drawing.Size(117, 25);
            this.labelObject.TabIndex = 1;
            this.labelObject.Text = "復元対象:";
            // 
            // comboBoxObject
            // 
            this.comboBoxObject.FormattingEnabled = true;
            this.comboBoxObject.Location = new System.Drawing.Point(244, 78);
            this.comboBoxObject.Name = "comboBoxObject";
            this.comboBoxObject.Size = new System.Drawing.Size(376, 32);
            this.comboBoxObject.TabIndex = 2;
            this.comboBoxObject.SelectedIndexChanged += new System.EventHandler(this.comboBoxObject_SelectedIndexChanged);
            // 
            // labelLocalPath
            // 
            this.labelLocalPath.AutoSize = true;
            this.labelLocalPath.Location = new System.Drawing.Point(79, 193);
            this.labelLocalPath.Name = "labelLocalPath";
            this.labelLocalPath.Size = new System.Drawing.Size(133, 25);
            this.labelLocalPath.TabIndex = 3;
            this.labelLocalPath.Text = "ローカルパス:";
            // 
            // textBoxLocalPath
            // 
            this.textBoxLocalPath.Enabled = false;
            this.textBoxLocalPath.Location = new System.Drawing.Point(244, 190);
            this.textBoxLocalPath.Name = "textBoxLocalPath";
            this.textBoxLocalPath.Size = new System.Drawing.Size(622, 31);
            this.textBoxLocalPath.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(92, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 33);
            this.label1.TabIndex = 5;
            this.label1.Text = "復元先";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(92, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 33);
            this.label2.TabIndex = 6;
            this.label2.Text = "復元元";
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(100, 310);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(113, 25);
            this.labelIP.TabIndex = 7;
            this.labelIP.Text = "IPアドレス:";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(244, 307);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(348, 31);
            this.textBoxIP.TabIndex = 8;
            // 
            // labelUserID
            // 
            this.labelUserID.AutoSize = true;
            this.labelUserID.Location = new System.Drawing.Point(93, 360);
            this.labelUserID.Name = "labelUserID";
            this.labelUserID.Size = new System.Drawing.Size(120, 25);
            this.labelUserID.TabIndex = 9;
            this.labelUserID.Text = "ユーザーID:";
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.Location = new System.Drawing.Point(244, 360);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(348, 31);
            this.textBoxUserID.TabIndex = 10;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(99, 418);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(114, 25);
            this.labelPassword.TabIndex = 11;
            this.labelPassword.Text = "パスワード:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(244, 418);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(348, 31);
            this.textBoxPassword.TabIndex = 12;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(106, 481);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 25);
            this.label3.TabIndex = 13;
            this.label3.Text = "保存パス:";
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Location = new System.Drawing.Point(244, 478);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.Size = new System.Drawing.Size(527, 31);
            this.textBoxSavePath.TabIndex = 14;
            // 
            // buttonCopyStart
            // 
            this.buttonCopyStart.Enabled = false;
            this.buttonCopyStart.Font = new System.Drawing.Font("MS UI Gothic", 24.12566F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCopyStart.Location = new System.Drawing.Point(836, 397);
            this.buttonCopyStart.Name = "buttonCopyStart";
            this.buttonCopyStart.Size = new System.Drawing.Size(241, 112);
            this.buttonCopyStart.TabIndex = 15;
            this.buttonCopyStart.Text = "復元";
            this.buttonCopyStart.UseVisualStyleBackColor = true;
            this.buttonCopyStart.Click += new System.EventHandler(this.buttonCopyStart_Click);
            // 
            // labelJobPath
            // 
            this.labelJobPath.AutoSize = true;
            this.labelJobPath.Location = new System.Drawing.Point(26, 137);
            this.labelJobPath.Name = "labelJobPath";
            this.labelJobPath.Size = new System.Drawing.Size(186, 25);
            this.labelJobPath.TabIndex = 16;
            this.labelJobPath.Text = "バックアップ元パス:";
            // 
            // textBoxJobPath
            // 
            this.textBoxJobPath.Enabled = false;
            this.textBoxJobPath.Location = new System.Drawing.Point(244, 134);
            this.textBoxJobPath.Name = "textBoxJobPath";
            this.textBoxJobPath.Size = new System.Drawing.Size(622, 31);
            this.textBoxJobPath.TabIndex = 17;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(614, 412);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(157, 43);
            this.buttonTest.TabIndex = 18;
            this.buttonTest.Text = "接続テスト";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.SynchronizingObject = this;
            // 
            // BudBackupCopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 560);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.textBoxJobPath);
            this.Controls.Add(this.labelJobPath);
            this.Controls.Add(this.buttonCopyStart);
            this.Controls.Add(this.textBoxSavePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxUserID);
            this.Controls.Add(this.labelUserID);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLocalPath);
            this.Controls.Add(this.labelLocalPath);
            this.Controls.Add(this.comboBoxObject);
            this.Controls.Add(this.labelObject);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BudBackupCopy";
            this.Text = "BudBackupCopy";
            this.Load += new System.EventHandler(this.BudBackupCopy_Load);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClose;
        private System.Windows.Forms.Label labelObject;
        private System.Windows.Forms.ComboBox comboBoxObject;
        private System.Windows.Forms.Label labelLocalPath;
        private System.Windows.Forms.TextBox textBoxLocalPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label labelUserID;
        private System.Windows.Forms.TextBox textBoxUserID;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Button buttonCopyStart;
        private System.Windows.Forms.Label labelJobPath;
        private System.Windows.Forms.TextBox textBoxJobPath;
        private System.Windows.Forms.Button buttonTest;
        private System.Timers.Timer timer1;
    }
}