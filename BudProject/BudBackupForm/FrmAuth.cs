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

namespace BudBackupSystem
{
    public partial class FrmAuth : Form
    {
        public string id = "";
        public FrmAuth()
        {
            InitializeComponent();
        }
        public FrmAuth(string id)
        {
            this.id = id;
            InitializeComponent();
            FrmAuth_Load();
        }
        /// <summary>
        /// init the monitor server modifification page
        /// </summary>
        private void FrmAuth_Load()
        {
            //get user info by record id
            IUserInfoService us = BLLFactory.ServiceAccess.CreateUserInfoService();
            UserInfo usInfo = us.GetUserInfoById(Convert.ToInt32(id));
            this.txtAuthEditId.Text = usInfo.id.ToString();
            this.txtAuEditLoginId.Text = usInfo.loginID;
            this.txtAuEditLoginPass.Text = usInfo.password;
            this.txtAuEditMail.Text = usInfo.mail;
            this.txtAuEditName.Text = usInfo.name;
            if (usInfo.mailFlg == 1) { this.rbAuthMailYes.Checked = true; } else { this.rbAuthMailNo.Checked = true; }
            if (usInfo.authorityFlg == 1) { this.rbAuthFlgYes.Checked = true; } else { this.rbAuthFlgNo.Checked = true; }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAuthCancel_Click(object sender, EventArgs e)
        {
            //shutdown the current page
            this.Hide();
            FrmMain frmMain = new FrmMain();
            frmMain.ShowDialog();
        }

        private void btnAuthSave_Click(object sender, EventArgs e)
        {
            string loginIDMessage = ValidationRegex.ValidteEmpty(this.lblAuEditLoginId.Text, this.txtAuEditLoginId.Text);
            string passMessage = ValidationRegex.ValidteEmpty(this.lblAuEditLoginPass.Text, this.txtAuEditLoginPass.Text);
            string nameMessage = ValidationRegex.ValidteEmpty(this.lblAuEditName.Text, this.txtAuEditName.Text);
            string mailMessage = ValidationRegex.ValidteEmpty(this.lblAuEditMail.Text, this.txtAuEditMail.Text);
            if (loginIDMessage != "")
            {
                MsgHelper.WarningMsg(loginIDMessage, ValidationRegex.publicTitle);
                return;
            }
            else if (passMessage != "")
            {
                MsgHelper.WarningMsg(passMessage, ValidationRegex.publicTitle);
                return;
            }
            else if (nameMessage != "")
            {
                MsgHelper.WarningMsg(nameMessage, ValidationRegex.publicTitle);
                return;
            }
            else if (mailMessage != "")
            {
                MsgHelper.WarningMsg(mailMessage, ValidationRegex.publicTitle);
                return;
            }
            else
            {
                string id = this.txtAuthEditId.Text;
                //save operation
                UserInfo userInfo = new UserInfo();
                
                userInfo.loginID = this.txtAuEditLoginId.Text;
                userInfo.password = this.txtAuEditLoginPass.Text;
                userInfo.name = this.txtAuEditName.Text;
                userInfo.mail = this.txtAuEditMail.Text;
                if (this.rbAuthMailYes.Checked == true)
                {
                    userInfo.mailFlg = 1;
                }
                else 
                {
                    userInfo.mailFlg = 0;
                }
                if (this.rbAuthFlgYes.Checked == true)
                {
                    userInfo.authorityFlg = 1;
                }
                else
                {
                    userInfo.authorityFlg = 0;
                }
                userInfo.deleteFlg = 0;
                userInfo.creater = FrmMain.userinfo.loginID;
                userInfo.createDate = CommonUtil.DateTimeNowToString();
                userInfo.updater = FrmMain.userinfo.loginID;
                userInfo.updateDate = CommonUtil.DateTimeNowToString();

                IUserInfoService userInfoService = BLLFactory.ServiceAccess.CreateUserInfoService();

                int flag = -1;
                if (id == "")
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q001, ValidationRegex.publicTitle))
                    {
                        flag = userInfoService.InsertUserInfo(userInfo);
                    }
                    if (flag > -1)
                    {
                        MsgHelper.InfoMsg(ValidationRegex.I001, ValidationRegex.publicTitle);
                        this.Dispose();
                    }
                    else
                    {
                        MsgHelper.InfoMsg(ValidationRegex.I002, ValidationRegex.publicTitle);
                    }
                }
                else
                {
                    userInfo.id = id;
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q002, ValidationRegex.publicTitle))
                    {
                        flag = userInfoService.UpdateUserInfo(userInfo);
                    }
                    if (flag > -1)
                    {
                        MsgHelper.InfoMsg(ValidationRegex.U001, ValidationRegex.publicTitle);
                        this.Dispose();
                    }
                    else
                    {
                        MsgHelper.InfoMsg(ValidationRegex.U002, ValidationRegex.publicTitle);
                    }
                }
            }
        }

        private void FrmAuth_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
