using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Model;
using IBLL;
using BudBackupSystem.util;
using log4net;
using System.Reflection;

namespace BudBackupSystem
{
    public partial class FrmGroupTransfer : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string id = "";
        private BackupServerGroup backupServerGroup = null;
        private bool gFlag = false;
        public OpaqueCommand OpaqueCommand = new OpaqueCommand();
        private int innerFlag = -1;
        private BackupServerGroup innerBackupServerGroup = new BackupServerGroup();
        private IBackupServerGroupService backupService = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
        public FrmGroupTransfer()
        {
            InitializeComponent();
            cobMonitorServer_init();
        }
        public FrmGroupTransfer(string id)
        {
            this.id = id;
            InitializeComponent();
            cobMonitorServer_init();
            FrmGroupTransfer_Load();
        }
        private void cobMonitorServer_init() 
        {
            IMonitorServerService ms = BLLFactory.ServiceAccess.CreateMonitorServerService();
            IList<MonitorServer> msLists = ms.GetMonitorServerList();
            List<ComboBoxItem> cbiList = new List<ComboBoxItem>();
            foreach (MonitorServer msInfo in msLists)
            {
                cbiList.Add(new ComboBoxItem(msInfo.id, msInfo.monitorServerName));
            }
            this.cobMonitorServer.DisplayMember = "Text";
            this.cobMonitorServer.ValueMember = "Value";
            this.cobMonitorServer.DataSource = cbiList;
        }
        /// <summary>
        /// init the monitor server modifification page
        /// </summary>
        private void FrmGroupTransfer_Load(object sender, EventArgs e) 
        {
            //get monitor server by record id
            IBackupServerGroupService bs = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
            backupServerGroup = bs.GetBackupServerGroupById(Convert.ToInt32(id));
            this.txtMonitorId.Text = backupServerGroup.id.ToString();
            this.txtMonitorName.Text = backupServerGroup.backupServerGroupName;
            this.txtMonitorMemo.Text = backupServerGroup.memo;
            this.cobMonitorServer.SelectedValue = backupServerGroup.monitorServerID;
        }
        private void saveOperation()
        {
            try
            {
                innerFlag = backupService.InsertBackupServerGroup(innerBackupServerGroup);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private void updateOperation()
        {
            try
            {
                innerFlag = backupService.UpdateBackupServerGroup(innerBackupServerGroup);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string nameMessage = ValidationRegex.ValidteEmpty(this.lblName.Text, this.txtMonitorName.Text);
            string serverMessage = ValidationRegex.ValidteEmpty(this.lblServer.Text, this.cobMonitorServer.Text);

           if (nameMessage != "")
            {
                MsgHelper.WarningMsg(nameMessage, ValidationRegex.publicTitle);
                return;
            }
           else if (serverMessage != "") 
            {
                MsgHelper.WarningMsg(serverMessage, ValidationRegex.publicTitle);
                return;
            }
           else 
            {
                string id = this.txtMonitorId.Text;
                //save operation
                //BackupServerGroup backupServerGroup = new BackupServerGroup();

                innerBackupServerGroup.backupServerGroupName = this.txtMonitorName.Text.Trim();
                innerBackupServerGroup.monitorServerID = this.cobMonitorServer.SelectedValue.ToString();
                innerBackupServerGroup.memo = this.txtMonitorMemo.Text.Trim();
                innerBackupServerGroup.deleteFlg = 0;
                innerBackupServerGroup.creater = FrmMain.userinfo.loginID;
                innerBackupServerGroup.createDate = CommonUtil.DateTimeNowToString();
                innerBackupServerGroup.updater = FrmMain.userinfo.loginID;
                innerBackupServerGroup.updateDate = CommonUtil.DateTimeNowToString();

                //IBackupServerGroupService backupService = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
                //int flag = -1;
                if (id == "")
                {
                    IList<BackupServerGroup> existLists = backupService.GetBackupServerGroupByName(this.txtMonitorName.Text.Trim());
                    if (existLists.Count > 0)
                    {
                        string msg = ValidationRegex.W008;
                        msg = msg.Replace("{1}", this.lblName.Text);
                        MsgHelper.WarningMsg(msg, ValidationRegex.publicTitle);
                    }
                    else
                    {
                        if (MsgHelper.QuestionMsg(ValidationRegex.Q001, ValidationRegex.publicTitle))
                        {
                            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(saveOperation));
                        }
                        if (innerFlag > -1)
                        {
                            MsgHelper.InfoMsg(ValidationRegex.I001, ValidationRegex.publicTitle);
                            gFlag = true;
                            this.Dispose();
                        }
                    }
                }
                else
                {
                    innerBackupServerGroup.id = id;
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q002, ValidationRegex.publicTitle))
                    {
                        ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(updateOperation));
                    }
                    if (innerFlag > -1)
                    {
                        MsgHelper.InfoMsg(ValidationRegex.U001, ValidationRegex.publicTitle);
                        gFlag = true;
                        this.Dispose();
                    }
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //shutdown the current page
            if (hasBeenOpened())
            {
                this.Dispose();
            }
        }

        private void FrmGroupTransfer_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FrmGroupTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    e.Cancel = !hasBeenOpened(); //×ボタン：ユーザーインターフェイスによる
                    break;
                case CloseReason.None:          //閉じるボタン
                default:
                    e.Cancel = false;
                    break;
            }
        }
        
        /// <summary>
        /// 入力値が変更され画面が閉じられ用としたときに、注意を促し、閉じるかどうかを返す
        /// </summary>
        /// <returns>画面を閉じる場合はtrue</returns>
        private bool hasBeenOpened()
        {
            //shutdown the current page
            //新規入力：入力された場合
            if ((string.IsNullOrEmpty(id)
                && (   !string.IsNullOrWhiteSpace(this.txtMonitorName.Text)
                    || !string.IsNullOrWhiteSpace(this.txtMonitorMemo.Text)
                    || this.cobMonitorServer.SelectedIndex > 1
                    )
                 ) 
            //保存済みの値の変更：値が変更された場合
            || (!string.IsNullOrEmpty(id)
                && (   backupServerGroup.backupServerGroupName != this.txtMonitorName.Text.Trim()
                    || backupServerGroup.memo != this.txtMonitorMemo.Text.Trim()
                    || backupServerGroup.monitorServerID != (this.cobMonitorServer.SelectedIndex>=0 ? this.cobMonitorServer.SelectedValue.ToString() : string.Empty)
                    )
                )
            )
            {
                //確認して閉じてもOKと答えられた場合 true
                return MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle);
            }
            else
            {
                return true;
            }
        }

        private void FrmGroupTransfer_Load()
        {

        }

    }
}
