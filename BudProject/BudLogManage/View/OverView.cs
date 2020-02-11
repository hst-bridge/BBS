using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudLogManage.Model;
using BudLogManage.Common.Util;
using BudLogManage.Controller;
using System.Reflection;
using log4net;

namespace BudLogManage.View
{
    internal partial class OverView : ViewControl
    {
        public OverView()
        {
            InitializeComponent();
        }
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool isCalc = false;

        //用于存储上一次查询所选择的时间间隔
        private TimeInterval preTI = null;
        /// <summary>
        /// 解析读取内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (isCalc == false)
            {
                
                    isCalc = true;
                    this.button2.Text = MessageUtil.GetMessage("waiting");
                    new System.Threading.Thread(() =>
                    {
                        try
                        {

                            //获取开始时间
                            TimeInterval ti = new TimeInterval()
                            {
                                Start = DateTimeUtil.GetDateTime(this.dtpStartDate.Text, this.dtpLogStartTime.Text),
                                End = DateTimeUtil.GetDateTime(this.dtpEndDate.Text, this.dtpLogEndTime.Text)
                            };
                            //保存时间间隔
                            preTI = ti;

                            TotalController tc = new TotalController();
                            Total total = tc.GetTotal(ti);
                            this.BeginInvoke(new EventHandler(delegate(Object send, EventArgs ea)
                            {
                                this.filecopy.Text = total.FilesCopied.ToString();
                                this.bytescopy.Text = total.BytesCopied.ToString();
                                this.filedelete.Text = total.FilesDeleted.ToString();
                                this.bytesdelete.Text = total.BytesDeleted.ToString();
                                this.filetransport.Text = total.FileTransfered.ToString();
                                this.bytestransport.Text = total.BytesTransfered.ToString();
                                this.fileTransDelete.Text = total.FileTransDeleted.ToString();
                                this.bytesTransDelete.Text = total.BytesTransDeleted.ToString();

                            }), new Object[] { this, EventArgs.Empty });

                            System.Threading.Thread.Sleep(500);

                        }
                        catch (System.Exception ex)
                        {
                            logger.Error(ex.Message);
                            MessageBox.Show(MessageUtil.GetMessage("UnknownError"));
                        }
                        finally
                        {
                           
                            this.BeginInvoke(new EventHandler(delegate(Object send, EventArgs ea) { this.button2.Text = "査問"; }), new Object[] { this, EventArgs.Empty });
                            isCalc = false;
                        }
                    }).Start();
                
            }
        }

        /// <summary>
        /// 导出到文件 暂时只支持CSV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(preTI==null){
                    MessageBox.Show(MessageUtil.GetMessage("NoValueForExport"));
                    return;
                }
                //选择文件路径
                if (DialogResult.OK == this.openFileDialog1.ShowDialog())
                {

                    string fpath = this.openFileDialog1.FileName;
                    Total total = new Total()
                    {
                        FilesCopied = Convert.ToInt32(this.filecopy.Text),
                        BytesCopied = new BudLogManage.Common.Helper.Size(this.bytescopy.Text),

                        FilesDeleted = Convert.ToInt32(this.filedelete.Text),
                        BytesDeleted = new BudLogManage.Common.Helper.Size(this.bytesdelete.Text),

                        FileTransfered = Convert.ToInt32(this.filetransport.Text),
                        BytesTransfered = new Common.Helper.Size(this.bytestransport.Text),
                        FileTransDeleted = Convert.ToInt32(this.fileTransDelete.Text),
                        BytesTransDeleted = new BudLogManage.Common.Helper.Size(this.bytesTransDelete.Text),
                        TimeInterval = preTI
                    };

                    TotalController tc = new TotalController();
                    tc.Export(total, fpath);

                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show(MessageUtil.GetMessage("ExportFailed"));
            }

        }
    }
}
