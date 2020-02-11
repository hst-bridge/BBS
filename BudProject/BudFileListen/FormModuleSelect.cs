using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Common;

namespace BudFileListen
{
    public partial class FormModuleSelect : Form
    {
        public FormModuleSelect()
        {
            InitializeComponent();
        }

        /// <summary>
        /// log4net
        /// </summary>
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.btnRun.Enabled = false;
            try
            {
                bool isAuto = this.rdBtnAuto.Checked;
                //set 自動 or 手動
                ConfigurationManager.AppSettings["ControlFlg"] = isAuto ? "1" : "0";

                //配置中设定的时间：
                //DateTime startTime = DateTime.Parse(ConfigurationManager.AppSettings["BatchStartTime"]);
                //DateTime endTime = DateTime.Parse(ConfigurationManager.AppSettings["BatchEndTime"]);

                //自动：结束时间设为此时的上一分钟，明天在设置的开始时间自动执行；
                //手动：使得结束时间在此时的后两小时，可以立即执行
                //ConfigurationManager.AppSettings["BatchEndTime"] = isAuto ? startTime.AddHours(3).ToString("HH:mm") : endTime.AddHours(2).ToString("HH:mm");

                BudFileListen budFileListen = new BudFileListen();
                this.Hide();
                budFileListen.Show();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                MsgHelper.InfoMsg("失敗しました", "失敗提示");
                Application.Exit();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
