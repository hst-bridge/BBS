namespace BudBackupSystem
{
    partial class FrmTransfer
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
            this.tctransfer = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cobMonitorServer = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtssbpath = new System.Windows.Forms.TextBox();
            this.labelSSB = new System.Windows.Forms.Label();
            this.btnReference = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBackupServerGroupName = new System.Windows.Forms.TextBox();
            this.txtTrId = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLinkTest = new System.Windows.Forms.Button();
            this.lblTrStfile = new System.Windows.Forms.Label();
            this.lblTrPass = new System.Windows.Forms.Label();
            this.lblTrAccount = new System.Windows.Forms.Label();
            this.lblTrMemo = new System.Windows.Forms.Label();
            this.lblTrIp = new System.Windows.Forms.Label();
            this.txtTrStfile = new System.Windows.Forms.TextBox();
            this.txtTrPass = new System.Windows.Forms.TextBox();
            this.txtTraccount = new System.Windows.Forms.TextBox();
            this.txtTrMemo = new System.Windows.Forms.TextBox();
            this.txtTrIp = new System.Windows.Forms.TextBox();
            this.txtTrName = new System.Windows.Forms.TextBox();
            this.lblMonitorServer = new System.Windows.Forms.Label();
            this.lblTrName = new System.Windows.Forms.Label();
            this.tctransfer.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tctransfer
            // 
            this.tctransfer.Controls.Add(this.tabPage1);
            this.tctransfer.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.tctransfer.Location = new System.Drawing.Point(30, 35);
            this.tctransfer.Name = "tctransfer";
            this.tctransfer.SelectedIndex = 0;
            this.tctransfer.Size = new System.Drawing.Size(561, 383);
            this.tctransfer.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cobMonitorServer);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.txtssbpath);
            this.tabPage1.Controls.Add(this.labelSSB);
            this.tabPage1.Controls.Add(this.btnReference);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtBackupServerGroupName);
            this.tabPage1.Controls.Add(this.txtTrId);
            this.tabPage1.Controls.Add(this.btnSave);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnLinkTest);
            this.tabPage1.Controls.Add(this.lblTrStfile);
            this.tabPage1.Controls.Add(this.lblTrPass);
            this.tabPage1.Controls.Add(this.lblTrAccount);
            this.tabPage1.Controls.Add(this.lblTrMemo);
            this.tabPage1.Controls.Add(this.lblTrIp);
            this.tabPage1.Controls.Add(this.txtTrStfile);
            this.tabPage1.Controls.Add(this.txtTrPass);
            this.tabPage1.Controls.Add(this.txtTraccount);
            this.tabPage1.Controls.Add(this.txtTrMemo);
            this.tabPage1.Controls.Add(this.txtTrIp);
            this.tabPage1.Controls.Add(this.txtTrName);
            this.tabPage1.Controls.Add(this.lblMonitorServer);
            this.tabPage1.Controls.Add(this.lblTrName);
            this.tabPage1.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(553, 357);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "転送先設定·編集";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cobMonitorServer
            // 
            this.cobMonitorServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobMonitorServer.FormattingEnabled = true;
            this.cobMonitorServer.Location = new System.Drawing.Point(172, 20);
            this.cobMonitorServer.Name = "cobMonitorServer";
            this.cobMonitorServer.Size = new System.Drawing.Size(221, 20);
            this.cobMonitorServer.TabIndex = 16;
            this.cobMonitorServer.SelectedIndexChanged += new System.EventHandler(this.cobMonitorServer_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(416, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "入力例：C:\\Test";
            this.label7.Visible = false;
            // 
            // txtssbpath
            // 
            this.txtssbpath.Location = new System.Drawing.Point(172, 278);
            this.txtssbpath.Name = "txtssbpath";
            this.txtssbpath.Size = new System.Drawing.Size(221, 19);
            this.txtssbpath.TabIndex = 14;
            this.txtssbpath.Visible = false;
            // 
            // labelSSB
            // 
            this.labelSSB.AutoSize = true;
            this.labelSSB.Location = new System.Drawing.Point(52, 280);
            this.labelSSB.Name = "labelSSB";
            this.labelSSB.Size = new System.Drawing.Size(95, 12);
            this.labelSSB.TabIndex = 13;
            this.labelSSB.Text = "SSB転送先のパス";
            this.labelSSB.Visible = false;
            // 
            // btnReference
            // 
            this.btnReference.Location = new System.Drawing.Point(418, 235);
            this.btnReference.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnReference.Name = "btnReference";
            this.btnReference.Size = new System.Drawing.Size(80, 23);
            this.btnReference.TabIndex = 11;
            this.btnReference.Text = "参照";
            this.btnReference.UseVisualStyleBackColor = true;
            this.btnReference.Visible = false;
            this.btnReference.Click += new System.EventHandler(this.btnReference_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(34, 238);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(35, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(34, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(34, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(34, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "*";
            this.label1.Visible = false;
            // 
            // txtBackupServerGroupName
            // 
            this.txtBackupServerGroupName.Location = new System.Drawing.Point(445, 20);
            this.txtBackupServerGroupName.Name = "txtBackupServerGroupName";
            this.txtBackupServerGroupName.Size = new System.Drawing.Size(100, 19);
            this.txtBackupServerGroupName.TabIndex = 10;
            this.txtBackupServerGroupName.Visible = false;
            // 
            // txtTrId
            // 
            this.txtTrId.Location = new System.Drawing.Point(445, 44);
            this.txtTrId.Name = "txtTrId";
            this.txtTrId.Size = new System.Drawing.Size(100, 19);
            this.txtTrId.TabIndex = 10;
            this.txtTrId.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(316, 302);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(161, 302);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLinkTest
            // 
            this.btnLinkTest.Location = new System.Drawing.Point(418, 188);
            this.btnLinkTest.Name = "btnLinkTest";
            this.btnLinkTest.Size = new System.Drawing.Size(80, 23);
            this.btnLinkTest.TabIndex = 7;
            this.btnLinkTest.Text = "接続テスト";
            this.btnLinkTest.UseVisualStyleBackColor = true;
            this.btnLinkTest.Click += new System.EventHandler(this.btnLinkTest_Click);
            // 
            // lblTrStfile
            // 
            this.lblTrStfile.AutoSize = true;
            this.lblTrStfile.Location = new System.Drawing.Point(52, 238);
            this.lblTrStfile.Name = "lblTrStfile";
            this.lblTrStfile.Size = new System.Drawing.Size(77, 12);
            this.lblTrStfile.TabIndex = 6;
            this.lblTrStfile.Text = "開始フォルダ";
            // 
            // lblTrPass
            // 
            this.lblTrPass.AutoSize = true;
            this.lblTrPass.Location = new System.Drawing.Point(52, 192);
            this.lblTrPass.Name = "lblTrPass";
            this.lblTrPass.Size = new System.Drawing.Size(65, 12);
            this.lblTrPass.TabIndex = 5;
            this.lblTrPass.Text = "パスワード";
            // 
            // lblTrAccount
            // 
            this.lblTrAccount.AutoSize = true;
            this.lblTrAccount.Location = new System.Drawing.Point(52, 150);
            this.lblTrAccount.Name = "lblTrAccount";
            this.lblTrAccount.Size = new System.Drawing.Size(101, 12);
            this.lblTrAccount.TabIndex = 4;
            this.lblTrAccount.Text = "接続アカウント名";
            // 
            // lblTrMemo
            // 
            this.lblTrMemo.AutoSize = true;
            this.lblTrMemo.Location = new System.Drawing.Point(52, 109);
            this.lblTrMemo.Name = "lblTrMemo";
            this.lblTrMemo.Size = new System.Drawing.Size(29, 12);
            this.lblTrMemo.TabIndex = 3;
            this.lblTrMemo.Text = "メモ";
            // 
            // lblTrIp
            // 
            this.lblTrIp.AutoSize = true;
            this.lblTrIp.Location = new System.Drawing.Point(52, 71);
            this.lblTrIp.Name = "lblTrIp";
            this.lblTrIp.Size = new System.Drawing.Size(65, 12);
            this.lblTrIp.TabIndex = 2;
            this.lblTrIp.Text = "IPアドレス";
            // 
            // txtTrStfile
            // 
            this.txtTrStfile.Location = new System.Drawing.Point(172, 237);
            this.txtTrStfile.Name = "txtTrStfile";
            this.txtTrStfile.Size = new System.Drawing.Size(221, 19);
            this.txtTrStfile.TabIndex = 6;
            this.txtTrStfile.TextChanged += new System.EventHandler(this.txtTrStfile_TextChanged);
            // 
            // txtTrPass
            // 
            this.txtTrPass.Location = new System.Drawing.Point(172, 190);
            this.txtTrPass.Name = "txtTrPass";
            this.txtTrPass.PasswordChar = '*';
            this.txtTrPass.Size = new System.Drawing.Size(221, 19);
            this.txtTrPass.TabIndex = 5;
            this.txtTrPass.UseSystemPasswordChar = true;
            // 
            // txtTraccount
            // 
            this.txtTraccount.Location = new System.Drawing.Point(172, 149);
            this.txtTraccount.Name = "txtTraccount";
            this.txtTraccount.Size = new System.Drawing.Size(221, 19);
            this.txtTraccount.TabIndex = 4;
            // 
            // txtTrMemo
            // 
            this.txtTrMemo.Location = new System.Drawing.Point(172, 108);
            this.txtTrMemo.Name = "txtTrMemo";
            this.txtTrMemo.Size = new System.Drawing.Size(221, 19);
            this.txtTrMemo.TabIndex = 3;
            // 
            // txtTrIp
            // 
            this.txtTrIp.Location = new System.Drawing.Point(172, 69);
            this.txtTrIp.Name = "txtTrIp";
            this.txtTrIp.Size = new System.Drawing.Size(221, 19);
            this.txtTrIp.TabIndex = 2;
            this.txtTrIp.Leave += new System.EventHandler(this.txtTrIp_Leave);
            // 
            // txtTrName
            // 
            this.txtTrName.Location = new System.Drawing.Point(172, 44);
            this.txtTrName.Name = "txtTrName";
            this.txtTrName.Size = new System.Drawing.Size(221, 19);
            this.txtTrName.TabIndex = 1;
            this.txtTrName.Visible = false;
            // 
            // lblMonitorServer
            // 
            this.lblMonitorServer.AutoSize = true;
            this.lblMonitorServer.Location = new System.Drawing.Point(52, 23);
            this.lblMonitorServer.Name = "lblMonitorServer";
            this.lblMonitorServer.Size = new System.Drawing.Size(41, 12);
            this.lblMonitorServer.TabIndex = 0;
            this.lblMonitorServer.Text = "転送元";
            // 
            // lblTrName
            // 
            this.lblTrName.AutoSize = true;
            this.lblTrName.Location = new System.Drawing.Point(52, 47);
            this.lblTrName.Name = "lblTrName";
            this.lblTrName.Size = new System.Drawing.Size(65, 12);
            this.lblTrName.TabIndex = 0;
            this.lblTrName.Text = "転送先名称";
            this.lblTrName.Visible = false;
            // 
            // FrmTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 436);
            this.Controls.Add(this.tctransfer);
            this.MaximizeBox = false;
            this.Name = "FrmTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTransfer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTransfer_FormClosed);
            this.Load += new System.EventHandler(this.FrmTransfer_Load);
            this.tctransfer.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tctransfer;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLinkTest;
        private System.Windows.Forms.Label lblTrStfile;
        private System.Windows.Forms.Label lblTrPass;
        private System.Windows.Forms.Label lblTrAccount;
        private System.Windows.Forms.Label lblTrMemo;
        private System.Windows.Forms.Label lblTrIp;
        private System.Windows.Forms.TextBox txtTrStfile;
        private System.Windows.Forms.TextBox txtTrPass;
        private System.Windows.Forms.TextBox txtTraccount;
        private System.Windows.Forms.TextBox txtTrMemo;
        private System.Windows.Forms.TextBox txtTrIp;
        private System.Windows.Forms.TextBox txtTrName;
        private System.Windows.Forms.Label lblTrName;
        private System.Windows.Forms.TextBox txtTrId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReference;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtssbpath;
        private System.Windows.Forms.Label labelSSB;
        private System.Windows.Forms.Label lblMonitorServer;
        private System.Windows.Forms.ComboBox cobMonitorServer;
        private System.Windows.Forms.TextBox txtBackupServerGroupName;
    }
}