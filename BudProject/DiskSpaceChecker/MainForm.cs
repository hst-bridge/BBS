using DiskSpaceChecker.BLL;
using DiskSpaceChecker.Common.Util;
using DiskSpaceChecker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskSpaceChecker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        /// <summary>
        /// check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //get the config
            string volume = (string)this.comboBox1.SelectedItem;
            if (string.IsNullOrWhiteSpace(volume))
            {
                MessageBox.Show("please select the volume.");
                return;
            }

            string baseline = this.textBox1.Text;
            if (string.IsNullOrWhiteSpace(baseline))
            {
                MessageBox.Show("please input the baseline.");
                return;
            }

            Config config = new Config() { Volume = volume, Baseline = baseline };
            OperationController opCtrl = new OperationController();
            opCtrl.Check(config);
            this.button1.Enabled = false;
            this.button2.Enabled = true;
        }

        #region 托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)   //判断窗体是不是最小化  
                this.WindowState = FormWindowState.Normal;   //如果是最小化就把窗体状态改为默认大小  
            this.Activate();  //激活窗体并赋予焦点，这句话可以不写  
            this.ShowInTaskbar = true;
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //关闭业务线程
            OperationController oc = new OperationController();
            oc.Stop();

            this.notifyIcon1.Visible = false;
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;   //最小化窗口  
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }
        #endregion

        /// <summary>
        /// 加载本地盘符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //disabled the maximize
            FormStyleUtil.RemoveMaximizeMenu(this);

            ConfigController configCtrl = new ConfigController();
            Config config = configCtrl.Get();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady)
                {
                    this.comboBox1.Items.Add(d.Name);
                    if (d.Name.Equals(config.Volume)) this.comboBox1.SelectedItem = d.Name;
                }

            }
            this.textBox1.Text = config.Baseline;
        }

        /// <summary>
        /// cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OperationController opCtrl = new OperationController();
            opCtrl.Stop();
            this.button2.Enabled = false;
            this.button1.Enabled = true;
        }
    }
}
