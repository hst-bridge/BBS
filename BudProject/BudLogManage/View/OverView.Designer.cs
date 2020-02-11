namespace BudLogManage.View
{
    partial class OverView
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bytestransport = new System.Windows.Forms.Label();
            this.filetransport = new System.Windows.Forms.Label();
            this.bytesdelete = new System.Windows.Forms.Label();
            this.filedelete = new System.Windows.Forms.Label();
            this.bytescopy = new System.Windows.Forms.Label();
            this.filecopy = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtpLogEndTime = new System.Windows.Forms.DateTimePicker();
            this.dtpLogStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblLogTime = new System.Windows.Forms.Label();
            this.lblLogDate = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileTransDelete = new System.Windows.Forms.Label();
            this.bytesTransDelete = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(393, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "輸出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.bytestransport);
            this.panel2.Controls.Add(this.bytesTransDelete);
            this.panel2.Controls.Add(this.filetransport);
            this.panel2.Controls.Add(this.fileTransDelete);
            this.panel2.Controls.Add(this.bytesdelete);
            this.panel2.Controls.Add(this.filedelete);
            this.panel2.Controls.Add(this.bytescopy);
            this.panel2.Controls.Add(this.filecopy);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(20, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(367, 202);
            this.panel2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(247, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "転送";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "バイト";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "削除";
            // 
            // bytestransport
            // 
            this.bytestransport.AutoSize = true;
            this.bytestransport.Location = new System.Drawing.Point(250, 113);
            this.bytestransport.Name = "bytestransport";
            this.bytestransport.Size = new System.Drawing.Size(17, 12);
            this.bytestransport.TabIndex = 2;
            this.bytestransport.Text = "0B";
            // 
            // filetransport
            // 
            this.filetransport.AutoSize = true;
            this.filetransport.Location = new System.Drawing.Point(253, 64);
            this.filetransport.Name = "filetransport";
            this.filetransport.Size = new System.Drawing.Size(11, 12);
            this.filetransport.TabIndex = 2;
            this.filetransport.Text = "0";
            // 
            // bytesdelete
            // 
            this.bytesdelete.AutoSize = true;
            this.bytesdelete.Location = new System.Drawing.Point(170, 113);
            this.bytesdelete.Name = "bytesdelete";
            this.bytesdelete.Size = new System.Drawing.Size(17, 12);
            this.bytesdelete.TabIndex = 2;
            this.bytesdelete.Text = "0B";
            // 
            // filedelete
            // 
            this.filedelete.AutoSize = true;
            this.filedelete.Location = new System.Drawing.Point(176, 64);
            this.filedelete.Name = "filedelete";
            this.filedelete.Size = new System.Drawing.Size(11, 12);
            this.filedelete.TabIndex = 2;
            this.filedelete.Text = "0";
            // 
            // bytescopy
            // 
            this.bytescopy.AutoSize = true;
            this.bytescopy.Location = new System.Drawing.Point(99, 113);
            this.bytescopy.Name = "bytescopy";
            this.bytescopy.Size = new System.Drawing.Size(17, 12);
            this.bytescopy.TabIndex = 2;
            this.bytescopy.Text = "0B";
            // 
            // filecopy
            // 
            this.filecopy.AutoSize = true;
            this.filecopy.Location = new System.Drawing.Point(102, 63);
            this.filecopy.Name = "filecopy";
            this.filecopy.Size = new System.Drawing.Size(11, 12);
            this.filecopy.TabIndex = 2;
            this.filecopy.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "ファイル";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "コピー済み";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.dtpLogEndTime);
            this.panel1.Controls.Add(this.dtpLogStartTime);
            this.panel1.Controls.Add(this.dtpEndDate);
            this.panel1.Controls.Add(this.dtpStartDate);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.lblLogTime);
            this.panel1.Controls.Add(this.lblLogDate);
            this.panel1.Location = new System.Drawing.Point(20, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(367, 63);
            this.panel1.TabIndex = 3;
            // 
            // dtpLogEndTime
            // 
            this.dtpLogEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpLogEndTime.Location = new System.Drawing.Point(221, 28);
            this.dtpLogEndTime.Name = "dtpLogEndTime";
            this.dtpLogEndTime.ShowUpDown = true;
            this.dtpLogEndTime.Size = new System.Drawing.Size(108, 21);
            this.dtpLogEndTime.TabIndex = 4;
            this.dtpLogEndTime.Value = new System.DateTime(2014, 1, 29, 23, 59, 59, 0);
            // 
            // dtpLogStartTime
            // 
            this.dtpLogStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpLogStartTime.Location = new System.Drawing.Point(78, 28);
            this.dtpLogStartTime.Name = "dtpLogStartTime";
            this.dtpLogStartTime.ShowUpDown = true;
            this.dtpLogStartTime.Size = new System.Drawing.Size(108, 21);
            this.dtpLogStartTime.TabIndex = 3;
            this.dtpLogStartTime.Value = new System.DateTime(2014, 1, 29, 0, 0, 0, 0);
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Location = new System.Drawing.Point(221, 2);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(108, 21);
            this.dtpEndDate.TabIndex = 2;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(78, 3);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(108, 21);
            this.dtpStartDate.TabIndex = 1;
            this.dtpStartDate.Value = new System.DateTime(2014, 6, 16, 0, 0, 0, 0);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(198, 31);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 31;
            this.label17.Text = "～";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(198, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 12);
            this.label16.TabIndex = 30;
            this.label16.Text = "～";
            // 
            // lblLogTime
            // 
            this.lblLogTime.AutoSize = true;
            this.lblLogTime.Location = new System.Drawing.Point(13, 32);
            this.lblLogTime.Name = "lblLogTime";
            this.lblLogTime.Size = new System.Drawing.Size(41, 12);
            this.lblLogTime.TabIndex = 29;
            this.lblLogTime.Text = "時間：";
            // 
            // lblLogDate
            // 
            this.lblLogDate.AutoSize = true;
            this.lblLogDate.Location = new System.Drawing.Point(13, 10);
            this.lblLogDate.Name = "lblLogDate";
            this.lblLogDate.Size = new System.Drawing.Size(41, 12);
            this.lblLogDate.TabIndex = 28;
            this.lblLogDate.Text = "日付：";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(393, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(83, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "査問";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            this.openFileDialog1.Title = "ファイルを選択します";
            // 
            // fileTransDelete
            // 
            this.fileTransDelete.AutoSize = true;
            this.fileTransDelete.Location = new System.Drawing.Point(315, 64);
            this.fileTransDelete.Name = "fileTransDelete";
            this.fileTransDelete.Size = new System.Drawing.Size(11, 12);
            this.fileTransDelete.TabIndex = 2;
            this.fileTransDelete.Text = "0";
            // 
            // bytesTransDelete
            // 
            this.bytesTransDelete.AutoSize = true;
            this.bytesTransDelete.Location = new System.Drawing.Point(309, 113);
            this.bytesTransDelete.Name = "bytesTransDelete";
            this.bytesTransDelete.Size = new System.Drawing.Size(17, 12);
            this.bytesTransDelete.TabIndex = 2;
            this.bytesTransDelete.Text = "0B";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(309, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "削除";
            // 
            // OverView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "OverView";
            this.Size = new System.Drawing.Size(495, 326);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker dtpLogEndTime;
        private System.Windows.Forms.DateTimePicker dtpLogStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblLogTime;
        private System.Windows.Forms.Label lblLogDate;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label bytesdelete;
        private System.Windows.Forms.Label filedelete;
        private System.Windows.Forms.Label bytescopy;
        private System.Windows.Forms.Label filecopy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label bytestransport;
        private System.Windows.Forms.Label filetransport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label bytesTransDelete;
        private System.Windows.Forms.Label fileTransDelete;
    }
}
