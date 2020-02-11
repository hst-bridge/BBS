using System;
using BudDBSync.FormValidator.Common;
using BudDBSync.FormValidator;
using System.Windows.Forms;
using BudDBSync.Model;
using BudDBSync.BLL;
using BudDBSync.Util.Message;
using System.Data;

namespace BudDBSync.View
{
    /// <summary>
    /// 将servername databasename看成唯一约束,
    /// 故此两者不能修改(PS:修改此两者等于消除)
    /// </summary>
    internal partial class DBEDIT : ViewControl
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
                    validator = new DBEDITValidator(this.panel1);
                }
                return validator;
            }
        }

        private DBServerManager dbsm = new DBServerManager();

        private DBServer dbserver = null;
        private int index = 0;

        public DBEDIT(int index)
        {
            InitializeComponent();
            this.index = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewLocation.SwitchTo(new DBList());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //valid the textbox input
            string prompt = null;
            if (!Validator.Validate(out prompt))
            {
                MessageBox.Show(prompt);
                return;
            }
            
            dbserver.ServerName = this.textBox1.Text.Trim();
            dbserver.LoginName = this.textBox2.Text.Trim();
            dbserver.Password = this.textBox3.Text.Trim();
            dbserver.DatabaseName = this.textBox4.Text.Trim();

            if (targetServerAvailable==null || !targetServerAvailable.ConnString.Equals(dbserver.ConnString))
            {
                MessageBox.Show(MessageUtil.GetMessage("LinkMust"));
                return;
            }

            if (dbsm.UpdateSourceDBServer(index,dbserver))
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
                if (dbsm.LinkTest(ds))
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

        private void DBEDIT_Load(object sender, EventArgs e)
        {
            dbserver = dbsm.GetSourceInfoByIndex(this.index);
            this.textBox1.Text = dbserver.ServerName;
            this.textBox2.Text = dbserver.LoginName;
            this.textBox3.Text = dbserver.Password;
            this.textBox4.Text = dbserver.DatabaseName;

        }
    }
}
