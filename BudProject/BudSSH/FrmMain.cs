using System.Windows.Forms;
using log4net;
using System.Reflection;
using BudSSH.Common.Util.FormStyle;
using BudSSH.Model;
using BudSSH.Controller;
using BudSSH.Common.Util;
using System;

namespace BudSSH
{
    public partial class FrmMain : Form
    {
        #region private

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        public FrmMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            //disabled the maximize
            FormStyleUtil.RemoveMaximizeMenu(this);
            //加载配置

            //创建并开启启动计划
            var taskController = new TaskController();
            taskController.BootTask();

            //启动后开始执行
            button2.PerformClick();
        }

        #region business

        #region config 
        #region path
        /// <summary>
        /// 选取日志输入路径：1.网络路径（从mac上获取监控日志）2.本地程序产生日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, System.EventArgs e)
        {
            //get the path
            if (DialogResult.OK == this.folderBrowserDialog1.ShowDialog())
            {
                this.inputlogTB.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// 选取日志输出路径：1.网络路径 2.本地路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, System.EventArgs e)
        {
            //get the path
            if (DialogResult.OK == this.folderBrowserDialog1.ShowDialog())
            {
                this.outputTB.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, System.EventArgs e)
        {
            bool status = false;
            try
            {
                //获取输入
                string logpath = this.inputlogTB.Text.Trim();
                string sshpath = this.outputTB.Text.Trim();
                //校验输入
                if (string.IsNullOrWhiteSpace(logpath) || string.IsNullOrWhiteSpace(sshpath))
                {
                    MessageBox.Show("完全に記入してください");
                    return;
                }
                //保存
                Config config = new Config()
                {
                    Path = new PathConfig() { InputLogPath = logpath, OutputPath = sshpath }
                };

                ConfigController cc = new ConfigController();
                status = cc.SaveConfig(config);

            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }

            if (status) MessageBox.Show(MessageUtil.GetMessage("SaveSuccess"));
            else MessageBox.Show(MessageUtil.GetMessage("SaveFailed"));

        }

        /// <summary>
        /// 加载路径配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPage1_Enter(object sender, System.EventArgs e)
        {
            try
            {
                var configController = new ConfigController();
                Config config = configController.LoadConfig();
                var path = config.Path;
                this.inputlogTB.Text = path.InputLogPath ?? "";
                this.outputTB.Text = path.OutputPath ?? "";
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                MessageBox.Show(MessageUtil.GetMessage("LoadFailed"));
            }
        }


        #endregion

        #region db

        /// <summary>
        /// 加载数据库配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPage2_Enter(object sender, System.EventArgs e)
        {
            try
            {
                var configController = new ConfigController();
                Config config = configController.LoadConfig();
                var db = config.DB;
                if (db != null)
                {
                    this.ServerNameTB.Text = db.ServerName ?? "";
                    this.LoginNameTB.Text = db.LoginName ?? "";
                    this.DatabaseNameTB.Text = db.DatabaseName ?? "";
                    this.PasswordTB.Text = db.Password ?? "";
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                MessageBox.Show(MessageUtil.GetMessage("LoadFailed"));
            }
        }
        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, System.EventArgs e)
        {
            //封装配置
            DBConfig dbconfig = new DBConfig()
            {
                ServerName = this.ServerNameTB.Text.Trim(),
                DatabaseName = this.DatabaseNameTB.Text.Trim(),
                LoginName = this.LoginNameTB.Text.Trim(),
                Password = this.PasswordTB.Text.Trim()
            };
            //校验配置
            if (!ValidateDB(dbconfig))
            {
                MessageBox.Show("完全に記入してください");
                return;
            }
            //测试
            this.button6.Text = MessageUtil.GetMessage("DBLinking");
            new System.Threading.Thread(delegate ()
            {
                string msg = string.Empty;
                ConfigController configC = new ConfigController();
                if (configC.TestDBServer(dbconfig))
                {
                    msg = MessageUtil.GetMessage("LinkSuccess");
                }
                else
                {
                    msg = MessageUtil.GetMessage("LinkFailed");
                }
                this.button6.BeginInvoke(new EventHandler(delegate (Object send, EventArgs ea) { MessageBox.Show(msg); this.button6.Text = "接続テスト"; }), new Object[] { this, EventArgs.Empty });

            }).Start();

        }

        private bool ValidateDB(DBConfig dbconfig)
        {
            if (string.IsNullOrWhiteSpace(dbconfig.ServerName) || string.IsNullOrWhiteSpace(dbconfig.DatabaseName) || string.IsNullOrWhiteSpace(dbconfig.LoginName) || string.IsNullOrWhiteSpace(dbconfig.Password))
                return false;
            else return true;
        }

        /// <summary>
        /// 保存数据库配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            bool status = false;
            //获取配置
            Config config = new Config()
            {
                DB = new DBConfig()
                {
                    ServerName = this.ServerNameTB.Text.Trim(),
                    DatabaseName = this.DatabaseNameTB.Text.Trim(),
                    LoginName = this.LoginNameTB.Text.Trim(),
                    Password = this.PasswordTB.Text.Trim()
                }
            };
            //校验配置
            if (!ValidateDB(config.DB))
            {
                MessageBox.Show("完全に記入してください");
                return;
            }
            //保存配置
            try
            {
                ConfigController cc = new ConfigController();
                status = cc.SaveConfig(config);
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }

            if (status) MessageBox.Show(MessageUtil.GetMessage("SaveSuccess"));
            else MessageBox.Show(MessageUtil.GetMessage("SaveFailed"));

        }

        #endregion
        #endregion
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, System.EventArgs e)
        {
            //判断配置是否完整
            ConfigController cc = new ConfigController();
            if (!ValidateConfig(cc.LoadConfig()))
            {
                MessageBox.Show("完全に記入してください");
                return;
            }

            this.button2.Enabled = false;
            OperationController oc = new OperationController();
            oc.Start();

            this.button1.Enabled = true;
        }

        private bool ValidateConfig(Config config)
        {
            return ValidateDB(config.DB) && !(string.IsNullOrWhiteSpace(config.Path.InputLogPath) || string.IsNullOrWhiteSpace(config.Path.OutputPath));
        }

        /// <summary>
        /// 停止copy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            OperationController oc = new OperationController();
            oc.Stop();

            this.button2.Enabled = true;
        }

        #endregion

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
            try
            {
                this.WindowState = FormWindowState.Minimized;   //最小化窗口  
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
            catch (System.Exception)
            {
                Application.Exit();
            }
        }
        #endregion

    }
}
