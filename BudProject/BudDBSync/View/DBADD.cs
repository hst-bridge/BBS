using System;
using BudDBSync.FormValidator.Common;
using BudDBSync.FormValidator;
using System.Windows.Forms;
using BudDBSync.Model;
using BudDBSync.BLL;
using BudDBSync.Util.Message;

namespace BudDBSync.View
{
    internal partial class DBADD : ViewControl
    {

        //用于保存最近一次可用的target server
        private DBServer targetServerAvailable = null;

        private IValidator validator = null;
        internal IValidator Validator
        {
            get
            {
                if (validator == null)
                {
                    validator = new DBADDValidator(this.panel1);
                }
                return validator;
            }
        }

        private DBServerManager dsm = new DBServerManager();

        public DBADD()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewLocation.SwitchTo(new DBList());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            #region 测试是否可用
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

            if (targetServerAvailable == null || !targetServerAvailable.ConnString.Equals(ds.ConnString))
            {
                MessageBox.Show(MessageUtil.GetMessage("LinkMust"));
                return;
            }
            //判断是否已经存在
            if (dsm.GetAllSourceInfolist().Exists(x => x.ServerName.Equals(ds.ServerName) && x.DatabaseName.Equals(ds.DatabaseName)))
            {
                MessageBox.Show(MessageUtil.GetMessage("dsExisted"));
                return;
            }
            #endregion

            if (dsm.AddSourceDBServer(ds))
            {
                MessageBox.Show(MessageUtil.GetMessage("SaveSuccess"));
                ViewLocation.SwitchTo(new DBList());
            }
            else
            {
                MessageBox.Show(MessageUtil.GetMessage("SaveFailed"));
            }
            
        }

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
    }
}
