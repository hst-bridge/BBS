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
using System.Windows.Forms.Design;
using BudBackupSystem.util;
using log4net;
using System.Reflection;
using System.Management;
using System.IO;

namespace BudBackupSystem
{
    public partial class FrmObject : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IMonitorServerService ms = BLLFactory.ServiceAccess.CreateMonitorServerService();
        public string id = "";
        private MonitorServer monitorServer = null;
        private bool gFlag = false;
        public OpaqueCommand OpaqueCommand = new OpaqueCommand();
        //public MacPath mp = new MacPath();
        private int innerFlag = -1;
        private MonitorServer innerMonitorServer = new MonitorServer();
        private IMonitorServerService monitorService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private IMonitorServerFolderService imsfs = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
        private IMonitorFileListenService monitorFileListenService = BLLFactory.ServiceAccess.CreateMonitorFileListenService();
        private bool connectFlg;
        public FrmObject()
        {
            InitializeComponent();
            this.connectFlg = false;
            this.btnLinkPath.Enabled = false;
        }

        public FrmObject(string id)
        {
            this.id = id;
            InitializeComponent();
            FrmObject_Load();
        }
        /// <summary>
        /// init the monitor server modifification page
        /// </summary>
        private void FrmObject_Load(object sender, EventArgs e) 
        {
            //get monitor server by record id
            if (!String.IsNullOrEmpty(id))
            {
                monitorServer = ms.GetMonitorServerById(Convert.ToInt32(id));
                this.txtId.Text = monitorServer.id.ToString();
                this.txtMonitorName.Text = monitorServer.monitorServerName;
                this.txtMonitorIp.Text = monitorServer.monitorServerIP;
                this.txtMonitorMemo.Text = monitorServer.memo;
                this.txtMonitorAccount.Text = monitorServer.account;
                this.txtMonitorPass.Text = monitorServer.password;
                this.txtMonitorStfile.Text = monitorServer.startFile;
                this.txtLocalPath.Text = monitorServer.monitorLocalPath;
                this.txtMacPath.Text = monitorServer.monitorMacPath;
                //
                if (monitorServer.copyInit == 1)
                {
                    this.checkBoxTopDirFile.Checked = true;
                }
                else
                {
                    this.checkBoxTopDirFile.Checked = false;
                }
            }
            else
            {
                this.txtLocalPath.Text = CommonUtil.GetLocalCopyPath();
            }
        }
        private bool messageCheck() 
        {
            bool flag = false;
            string nameMessage = ValidationRegex.ValidteEmpty(this.lblName.Text, this.txtMonitorName.Text);
            string ipMessage = ValidationRegex.ValidteEmpty(this.lblMonitorIp.Text, this.txtMonitorIp.Text);
            string accountMessage = ValidationRegex.ValidteEmpty(this.lblMonitorAccount.Text, this.txtMonitorAccount.Text);
            string passMessage = ValidationRegex.ValidteEmpty(this.lblMonitorPass.Text, this.txtMonitorPass.Text);
            string stfileMessage = ValidationRegex.ValidteEmpty(this.lblMonitorStfile.Text, this.txtMonitorStfile.Text);
            string localpathMessage = ValidationRegex.ValidteEmpty(this.lblLocalPath.Text, this.txtLocalPath.Text);
            // SSH エラーコピー対応 20140416
            string macMessage = ValidationRegex.ValidteEmpty(this.lblMacPath.Text, this.txtMacPath.Text);
            //string monitorDrive = ValidationRegex.ValidteEmpty(this.lblMonitorDrive.Text, this.cobMonitorDrive.Text);
            Regex patt = new Regex("([1-9]|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])(\\.(\\d|[1-9]\\d|1\\d{2}|2[0-4]\\d|25[0-5])){3}");
            if (nameMessage != "")
            {
                MsgHelper.WarningMsg(nameMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (ipMessage != "")
            {
                MsgHelper.WarningMsg(ipMessage, ValidationRegex.publicTitle);
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
            else if (localpathMessage != "")
            {
                MsgHelper.WarningMsg(localpathMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            else if (macMessage != "")
            {
                MsgHelper.WarningMsg(macMessage, ValidationRegex.publicTitle);
                flag = true;
            }
            //else if (monitorDrive != "")
            //{
            //    MsgHelper.WarningMsg(monitorDrive, ValidationRegex.publicTitle);
            //    flag = true;
            //}
            else if (!patt.IsMatch(this.txtMonitorIp.Text.Trim()))
            {
                string msg = ValidationRegex.W003.Replace("{1}", this.lblMonitorIp.Text);
                MsgHelper.WarningMsg(msg, ValidationRegex.publicTitle);
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// save operation
        /// </summary>
        private void saveOperation()
        {
            try
            {
                innerFlag = monitorService.InsertMonitorServer(innerMonitorServer);

                if (innerFlag > -1)
                {
                    int maxId = monitorService.GetMaxId();
                    string sb = innerMonitorServer.monitorDrive;

                    if (System.IO.File.Exists(sb))
                    {
                        FileInfo file = new FileInfo(sb);
                        if (file.Exists)
                        {
                            MonitorServerFolder msf = minitorFolderDataFormat(file, "1", maxId);
                            int innerId = imsfs.InsertMonitorServerFolder(msf);
                        }
                    }
                    else if (System.IO.Directory.Exists(sb))
                    {
                        DirectoryInfo di = new DirectoryInfo(sb);
                        MonitorServerFolder msf = minitorFolderDataFormat(di, "1", maxId);
                        int innerId = imsfs.InsertMonitorServerFolder(msf);
                        getChildFolderAndFile(di, maxId);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// update operation
        /// </summary>
        private void updateOperation()
        {
            try
            {
                innerFlag = monitorService.UpdateMonitorServer(innerMonitorServer);
                //
                if (innerFlag > -1)
                {
                    if (System.IO.Directory.Exists(innerMonitorServer.monitorDrive))
                    {
                        int resultForDeleteMonitorServerFolder = imsfs.DeleteMonitorServerFolderByServerId(Int32.Parse(innerMonitorServer.id));
                        if (resultForDeleteMonitorServerFolder > -1)
                        {
                            DirectoryInfo di = new DirectoryInfo(innerMonitorServer.monitorDrive);
                            MonitorServerFolder msf = minitorFolderDataFormat(di, "1", Int32.Parse(innerMonitorServer.id));
                            int innerId = imsfs.InsertMonitorServerFolder(msf);
                            //getChildFolderAndFile(di, maxId)
                        }
                    }
                    // 
                    int monitorFileListenForResult = monitorFileListenService.UpdateMonitorServer(innerMonitorServer, monitorServer);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// save or update button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool mesFlg = messageCheck();
            if (mesFlg) 
            {
                return;
            }
            else
            {
                string id = this.txtId.Text;
                //save operation
                //MonitorServer monitorServer = new MonitorServer();
                innerMonitorServer.monitorServerName = this.txtMonitorName.Text.Trim();
                innerMonitorServer.monitorServerIP = this.txtMonitorIp.Text.Trim();
                innerMonitorServer.monitorSystem = 1;
                innerMonitorServer.memo = this.txtMonitorMemo.Text.Trim();
                innerMonitorServer.account = this.txtMonitorAccount.Text.Trim();
                innerMonitorServer.password = this.txtMonitorPass.Text.Trim();
                innerMonitorServer.startFile = this.txtMonitorStfile.Text.Trim().TrimEnd('\\');
                innerMonitorServer.monitorDrive = "\\\\" + this.txtMonitorIp.Text.Trim() + "\\" + this.txtMonitorStfile.Text.Trim().TrimEnd('\\');
                innerMonitorServer.monitorDriveP = "";
                innerMonitorServer.monitorLocalPath = this.txtLocalPath.Text.Trim();
                innerMonitorServer.monitorMacPath = this.txtMacPath.Text.Trim();
                // Top Directory コピーするかどうか
                if (checkBoxTopDirFile.Checked)
                {
                    innerMonitorServer.copyInit = 1;
                }
                else
                {
                    innerMonitorServer.copyInit = 0;
                }
                innerMonitorServer.deleteFlg = 0;
                if (id == "")
                {
                    innerMonitorServer.creater = FrmMain.userinfo.loginID;
                    innerMonitorServer.createDate = CommonUtil.DateTimeNowToString();
                }
                else
                {
                    innerMonitorServer.creater = monitorServer.creater;
                    innerMonitorServer.createDate = monitorServer.createDate;
                }
                innerMonitorServer.updater = FrmMain.userinfo.loginID;
                innerMonitorServer.updateDate = CommonUtil.DateTimeNowToString();

                //IMonitorServerService monitorService = BLLFactory.ServiceAccess.CreateMonitorServerService();
                //IMonitorServerFolderService imsfs = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
                //int flag = -1;
                if (id == "")
                {
                    IList<MonitorServer> existLists = monitorService.GetMonitorServerListByName(this.txtMonitorName.Text.Trim());
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
                    innerMonitorServer.id = id;
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

        private void getChildFolderAndFile(DirectoryInfo sb, int id)
        {
            try
            {
                IMonitorServerFolderService imsfs = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
                if (sb is DirectoryInfo)
                {
                    FileSystemInfo[] fsis = sb.GetFileSystemInfos();
                    if (fsis.Count() > 0)
                    {
                        foreach (FileSystemInfo fsi in fsis)
                        {
                            if (fsi is FileInfo)
                            {
                                if (fsi.Exists)
                                {
                                    MonitorServerFolder msf = minitorFolderDataFormat(fsi, "0", id);
                                    int innerId = imsfs.InsertMonitorServerFolder(msf);
                                }
                            }
                            else
                            {
                                MonitorServerFolder msf = minitorFolderDataFormat(fsi, "0", id);
                                int innerId = imsfs.InsertMonitorServerFolder(msf);
                                getChildFolderAndFile((DirectoryInfo)fsi, id);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private MonitorServerFolder minitorFolderDataFormat(FileSystemInfo fileInfo, string initFlg, int id)
        {
            MonitorServerFolder msf = new MonitorServerFolder();
            // 開始フォルダーのパス
            string startPath = "\\\\" + this.txtMonitorIp.Text.Trim() + "\\" + this.txtMonitorStfile.Text.Trim().TrimEnd('\\');
            msf.monitorServerID = id;
            if (fileInfo is DirectoryInfo)
            {
                if (startPath.Equals(fileInfo.FullName.TrimEnd('\\')))
                {
                    msf.monitorFilePath = startPath;
                    msf.monitorFileName = "";
                }
                else
                {
                    msf.monitorFilePath = fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf("\\"));
                    msf.monitorFileName = fileInfo.Name;
                }
                msf.monitorFileType = "99";
                msf.monitorFlg = "0";
            }
            else
            {
                msf.monitorFileName = fileInfo.Name;
                int index = fileInfo.FullName.LastIndexOf("\\");
                string filePath = fileInfo.FullName.Substring(0, index);
                msf.monitorFilePath = filePath;
                msf.monitorFileType = fileInfo.Extension;
                msf.monitorFlg = "1";
            }
            msf.initFlg = initFlg;
            msf.monitorStatus = "未コピー";
            msf.createDate = CommonUtil.DateTimeNowToString();
            msf.creater = FrmLogin.userinfo.loginID;
            msf.updateDate = CommonUtil.DateTimeNowToString();
            msf.updater = FrmLogin.userinfo.loginID;
            return msf;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            int flg = 1;
            //shutdown the current page
            if ((id == null || id == "") && (this.txtMonitorName.Text.Trim() != "" 
                || this.txtMonitorIp.Text.Trim() != ""
                || this.txtMonitorMemo.Text.Trim() != ""
                || this.txtMonitorAccount.Text.Trim() != ""
                || this.txtMonitorPass.Text.Trim() != ""
                || this.txtMonitorStfile.Text.Trim() != ""
                || this.txtLocalPath.Text.Trim() != ""))
            {
                if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                {
                    gFlag = true;
                }
            }
            else if ((id != null && id != "")
                && (monitorServer.monitorServerName != this.txtMonitorName.Text.Trim()
                || monitorServer.monitorServerIP != this.txtMonitorIp.Text.Trim()
                || monitorServer.monitorSystem != flg
                || monitorServer.memo != this.txtMonitorMemo.Text.Trim()
                || monitorServer.account != this.txtMonitorAccount.Text.Trim()
                || monitorServer.password != this.txtMonitorPass.Text.Trim()
                || monitorServer.startFile != this.txtMonitorStfile.Text.Trim()
                || monitorServer.monitorLocalPath != this.txtLocalPath.Text.Trim()))
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

        private void FrmObject_FormClosed(object sender, FormClosedEventArgs e)
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
            string serverFolderPath = @"\\" + this.txtMonitorIp.Text.Trim();
            if (netWorkFileShare.ConnectState(serverFolderPath, txtMonitorAccount.Text.Trim(), txtMonitorPass.Text.Trim()))
            {
                MsgHelper.InfoMsg(ValidationRegex.C001, ValidationRegex.publicTitle);
                this.connectFlg = true;
                this.btnLinkPath.Enabled = true;
            }
            else
            {
                MsgHelper.InfoMsg(ValidationRegex.C002, ValidationRegex.publicTitle);
                this.connectFlg = false;
                this.btnLinkPath.Enabled = false;
            }
            this.btnLinkTest.Enabled = true;
        }

        private void btnLocalPath_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            if (folderDialog.DisplayDialog() == DialogResult.OK)
            {
                this.txtLocalPath.Text = folderDialog.Path;
            }
        }

        private void FrmObject_FormClosing(object sender, FormClosingEventArgs e)
        {
            int flg = 1;
            if (gFlag == false) 
            {
                if ((id == null || id == "") && (this.txtMonitorName.Text.Trim() != ""
                || this.txtMonitorIp.Text.Trim() != ""
                || this.txtMonitorMemo.Text.Trim() != ""
                || this.txtMonitorAccount.Text.Trim() != ""
                || this.txtMonitorPass.Text.Trim() != ""
                || this.txtMonitorStfile.Text.Trim() != ""
                || this.txtLocalPath.Text.Trim() != ""))
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
                    && (monitorServer.monitorServerName != this.txtMonitorName.Text.Trim()
                    || monitorServer.monitorServerIP != this.txtMonitorIp.Text.Trim()
                    || monitorServer.monitorSystem != flg
                    || monitorServer.memo != this.txtMonitorMemo.Text.Trim()
                    || monitorServer.account != this.txtMonitorAccount.Text.Trim()
                    || monitorServer.password != this.txtMonitorPass.Text.Trim()
                    || monitorServer.startFile != this.txtMonitorStfile.Text.Trim()
                    || monitorServer.monitorLocalPath != this.txtLocalPath.Text.Trim()))
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

        private void btnMonitorMacPath_Click(object sender, EventArgs e)
        {
            //bool mesFlg = messageCheck();
            //if (mesFlg)
            //{
            //    return;
            //}
            //else
            //{
            //    FrmMacPath frmMacPath = new FrmMacPath(this.txtMonitorIp.Text, 22, this.txtMonitorAccount.Text, this.txtMonitorPass.Text, mp);
            //    frmMacPath.FormClosed += new FormClosedEventHandler(this.Form_Closed);
            //    frmMacPath.ShowDialog();
            //}
        }
        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            //this.txtMonitorMacPath.Text = mp.MacPathString;
        }

        private void btnLinkPath_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            folderDialog.DisplayDialog();
            this.txtMonitorStfile.Text = folderDialog.Path;
        }

        private void FrmObject_Load()
        {

        }

        /// <summary>
        /// 共有ポイント Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMonitorStfile_TextChanged(object sender, EventArgs e)
        {
            this.txtLocalPath.Text = CommonUtil.GetLocalCopyPath() + this.txtMonitorStfile.Text;
        }
    }
}
