using System;
using System.Windows.Forms;
using BudDataSync.Util.FormStyle;
using BudDBSync.View;
using BudDBSync.FormValidator.Common;
using BudDBSync.FormValidator;
using BudDBSync.BLL;
using BudDBSync.Model;
using BudDBSync.Util.Message;

namespace BudDBSync
{
    public partial class FrmMain : Form
    {
        #region private
        private ViewManager viewManager;
        internal ViewManager ViewManager
        {
            get
            {
                if (viewManager == null)
                {
                    viewManager = new ViewManager(this.tabPage2);
                }
                return viewManager;
            }
            set{viewManager = value;}
            
        }

        private IValidator validator = null;
        internal IValidator Validator
        {
            get
            {
                if (validator == null)
                {
                    validator = new FrmMainValidator(this.tabPage1);
                }
                return validator;
            }
        }

        private DBServerManager dsm = new DBServerManager();
        private DBSyncManager dbsyncm = new DBSyncManager();
        //用于保存最近一次可用的target server
        private DBServer targetServerAvailable = null;
        #endregion

        public FrmMain()
        {
            InitializeComponent();
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
           
            //disabled the maximize
            FormStyleUtil.RemoveMaximizeMenu(this);
            
            //load targetdb info
            LoadTargetDBINFO();

            //dbserver list represent
            ViewManager.Load(new DBList());
            
        }

        #region begin sync And end sync
        /// <summary>
        /// begin sync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                #region 判断配置中的targetdb是否可用
                
                DBServer target = dsm.GetTargetInfo();
                if (string.IsNullOrWhiteSpace(target.ServerName) && !dsm.LinkTest(target))
                {
                    MessageBox.Show(MessageUtil.GetMessage("targetUnavailable"));
                    return;
                }
                
                #endregion

                /**
                 * 需要优化点：能够很好的反馈数据源的正确性 
                 */

                dbsyncm.BeginSync();
                this.button1.Enabled = false;
                this.button2.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show(MessageUtil.GetMessage("beginFailed"));
            }
        }
        /// <summary>
        /// endsync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            dbsyncm.EndSync();
            this.button2.Enabled = false;
            this.button1.Enabled = true;
        }
        #endregion

        #region target db

        private void LoadTargetDBINFO()
        {
            DBServer ds = dsm.GetTargetInfo();
           
            this.textBox1.Text = ds.ServerName;
            this.textBox2.Text = ds.LoginName;
            this.textBox3.Text = ds.Password;
            this.textBox4.Text = ds.DatabaseName;
            if (!string.IsNullOrWhiteSpace(ds.ServerName))
            {
                targetServerAvailable = ds;
                SetTargetReadOnly();
            }
        }

        private void SetTargetReadOnly()
        {
            this.textBox1.ReadOnly = true;
            this.textBox2.ReadOnly = true;
            this.textBox3.ReadOnly = true;
            this.textBox4.ReadOnly = true;
            this.button4.Text = "編集";
        }

        private void SetTargetWriteable()
        {
            this.textBox1.ReadOnly = false;
            this.textBox2.ReadOnly = false;
            this.textBox3.ReadOnly = false;
            this.textBox4.ReadOnly = false;
            this.button4.Text = "保存";
        }

        /// <summary>
        /// Test whether the target server is connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //valid the textbox input
            string prompt = null;
            if (!Validator.Validate(out prompt))
            {
                MessageBox.Show(prompt);
                return;
            }

            DBServer ds = new DBServer()
            {
                ServerName = this.textBox1.Text.Trim(),
                LoginName = this.textBox2.Text.Trim(),
                Password = this.textBox3.Text.Trim(),
                DatabaseName = this.textBox4.Text.Trim()
            };

            #region test link
            this.button3.Text = MessageUtil.GetMessage("DBLinking");
            new System.Threading.Thread(delegate()
            {
                if (dsm.LinkTest(ds))
                {
                    targetServerAvailable = ds;
                    MessageBox.Show(MessageUtil.GetMessage("LinkSuccess"));
                }
                else
                {
                    MessageBox.Show(MessageUtil.GetMessage("LinkFailed"));
                }
                this.button3.BeginInvoke(new EventHandler(delegate(Object send, EventArgs ea) { this.button3.Text = "接続テスト"; }), new Object[] { this, EventArgs.Empty });

            }).Start();
            #endregion


        }
        private void button4_Click(object sender, EventArgs e)
        {
            //判断操作 1.保存 2.編集

            if (this.textBox1.ReadOnly)
            {
                //編集
                SetTargetWriteable();
                return;
            }

            #region 保存
            //valid the textbox input
            string prompt = null;
            if (!Validator.Validate(out prompt))
            {
                MessageBox.Show(prompt);
                return;
            }

            DBServer ds = new DBServer()
            {
                ServerName = this.textBox1.Text.Trim(),
                LoginName = this.textBox2.Text.Trim(),
                Password = this.textBox3.Text.Trim(),
                DatabaseName = this.textBox4.Text.Trim()
            };

            if (!targetServerAvailable.ConnString.Equals(ds.ConnString))
            {
                MessageBox.Show(MessageUtil.GetMessage("LinkMust"));
                return;
            }

            if (dsm.SetTargetInfo(ds))
            {
                MessageBox.Show(MessageUtil.GetMessage("SaveSuccess"));
                SetTargetReadOnly();
            }
            else
            {
                MessageBox.Show(MessageUtil.GetMessage("SaveFailed"));
            }

            #endregion

        }
        #endregion

        #region 窗口最小化 托盘
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)   //判断窗体是不是最小化  
                this.WindowState = FormWindowState.Normal;   //如果是最小化就把窗体状态改为默认大小  
            this.Activate();  //激活窗体并赋予焦点，这句话可以不写  
            this.ShowInTaskbar = true;
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbsyncm.EndSync();
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

    }
}
