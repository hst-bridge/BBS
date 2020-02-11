namespace BudFileListen
{
    partial class FormModuleSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModuleSelect));
            this.rdBtnAuto = new System.Windows.Forms.RadioButton();
            this.rdBtnManual = new System.Windows.Forms.RadioButton();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdBtnAuto
            // 
            this.rdBtnAuto.AutoSize = true;
            this.rdBtnAuto.Checked = true;
            this.rdBtnAuto.Location = new System.Drawing.Point(13, 44);
            this.rdBtnAuto.Name = "rdBtnAuto";
            this.rdBtnAuto.Size = new System.Drawing.Size(47, 16);
            this.rdBtnAuto.TabIndex = 0;
            this.rdBtnAuto.TabStop = true;
            this.rdBtnAuto.Text = "自動";
            this.rdBtnAuto.UseVisualStyleBackColor = true;
            // 
            // rdBtnManual
            // 
            this.rdBtnManual.AutoSize = true;
            this.rdBtnManual.Location = new System.Drawing.Point(154, 44);
            this.rdBtnManual.Name = "rdBtnManual";
            this.rdBtnManual.Size = new System.Drawing.Size(47, 16);
            this.rdBtnManual.TabIndex = 1;
            this.rdBtnManual.Text = "手動";
            this.rdBtnManual.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(80, 142);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "実行";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(206, 142);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdBtnAuto);
            this.panel1.Controls.Add(this.rdBtnManual);
            this.panel1.Location = new System.Drawing.Point(67, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 100);
            this.panel1.TabIndex = 3;
            // 
            // FormModuleSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 194);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormModuleSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BudFileListen";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdBtnAuto;
        private System.Windows.Forms.RadioButton rdBtnManual;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panel1;
    }
}