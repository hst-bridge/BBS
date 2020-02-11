namespace BudBackupSystem
{
    partial class FrmGroupFile
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSystemFileFlg = new System.Windows.Forms.CheckBox();
            this.chkHiddenFileFlg = new System.Windows.Forms.CheckBox();
            this.txtExceptAttr1 = new System.Windows.Forms.TextBox();
            this.txtExceptAttr2 = new System.Windows.Forms.TextBox();
            this.txtExceptAttr3 = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkExceptAttr1 = new System.Windows.Forms.CheckBox();
            this.chkExceptAttr2 = new System.Windows.Forms.CheckBox();
            this.chkExceptAttr3 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(16, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(112, 14);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "全フォルダ共通";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(14, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "【除外条件】";
            // 
            // chkSystemFileFlg
            // 
            this.chkSystemFileFlg.AutoSize = true;
            this.chkSystemFileFlg.Location = new System.Drawing.Point(50, 290);
            this.chkSystemFileFlg.Name = "chkSystemFileFlg";
            this.chkSystemFileFlg.Size = new System.Drawing.Size(120, 16);
            this.chkSystemFileFlg.TabIndex = 13;
            this.chkSystemFileFlg.Text = "システムファイル";
            this.chkSystemFileFlg.UseVisualStyleBackColor = true;
            this.chkSystemFileFlg.Visible = false;
            // 
            // chkHiddenFileFlg
            // 
            this.chkHiddenFileFlg.AutoSize = true;
            this.chkHiddenFileFlg.Location = new System.Drawing.Point(50, 239);
            this.chkHiddenFileFlg.Name = "chkHiddenFileFlg";
            this.chkHiddenFileFlg.Size = new System.Drawing.Size(96, 16);
            this.chkHiddenFileFlg.TabIndex = 14;
            this.chkHiddenFileFlg.Text = "隠しファイル";
            this.chkHiddenFileFlg.UseVisualStyleBackColor = true;
            this.chkHiddenFileFlg.Visible = false;
            // 
            // txtExceptAttr1
            // 
            this.txtExceptAttr1.Location = new System.Drawing.Point(131, 99);
            this.txtExceptAttr1.Name = "txtExceptAttr1";
            this.txtExceptAttr1.Size = new System.Drawing.Size(249, 19);
            this.txtExceptAttr1.TabIndex = 8;
            // 
            // txtExceptAttr2
            // 
            this.txtExceptAttr2.Location = new System.Drawing.Point(131, 141);
            this.txtExceptAttr2.Name = "txtExceptAttr2";
            this.txtExceptAttr2.Size = new System.Drawing.Size(249, 19);
            this.txtExceptAttr2.TabIndex = 10;
            // 
            // txtExceptAttr3
            // 
            this.txtExceptAttr3.Location = new System.Drawing.Point(131, 185);
            this.txtExceptAttr3.Name = "txtExceptAttr3";
            this.txtExceptAttr3.Size = new System.Drawing.Size(249, 19);
            this.txtExceptAttr3.TabIndex = 12;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(232, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(377, 290);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 23);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkExceptAttr1
            // 
            this.chkExceptAttr1.AutoSize = true;
            this.chkExceptAttr1.Location = new System.Drawing.Point(50, 101);
            this.chkExceptAttr1.Name = "chkExceptAttr1";
            this.chkExceptAttr1.Size = new System.Drawing.Size(60, 16);
            this.chkExceptAttr1.TabIndex = 17;
            this.chkExceptAttr1.Text = "拡張子";
            this.chkExceptAttr1.UseVisualStyleBackColor = true;
            // 
            // chkExceptAttr2
            // 
            this.chkExceptAttr2.AutoSize = true;
            this.chkExceptAttr2.Location = new System.Drawing.Point(50, 141);
            this.chkExceptAttr2.Name = "chkExceptAttr2";
            this.chkExceptAttr2.Size = new System.Drawing.Size(60, 16);
            this.chkExceptAttr2.TabIndex = 17;
            this.chkExceptAttr2.Text = "拡張子";
            this.chkExceptAttr2.UseVisualStyleBackColor = true;
            // 
            // chkExceptAttr3
            // 
            this.chkExceptAttr3.AutoSize = true;
            this.chkExceptAttr3.Location = new System.Drawing.Point(50, 186);
            this.chkExceptAttr3.Name = "chkExceptAttr3";
            this.chkExceptAttr3.Size = new System.Drawing.Size(60, 16);
            this.chkExceptAttr3.TabIndex = 17;
            this.chkExceptAttr3.Text = "拡張子";
            this.chkExceptAttr3.UseVisualStyleBackColor = true;
            // 
            // FrmGroupFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 335);
            this.Controls.Add(this.chkExceptAttr3);
            this.Controls.Add(this.chkExceptAttr2);
            this.Controls.Add(this.chkExceptAttr1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtExceptAttr3);
            this.Controls.Add(this.txtExceptAttr2);
            this.Controls.Add(this.txtExceptAttr1);
            this.Controls.Add(this.chkHiddenFileFlg);
            this.Controls.Add(this.chkSystemFileFlg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("MS Gothic", 9F);
            this.MaximizeBox = false;
            this.Name = "FrmGroupFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BUD Backup System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmGroupFile_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmGroupFile_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSystemFileFlg;
        private System.Windows.Forms.CheckBox chkHiddenFileFlg;
        private System.Windows.Forms.TextBox txtExceptAttr1;
        private System.Windows.Forms.TextBox txtExceptAttr2;
        private System.Windows.Forms.TextBox txtExceptAttr3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkExceptAttr1;
        private System.Windows.Forms.CheckBox chkExceptAttr2;
        private System.Windows.Forms.CheckBox chkExceptAttr3;
    }
}