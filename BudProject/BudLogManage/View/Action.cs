using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudLogManage.Controller;
using BudLogManage.Model;
using BudLogManage.Common.Util;

namespace BudLogManage.View
{
    internal partial class Action : ViewControl
    {
        public Action()
        {
            InitializeComponent();
            timer.Elapsed += (x, y) => RefreshNumber();
            timer.AutoReset = true;
 
        }

        private bool isReading = false;
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        /// <summary>
        /// 启动任务调度器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isReading)
            {
                isReading = true;
                int total = Convert.ToInt32(this.label3.Text);
                if (total > 0)
                {
                    this.label4.Text = "0";
                   // this.button1.Enabled = false;
                    this.button1.Text = MessageUtil.GetMessage("waiting");
                    ActionController ac = new ActionController();
                    ac.Start();
                    timer.Enabled = true;
                }
            }
            
        }

        /// <summary>
        /// 刷新处理文件数
        /// </summary>
        private void RefreshNumber()
        {
            this.timer.Enabled = false;
            //获取文件总数
            ActionController ac = new ActionController();
            Status sts = ac.GetStatus();
            int frc = sts.FilesReadedCount;
            int frcBefore = Convert.ToInt32(this.label4.Text);
            while(frcBefore<frc){
                int current = ++frcBefore;
                this.BeginInvoke(new EventHandler(delegate(Object send, EventArgs ea) { this.label4.Text = current.ToString(); }), new Object[] { this, EventArgs.Empty });
                System.Threading.Thread.Sleep(5);
            }

            if (frc == Convert.ToInt32(this.label3.Text))
            {
                //读取完毕
                this.BeginInvoke(new EventHandler(delegate(Object send, EventArgs ea) { this.button1.Text = MessageUtil.GetMessage("ReadComplete"); }), new Object[] { this, EventArgs.Empty });
                this.isReading = false;
                return;
            }

            this.timer.Enabled = true;
        }

        /// <summary>
        /// 停止任务调度器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ActionController ac = new ActionController();
            ac.Stop();
        }

        private void Action_Load(object sender, EventArgs e)
        {
            //获取文件总数
            ActionController ac = new ActionController();
            Status sts = ac.GetStatus();
            this.label3.Text = sts.FilesTotalCount.ToString();
            this.label4.Text = sts.FilesReadedCount.ToString();
        }

        ~Action()
        {
            this.timer.Enabled = false;
            this.timer.Close();
        }
    }
}
