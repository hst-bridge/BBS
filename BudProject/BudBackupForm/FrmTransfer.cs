using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Common;
using Model;
using IBLL;
using log4net;
using System.Reflection;
using BudBackupSystem.util;

namespace BudBackupSystem
{
    public partial class FrmTransfer : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string id = "";
        private BackupServer backupServer = null;
        private bool gFlag = false;
        public OpaqueCommand OpaqueCommand = new OpaqueCommand();
        private int innerFlag = -1;
        private BackupServer innerBackupServer = new BackupServer();
        private BackupServerGroup innerBackupServerGroup = new BackupServerGroup();
        private IBackupServerService backupService = BLLFactory.ServiceAccess.CreateBackupServer();

        //バックアップ先対象グループ　サービス
        private readonly IBackupServerGroupService groupSerivce = BLLFactory.ServiceAccess.CreateBackupServerGroupService();

        private bool connectFlg;

        /// <summary>
        /// ip与开始文件夹对应关系
        /// </summary>
        private Dictionary<string, string> IP_StartFolder = null;

        public FrmTransfer()
        {
            InitializeComponent();
            cobMonitorServer_init();
            this.connectFlg = false;
            this.btnReference.Enabled = false;
        }
        public FrmTransfer(string id)
        {
            this.id = id;
            InitializeComponent();
            cobMonitorServer_init();
            this.connectFlg = false;
            this.btnReference.Enabled = false;
        }

        /// <summary>
        /// load MonitorServer dropdownlist
        /// </summary>
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
        /// init the backup server modifification page
        /// </summary>
        private void FrmTransfer_Load(object sender, EventArgs e)
        {
            //get monitor server by record id
            if (!String.IsNullOrWhiteSpace(id))
            {
                IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
                backupServer = bs.GetBackupServerById(Convert.ToInt32(id));
                this.txtTrId.Text = backupServer.id.ToString();
                this.txtTrName.Text = backupServer.backupServerName;
                this.txtTrIp.Text = backupServer.backupServerIP;
                this.txtTrMemo.Text = backupServer.memo;
                this.txtTraccount.Text = backupServer.account;
                this.txtTrPass.Text = backupServer.password;
                this.txtTrStfile.Text = backupServer.startFile;

                //Set the related MonitorServer.
                this.innerBackupServerGroup = groupSerivce.GetBackupServerGroupByBackupServerID(Convert.ToInt32(id));

                foreach (object item in this.cobMonitorServer.Items)
                {
                    ComboBoxItem cobItem = (ComboBoxItem)item;
                    if (cobItem.Value != null && cobItem.Value.Equals(this.innerBackupServerGroup.monitorServerID))
                    {
                        this.cobMonitorServer.SelectedItem = item;
                        break;
                    }
                }
                this.txtBackupServerGroupName.Text = this.innerBackupServerGroup.backupServerGroupName;
            }
            else
            {
                this.IP_StartFolder = CommonUtil.GetTransfer_IP_StartFolder();
            }
        }
        private bool messageCheck() 
        {
            bool flag = false;
            string nameMessage = ValidationRegex.ValidteEmpty(this.lblTrName.Text, this.txtTrName.Text);
            string ipMessage = ValidationRegex.ValidteEmpty(this.lblTrIp.Text, this.txtTrIp.Text);
            string accountMessage = ValidationRegex.ValidteEmpty(this.lblTrAccount.Text, this.txtTraccount.Text);
            string passMessage = ValidationRegex.ValidteEmpty(this.lblTrPass.Text, this.txtTrPass.Text);
            string stfileMessage = ValidationRegex.ValidteEmpty(this.lblTrStfile.Text, this.txtTrStfile.Text);
            Regex patt = new Regex("([1-9]|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])(\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])){3}");

            int bkId = 0;
            if (!String.IsNullOrWhiteSpace(this.id))
            {
                bkId = Convert.ToInt32(this.id);
            }
            //check Backup Server Name
            IList<BackupServer> existLists = null;
            if (!String.IsNullOrWhiteSpace(this.txtTrName.Text))
            {
                existLists = backupService.GetBackupServerListByNameButId(bkId, this.txtTrName.Text.Trim());
            }

            if (nameMessage != "")
            {
                MsgHelper.WarningMsg(nameMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (existLists != null && existLists.Count > 0)
            {
                string msg = ValidationRegex.W008;
                msg = msg.Replace("{1}", this.lblTrName.Text);
                MsgHelper.WarningMsg(msg, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (ipMessage != "")
            {
                MsgHelper.WarningMsg(ipMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (!patt.IsMatch(this.txtTrIp.Text.Trim()))
            {
                string msg = ValidationRegex.W003.Replace("{1}", this.lblTrIp.Text);
                MsgHelper.WarningMsg(msg, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (accountMessage != "")
            {
                MsgHelper.WarningMsg(accountMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (passMessage != "")
            {
                MsgHelper.WarningMsg(passMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (stfileMessage != "")
            {
                MsgHelper.WarningMsg(stfileMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else
            {
                //IP and startfolder unique checking
                IList<BackupServer> bkList = backupService.GetBackupServerListButId(bkId, this.txtTrIp.Text, this.txtTrStfile.Text);
                if (bkList.Count > 0)
                {
                    MsgHelper.WarningMsg(ValidationRegex.W009, ValidationRegex.publicTitle);
                    flag = true;
                }
                else
                {
                    //check whether the ip and startfolder is deleted.
                    IList<BackupServer> deletedbkList = backupService.GetDeletedBackupServerList(this.txtTrIp.Text, this.txtTrStfile.Text);
                    if (deletedbkList.Count > 0)
                    {
                        flag = !MsgHelper.QuestionMsg(ValidationRegex.W010, ValidationRegex.publicTitle);
                    }
                }
            }
            return flag;
        }
        private void saveOperation()
        {
            try
            {
                innerFlag = groupSerivce.InsertBackupServerGroup(this.innerBackupServerGroup);
                innerFlag = backupService.InsertBackupServer(innerBackupServer);
                //add relation detail
                IList<BackupServerGroup> groupList = groupSerivce.GetBackupServerGroupByName(this.innerBackupServerGroup.backupServerGroupName);
                IList<BackupServer> bkList = backupService.GetBackupServerListByName(innerBackupServer.backupServerName);
                if (groupList.Count > 0 && bkList.Count > 0)
                {
                    BackupServerGroupDetail modelDetail = new BackupServerGroupDetail();
                    modelDetail.backupServerGroupId = Convert.ToInt32(groupList[0].id);
                    modelDetail.backupServerId = Convert.ToInt32(bkList[0].id);
                    modelDetail.deleteFlg = 0;
                    modelDetail.creater = FrmMain.userinfo.loginID;
                    modelDetail.createDate = CommonUtil.DateTimeNowToString();
                    modelDetail.updater = FrmMain.userinfo.loginID;
                    modelDetail.updateDate = CommonUtil.DateTimeNowToString();

                    IBackupServerGroupDetailService groubDetailService = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
                    innerFlag = groubDetailService.InsertBackupServerGroupDetail(modelDetail);
                }
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
                innerFlag = groupSerivce.UpdateBackupServerGroup(this.innerBackupServerGroup);
                innerFlag = backupService.UpdateBackupServer(innerBackupServer);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool mesFlg = messageCheck();
            if (mesFlg) 
            {
                return;
            }
            else
            {
                string id = this.txtTrId.Text;
                //save operation
                //BackupServer backupServer = new BackupServer();
                this.innerBackupServerGroup.monitorServerID = this.cobMonitorServer.SelectedValue.ToString();
                this.innerBackupServerGroup.backupServerGroupName = innerBackupServer.backupServerName = this.txtTrName.Text.Trim();
                innerBackupServer.backupServerIP = this.txtTrIp.Text.Trim();
                this.innerBackupServerGroup.memo = innerBackupServer.memo = this.txtTrMemo.Text.Trim();
                innerBackupServer.account = this.txtTraccount.Text.Trim();
                innerBackupServer.password = this.txtTrPass.Text.Trim();
                innerBackupServer.startFile = this.txtTrStfile.Text.Trim();
                innerBackupServer.ssbpath = this.txtssbpath.Text.Trim();
                this.innerBackupServerGroup.deleteFlg = innerBackupServer.deleteFlg = 0;
                this.innerBackupServerGroup.creater = innerBackupServer.creater = FrmMain.userinfo.loginID;
                this.innerBackupServerGroup.createDate = innerBackupServer.createDate = CommonUtil.DateTimeNowToString();
                this.innerBackupServerGroup.updater = innerBackupServer.updater = FrmMain.userinfo.loginID;
                this.innerBackupServerGroup.updateDate = innerBackupServer.updateDate = CommonUtil.DateTimeNowToString();

                IBackupServerService backupService = BLLFactory.ServiceAccess.CreateBackupServer();
                //int flag = -1;
                if (id == "")
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
                else
                {
                    innerBackupServer.id = id;
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
            if ((id == null || id == "") && (this.txtTrName.Text.Trim() != ""
                || this.txtTrIp.Text.Trim() != ""
                || this.txtTrMemo.Text.Trim() != ""
                || this.txtTraccount.Text.Trim() != ""
                || this.txtTrPass.Text.Trim() != ""
                || this.txtTrStfile.Text.Trim() != ""))
            {
                if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                {
                    gFlag = true;
                }
            }
            else if ((id != null && id != "")
                && (backupServer.backupServerName != this.txtTrName.Text.Trim()
                || backupServer.backupServerIP != this.txtTrIp.Text.Trim()
                || backupServer.memo != this.txtTrMemo.Text.Trim()
                || backupServer.account != this.txtTraccount.Text.Trim()
                || backupServer.password != this.txtTrPass.Text.Trim()
                || backupServer.startFile != this.txtTrStfile.Text.Trim()))
            {
                if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                {
                    gFlag = true;
                }
            }
            else
            {
                gFlag = true;
            }
            if (gFlag) 
            {
                this.Dispose();
            }
        }

        private void FrmTransfer_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FrmTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gFlag == false) 
            {
                if ((id == null || id == "") && (this.txtTrName.Text.Trim() != ""
                || this.txtTrIp.Text.Trim() != ""
                || this.txtTrMemo.Text.Trim() != ""
                || this.txtTraccount.Text.Trim() != ""
                || this.txtTrPass.Text.Trim() != ""
                || this.txtTrStfile.Text.Trim() != ""))
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                    {
                        e.Cancel = false;
                    }
                    else 
                    {
                        e.Cancel = true;
                    }
                }
                else if ((id != null && id != "")
                    && (backupServer.backupServerName != this.txtTrName.Text.Trim()
                    || backupServer.backupServerIP != this.txtTrIp.Text.Trim()
                    || backupServer.memo != this.txtTrMemo.Text.Trim()
                    || backupServer.account != this.txtTraccount.Text.Trim()
                    || backupServer.password != this.txtTrPass.Text.Trim()
                    || backupServer.startFile != this.txtTrStfile.Text.Trim()))
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                    {
                        e.Cancel = false;
                    }
                    else 
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void txtTrStfile_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLinkTest_Click(object sender, EventArgs e)
        {
            this.btnLinkTest.Enabled = false;
            bool mesFlg = messageCheck();
            if (mesFlg)
            {
                this.btnLinkTest.Enabled = true;
                return;
            }
            NetWorkFileShare netWorkFileShare = new NetWorkFileShare();
            string serverFolderPath = @"\\" + this.txtTrIp.Text.Trim();
            if (netWorkFileShare.ConnectState(serverFolderPath, txtTraccount.Text.Trim(), txtTrPass.Text.Trim()))
            {
                MsgHelper.InfoMsg(ValidationRegex.C001, ValidationRegex.publicTitle);
                this.connectFlg = true;
                this.btnReference.Enabled = true;
            }
            else
            {
                MsgHelper.InfoMsg(ValidationRegex.C002, ValidationRegex.publicTitle);
                this.connectFlg = false;
                this.btnReference.Enabled = false;
            }
            this.btnLinkTest.Enabled = true;
        }

        private void btnReference_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            folderDialog.DisplayDialog();
            if (!String.IsNullOrEmpty(folderDialog.Path))
            {
                string[] pathList = folderDialog.Path.Split('\\');
                if (pathList.Count() > 3)
                {
                    string pathName = "";
                    for (int i = 3; i < pathList.Count(); i++)
                    {
                        pathName = pathName + pathList[i] + "\\";
                    }
                    this.txtTrStfile.Text = pathName.TrimEnd('\\');
                }
            }
        }

        /// <summary>
        /// 自动填充开始文件夹
        /// </summary>
        private void AutoFillStartFolder()
        {
            txtTrName.Text = cobMonitorServer.Text;

            if (this.IP_StartFolder != null)
            {
                string backupName = txtTrName.Text;
                string ip = txtTrIp.Text;
                txtTrStfile.ResetText();
                if (!String.IsNullOrWhiteSpace(backupName) && ip != "")
                {
                    string val = "";
                    this.IP_StartFolder.TryGetValue(ip, out val);
                    if (!String.IsNullOrWhiteSpace(val))
                    {
                        txtTrStfile.Text = val + "/" + backupName;
                    }
                }
            }
        }

        /// <summary>
        /// 失去焦点时，自动填充开始文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTrIp_Leave(object sender, EventArgs e)
        {
            AutoFillStartFolder();
        }

        /// <summary>
        /// 下拉菜单变化时，自动填充开始文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cobMonitorServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoFillStartFolder();
        }

    }
}
