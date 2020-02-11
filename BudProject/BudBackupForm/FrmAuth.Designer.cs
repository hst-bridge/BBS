namespace BudBackupSystem
{
    partial class FrmAuth
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAuthEditId = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbAuthFlgNo = new System.Windows.Forms.RadioButton();
            this.rbAuthFlgYes = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbAuthMailNo = new System.Windows.Forms.RadioButton();
            this.rbAuthMailYes = new System.Windows.Forms.RadioButton();
            this.btnAuthSave = new System.Windows.Forms.Button();
            this.btnAuthCancel = new System.Windows.Forms.Button();
            this.txtAuEditMail = new System.Windows.Forms.TextBox();
            this.txtAuEditName = new System.Windows.Forms.TextBox();
            this.txtAuEditLoginPass = new System.Windows.Forms.TextBox();
            this.txtAuEditLoginId = new System.Windows.Forms.TextBox();
            this.lblAuEditAuthFlg = new System.Windows.Forms.Label();
            this.lblAuEditMailFlg = new System.Windows.Forms.Label();
            this.lblAuEditMail = new System.Windows.Forms.Label();
            this.lblAuEditName = new System.Windows.Forms.Label();
            this.lblAuEditLoginPass = new System.Windows.Forms.Label();
            this.lblAuEditLoginId = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(23, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(487, 341);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtAuthEditId);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.btnAuthSave);
            this.tabPage1.Controls.Add(this.btnAuthCancel);
            this.tabPage1.Controls.Add(this.txtAuEditMail);
            this.tabPage1.Controls.Add(this.txtAuEditName);
            this.tabPage1.Controls.Add(this.txtAuEditLoginPass);
            this.tabPage1.Controls.Add(this.txtAuEditLoginId);
            this.tabPage1.Controls.Add(this.lblAuEditAuthFlg);
            this.tabPage1.Controls.Add(this.lblAuEditMailFlg);
            this.tabPage1.Controls.Add(this.lblAuEditMail);
            this.tabPage1.Controls.Add(this.lblAuEditName);
            this.tabPage1.Controls.Add(this.lblAuEditLoginPass);
            this.tabPage1.Controls.Add(this.lblAuEditLoginId);
            this.tabPage1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(479, 315);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "権限設定·編集";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(46, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(46, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(46, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(46, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "*";
            // 
            // txtAuthEditId
            // 
            this.txtAuthEditId.Location = new System.Drawing.Point(358, 30);
            this.txtAuthEditId.Name = "txtAuthEditId";
            this.txtAuthEditId.Size = new System.Drawing.Size(63, 19);
            this.txtAuthEditId.TabIndex = 6;
            this.txtAuthEditId.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbAuthFlgNo);
            this.panel2.Controls.Add(this.rbAuthFlgYes);
            this.panel2.Location = new System.Drawing.Point(158, 216);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(231, 28);
            this.panel2.TabIndex = 5;
            // 
            // rbAuthFlgNo
            // 
            this.rbAuthFlgNo.AutoSize = true;
            this.rbAuthFlgNo.Location = new System.Drawing.Point(65, 6);
            this.rbAuthFlgNo.Name = "rbAuthFlgNo";
            this.rbAuthFlgNo.Size = new System.Drawing.Size(35, 16);
            this.rbAuthFlgNo.TabIndex = 8;
            this.rbAuthFlgNo.TabStop = true;
            this.rbAuthFlgNo.Text = "否";
            this.rbAuthFlgNo.UseVisualStyleBackColor = true;
            // 
            // rbAuthFlgYes
            // 
            this.rbAuthFlgYes.AutoSize = true;
            this.rbAuthFlgYes.Location = new System.Drawing.Point(4, 6);
            this.rbAuthFlgYes.Name = "rbAuthFlgYes";
            this.rbAuthFlgYes.Size = new System.Drawing.Size(35, 16);
            this.rbAuthFlgYes.TabIndex = 7;
            this.rbAuthFlgYes.TabStop = true;
            this.rbAuthFlgYes.Text = "是";
            this.rbAuthFlgYes.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbAuthMailNo);
            this.panel1.Controls.Add(this.rbAuthMailYes);
            this.panel1.Location = new System.Drawing.Point(159, 170);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 32);
            this.panel1.TabIndex = 4;
            // 
            // rbAuthMailNo
            // 
            this.rbAuthMailNo.AutoSize = true;
            this.rbAuthMailNo.Location = new System.Drawing.Point(63, 8);
            this.rbAuthMailNo.Name = "rbAuthMailNo";
            this.rbAuthMailNo.Size = new System.Drawing.Size(35, 16);
            this.rbAuthMailNo.TabIndex = 6;
            this.rbAuthMailNo.TabStop = true;
            this.rbAuthMailNo.Text = "否";
            this.rbAuthMailNo.UseVisualStyleBackColor = true;
            // 
            // rbAuthMailYes
            // 
            this.rbAuthMailYes.AutoSize = true;
            this.rbAuthMailYes.Location = new System.Drawing.Point(4, 8);
            this.rbAuthMailYes.Name = "rbAuthMailYes";
            this.rbAuthMailYes.Size = new System.Drawing.Size(35, 16);
            this.rbAuthMailYes.TabIndex = 5;
            this.rbAuthMailYes.TabStop = true;
            this.rbAuthMailYes.Text = "是";
            this.rbAuthMailYes.UseVisualStyleBackColor = true;
            // 
            // btnAuthSave
            // 
            this.btnAuthSave.Location = new System.Drawing.Point(326, 273);
            this.btnAuthSave.Name = "btnAuthSave";
            this.btnAuthSave.Size = new System.Drawing.Size(85, 23);
            this.btnAuthSave.TabIndex = 3;
            this.btnAuthSave.Text = "保存";
            this.btnAuthSave.UseVisualStyleBackColor = true;
            this.btnAuthSave.Click += new System.EventHandler(this.btnAuthSave_Click);
            // 
            // btnAuthCancel
            // 
            this.btnAuthCancel.Location = new System.Drawing.Point(178, 273);
            this.btnAuthCancel.Name = "btnAuthCancel";
            this.btnAuthCancel.Size = new System.Drawing.Size(91, 23);
            this.btnAuthCancel.TabIndex = 3;
            this.btnAuthCancel.Text = "キャンセル";
            this.btnAuthCancel.UseVisualStyleBackColor = true;
            this.btnAuthCancel.Click += new System.EventHandler(this.btnAuthCancel_Click);
            // 
            // txtAuEditMail
            // 
            this.txtAuEditMail.Location = new System.Drawing.Point(159, 132);
            this.txtAuEditMail.Name = "txtAuEditMail";
            this.txtAuEditMail.Size = new System.Drawing.Size(183, 19);
            this.txtAuEditMail.TabIndex = 4;
            this.txtAuEditMail.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtAuEditName
            // 
            this.txtAuEditName.Location = new System.Drawing.Point(159, 96);
            this.txtAuEditName.Name = "txtAuEditName";
            this.txtAuEditName.Size = new System.Drawing.Size(183, 19);
            this.txtAuEditName.TabIndex = 3;
            this.txtAuEditName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtAuEditLoginPass
            // 
            this.txtAuEditLoginPass.Location = new System.Drawing.Point(159, 62);
            this.txtAuEditLoginPass.Name = "txtAuEditLoginPass";
            this.txtAuEditLoginPass.PasswordChar = '*';
            this.txtAuEditLoginPass.Size = new System.Drawing.Size(183, 19);
            this.txtAuEditLoginPass.TabIndex = 2;
            this.txtAuEditLoginPass.UseSystemPasswordChar = true;
            this.txtAuEditLoginPass.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtAuEditLoginId
            // 
            this.txtAuEditLoginId.Location = new System.Drawing.Point(159, 29);
            this.txtAuEditLoginId.Name = "txtAuEditLoginId";
            this.txtAuEditLoginId.Size = new System.Drawing.Size(183, 19);
            this.txtAuEditLoginId.TabIndex = 1;
            this.txtAuEditLoginId.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lblAuEditAuthFlg
            // 
            this.lblAuEditAuthFlg.AutoSize = true;
            this.lblAuEditAuthFlg.Location = new System.Drawing.Point(60, 220);
            this.lblAuEditAuthFlg.Name = "lblAuEditAuthFlg";
            this.lblAuEditAuthFlg.Size = new System.Drawing.Size(53, 12);
            this.lblAuEditAuthFlg.TabIndex = 0;
            this.lblAuEditAuthFlg.Text = "権限設定";
            // 
            // lblAuEditMailFlg
            // 
            this.lblAuEditMailFlg.AutoSize = true;
            this.lblAuEditMailFlg.Location = new System.Drawing.Point(60, 180);
            this.lblAuEditMailFlg.Name = "lblAuEditMailFlg";
            this.lblAuEditMailFlg.Size = new System.Drawing.Size(77, 12);
            this.lblAuEditMailFlg.TabIndex = 0;
            this.lblAuEditMailFlg.Text = "メールフ設定";
            // 
            // lblAuEditMail
            // 
            this.lblAuEditMail.AutoSize = true;
            this.lblAuEditMail.Location = new System.Drawing.Point(60, 135);
            this.lblAuEditMail.Name = "lblAuEditMail";
            this.lblAuEditMail.Size = new System.Drawing.Size(41, 12);
            this.lblAuEditMail.TabIndex = 0;
            this.lblAuEditMail.Text = "メール";
            // 
            // lblAuEditName
            // 
            this.lblAuEditName.AutoSize = true;
            this.lblAuEditName.Location = new System.Drawing.Point(60, 96);
            this.lblAuEditName.Name = "lblAuEditName";
            this.lblAuEditName.Size = new System.Drawing.Size(29, 12);
            this.lblAuEditName.TabIndex = 0;
            this.lblAuEditName.Text = "名称";
            // 
            // lblAuEditLoginPass
            // 
            this.lblAuEditLoginPass.AutoSize = true;
            this.lblAuEditLoginPass.Location = new System.Drawing.Point(60, 62);
            this.lblAuEditLoginPass.Name = "lblAuEditLoginPass";
            this.lblAuEditLoginPass.Size = new System.Drawing.Size(65, 12);
            this.lblAuEditLoginPass.TabIndex = 0;
            this.lblAuEditLoginPass.Text = "パスワード";
            // 
            // lblAuEditLoginId
            // 
            this.lblAuEditLoginId.AutoSize = true;
            this.lblAuEditLoginId.Location = new System.Drawing.Point(60, 33);
            this.lblAuEditLoginId.Name = "lblAuEditLoginId";
            this.lblAuEditLoginId.Size = new System.Drawing.Size(65, 12);
            this.lblAuEditLoginId.TabIndex = 0;
            this.lblAuEditLoginId.Text = "ログインID";
            // 
            // FrmAuth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 401);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "FrmAuth";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmAuth_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtAuEditLoginId;
        private System.Windows.Forms.Label lblAuEditAuthFlg;
        private System.Windows.Forms.Label lblAuEditMailFlg;
        private System.Windows.Forms.Label lblAuEditMail;
        private System.Windows.Forms.Label lblAuEditName;
        private System.Windows.Forms.Label lblAuEditLoginPass;
        private System.Windows.Forms.Label lblAuEditLoginId;
        private System.Windows.Forms.TextBox txtAuEditMail;
        private System.Windows.Forms.TextBox txtAuEditName;
        private System.Windows.Forms.TextBox txtAuEditLoginPass;
        private System.Windows.Forms.Button btnAuthSave;
        private System.Windows.Forms.Button btnAuthCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbAuthFlgNo;
        private System.Windows.Forms.RadioButton rbAuthFlgYes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbAuthMailNo;
        private System.Windows.Forms.RadioButton rbAuthMailYes;
        private System.Windows.Forms.TextBox txtAuthEditId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}