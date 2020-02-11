namespace BudBackupSystem
{
    partial class FrmLogin
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
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtLoginPass = new System.Windows.Forms.TextBox();
            this.lblLoginPass = new System.Windows.Forms.Label();
            this.txtLoginId = new System.Windows.Forms.TextBox();
            this.lblLoginId = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.tabControl1.Location = new System.Drawing.Point(22, 26);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(408, 236);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnLogin);
            this.tabPage1.Controls.Add(this.txtLoginPass);
            this.tabPage1.Controls.Add(this.lblLoginPass);
            this.tabPage1.Controls.Add(this.txtLoginId);
            this.tabPage1.Controls.Add(this.lblLoginId);
            this.tabPage1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(400, 210);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ログイン";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(242, 147);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(95, 23);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "ログイン";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtLoginPass
            // 
            this.txtLoginPass.Location = new System.Drawing.Point(160, 84);
            this.txtLoginPass.Name = "txtLoginPass";
            this.txtLoginPass.PasswordChar = '*';
            this.txtLoginPass.Size = new System.Drawing.Size(126, 19);
            this.txtLoginPass.TabIndex = 2;
            this.txtLoginPass.UseSystemPasswordChar = true;
            this.txtLoginPass.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLoginPass_KeyUp);
            // 
            // lblLoginPass
            // 
            this.lblLoginPass.AutoSize = true;
            this.lblLoginPass.Location = new System.Drawing.Point(77, 89);
            this.lblLoginPass.Name = "lblLoginPass";
            this.lblLoginPass.Size = new System.Drawing.Size(65, 12);
            this.lblLoginPass.TabIndex = 2;
            this.lblLoginPass.Text = "パスワード";
            // 
            // txtLoginId
            // 
            this.txtLoginId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtLoginId.Location = new System.Drawing.Point(160, 47);
            this.txtLoginId.Name = "txtLoginId";
            this.txtLoginId.Size = new System.Drawing.Size(126, 19);
            this.txtLoginId.TabIndex = 1;
            this.txtLoginId.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLoginId_KeyUp);
            // 
            // lblLoginId
            // 
            this.lblLoginId.AutoSize = true;
            this.lblLoginId.Location = new System.Drawing.Point(77, 50);
            this.lblLoginId.Name = "lblLoginId";
            this.lblLoginId.Size = new System.Drawing.Size(65, 12);
            this.lblLoginId.TabIndex = 0;
            this.lblLoginId.Text = "ログインID";
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 287);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.MaximizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmLogin_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblLoginId;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtLoginPass;
        private System.Windows.Forms.Label lblLoginPass;
        private System.Windows.Forms.TextBox txtLoginId;
    }
}