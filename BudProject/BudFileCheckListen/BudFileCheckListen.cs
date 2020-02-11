using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using BudFileCheckListen.BLL;

namespace BudFileCheckListen
{
    public partial class BudFileCheckListen : Form
    {
        /// <summary>
        /// CheckTime
        /// </summary>
        private int CheckTime = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["CheckTime"]);
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BudFileCheckListen()
        {
            InitializeComponent();
        }

        #region 托盘相关程序
        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            //通知监听器 程序将关闭
            FileListener.Instance.ShutDown();
            this.notifyIcon.Visible = false;
            //clear database pool
            System.Data.SqlClient.SqlConnection.ClearAllPools();
            this.Close();
            this.Dispose();
            Application.Exit();
        }
        private void BudFileCheckListen_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;   //最小化窗口  
            this.ShowInTaskbar = false;
            this.notifyIcon.Visible = true;
        }


        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)   //判断窗体是不是最小化  
                this.WindowState = FormWindowState.Normal;   //如果是最小化就把窗体状态改为默认大小  
            this.Activate();  //激活窗体并赋予焦点，这句话可以不写  
            this.ShowInTaskbar = true;
        }
        #endregion

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
           
            try
            {
                //监听
                FileListener.Instance.Listen((ex) => {
                    Exception exception = ex as Exception;
                    if (ex == null) return true;
                    DialogResult dialogResult = System.Windows.Forms.DialogResult.Ignore;
                    if (ex is ArgumentException)
                    {
                        dialogResult = MessageBox.Show(exception.Message + " 終了するかどうか", "エラー", MessageBoxButtons.YesNo);
                    }
                    else
                    {
                        dialogResult = MessageBox.Show("エラー,終了するかどうか", "エラー", MessageBoxButtons.YesNo);
                    }
                    if (dialogResult == DialogResult.Yes)
                    {
                        //回复原来状态
                        this.BeginInvoke(new EventHandler((x,y)=>{
                            this.button2.Enabled = false;
                            this.button1.Enabled = true;
                        }));

                        return true;
                    }

                    return false;
                    
                });
                this.button2.Enabled = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show("システムエラーが、ログを参照してください");
                this.button1.Enabled = true;
                
            }
           
            
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            try
            {
                //监听
                FileListener.Instance.Stop();
                this.button1.Enabled = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show("システムエラーが、ログを参照してください");
                this.button2.Enabled = true;
            }
           
            
            
        }

       
        

        
    }
}
