using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
using Common;
using IBLL;
using log4net;
using System.Reflection;
using System.Data.Odbc;

namespace BudBackupSystem
{
    public partial class FrmLogin : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static UserInfo userinfo;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //messageBox title
            string loginIDError = ValidationRegex.ValidteEmpty(this.lblLoginId.Text, this.txtLoginId.Text);
            string passwordError = ValidationRegex.ValidteEmpty(this.lblLoginPass.Text, this.txtLoginPass.Text);
            if (loginIDError != "")
            {
                MsgHelper.WarningMsg(loginIDError, ValidationRegex.publicTitle);
                return;
            }
            else if (passwordError!= "")
            {
                MsgHelper.WarningMsg(passwordError, ValidationRegex.publicTitle);
                return;
            }

            IUserInfoService uiService = BLLFactory.ServiceAccess.CreateUserInfoService();
            UserInfo userInfo = new UserInfo();
            userInfo.loginID = this.txtLoginId.Text;
            userInfo.password = this.txtLoginPass.Text;
            UserInfo us;

            try
            {
                us = uiService.userExist(userInfo);
                if (us != null)
                {
                    userinfo = us;
                    logger.Info(us.loginID + "ログイン成功。");
                    FrmMain.userinfo = us;
                    this.Dispose();
                    FrmMain mainForm = new FrmMain();
                    mainForm.ShowDialog();
                }
                else
                {
                    logger.Info("ログイン失敗。" + ValidationRegex.I003);
                    MsgHelper.WarningMsg(ValidationRegex.I003, ValidationRegex.publicTitle);
                }
            }
            catch (OdbcException ex)
            {
                // 例外処理
                MsgHelper.WarningMsg(ValidationRegex.I004, ValidationRegex.publicTitle);
            }
            catch (Exception ex)
            {
                // 例外処理
                MsgHelper.WarningMsg(ValidationRegex.I005, ValidationRegex.publicTitle);
            }

           
        }

        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void txtLoginId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                txtLoginPass.Focus();
            }
            
        }

        private void txtLoginPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
