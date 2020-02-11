using BudBackupSystem.util;
using Common;
using DevExpress.XtraTreeList.Nodes;
using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace BudBackupSystem
{
    public partial class FrmMain : Form
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static IList<MonitorServerFolder> lists = new List<MonitorServerFolder>();
        private static List<MonitorServerFolder> msfListIndex = new List<MonitorServerFolder>();
        //private static IList<FileTypeSet> ftsLists = null;
        private static FileTypeSet ftsList = null;
        private static bool doFlag = true;
        public static Form mainForm = null;
        public static UserInfo userinfo;
        public OpaqueCommand OpaqueCommand = new OpaqueCommand();
        private static bool loadFlg = true;

        //Declare Service
        private IMonitorServerService imsSvc = null;
        private IMonitorServerFolderService imsfSvc = null;
        private IFileTypeSetService iftsSvc = null;
        /// <summary>
        /// SynchronizingTime
        /// </summary>
        private int SynchronizingTime = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["RealTime"]);

        public FrmMain()
        {
            FrmLogin fl = new FrmLogin();
            if (FrmLogin.userinfo == null)
            {
                fl.ShowDialog();
            }
            else
            {
                fl.FormClosed += new FormClosedEventHandler(this.Form_Closed);
            }
            InitializeComponent();
            if (userinfo != null && userinfo.authorityFlg != 1)
            {
                //set tabPage hidden
                this.tabPage4.Parent = null;
                this.tabPage6.Parent = null;
            }
        }
        public FrmMain(int pageIndex)
        {
            InitializeComponent();
            if (userinfo != null && userinfo.authorityFlg != 1)
            {
                //set tabPage hidden
                this.tabPage4.Parent = null;
                this.tabPage6.Parent = null;
            }
            this.tbOverlay.SelectedIndex = pageIndex;

        }
        public void FrmMain_Load(object sender, EventArgs e)
        {
            //Create Service
            this.imsSvc = BLLFactory.ServiceAccess.CreateMonitorServerService();
            this.imsfSvc = BLLFactory.ServiceAccess.CreateMonitorServerFolderService();
            this.iftsSvc = BLLFactory.ServiceAccess.CreateFileTypeSetService();

            //init the dgrdMain
            tabPage1_Load();
            //tabPage2_Load();
            //tabPage3_Load();
            //tabPage4_Load();
            //tabPage5_Load();
            //tabPage6_Load();
        }
        private void tabPage1_Load()
        {
            //init dgrdMain 
            //not allowed user add new row to datagridview
            this.dgrdMain.AllowUserToAddRows = false;
            //this.dgrdMain.AllowUserToResizeColumns = false;
            this.dgrdMain.AllowUserToResizeRows = false;
            this.dgrdMain.Rows.Clear();

            //init table data
            IList<MonitorServer> lists = this.imsSvc.GetMonitorServerList();
            int i = 1;
            foreach (MonitorServer list in lists)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdMain.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = list.id;
                dgvr.Cells[1].Value = i.ToString();
                dgvr.Cells[2].Value = list.monitorServerName;
                dgvr.Cells[3].Value = list.monitorServerIP;
                string monitorSystem = list.monitorSystem == 1 ? "MAC環境" : "WINDOWS環境";
                dgvr.Cells[4].Value = monitorSystem;
                dgvr.Cells[5].Value = list.memo;
                dgvr.Cells[6].Value = "有効";
                dgvr.Cells[7].Value = "編集";
                dgvr.Cells[8].Value = "削除";
                this.dgrdMain.Rows.Add(dgvr);
                i++;
            }
        }
        private void tabPage2_Load(object monitorServerId = null)
        {
            IList<MonitorServer> msLists = this.imsSvc.GetMonitorServerList();
            List<ComboBoxItem> cbiList = new List<ComboBoxItem>();
            foreach (MonitorServer ms in msLists)
            {
                cbiList.Add(new ComboBoxItem(ms.id, ms.monitorServerName));
            }
            this.cobMonitorServerList.DisplayMember = "Text";
            this.cobMonitorServerList.ValueMember = "Value";
            this.cobMonitorServerList.DataSource = cbiList;
            if (monitorServerId != null)
            {
                this.cobMonitorServerList.SelectedValue = monitorServerId;
            }
        }

        private void tabPage3_Load()
        {
            //init dgrdBackupServer 
            //not allowed user add new row to datagridview
            this.dgrdBackupServer.AllowUserToAddRows = false;
            //this.dgrdBackupServer.AllowUserToResizeColumns = false;
            this.dgrdBackupServer.AllowUserToResizeRows = false;
            this.dgrdBackupServer.Rows.Clear();

            //init table data
            IBackupServerService ts = BLLFactory.ServiceAccess.CreateBackupServer();
            IList<BackupServer> lists = ts.GetBackupServerList();
            int i = 1;
            foreach (BackupServer list in lists)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdBackupServer.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = list.id;
                dgvr.Cells[1].Value = i.ToString();
                dgvr.Cells[2].Value = list.backupServerName;
                dgvr.Cells[3].Value = list.backupServerIP;
                dgvr.Cells[4].Value = list.memo;
                dgvr.Cells[5].Value = "有効";
                dgvr.Cells[6].Value = "編集";
                dgvr.Cells[7].Value = "削除";
                this.dgrdBackupServer.Rows.Add(dgvr);
                i++;
            }
        }
        private void tabPage4_Load()
        {
            //init dgrdBackupServer 
            //not allowed user add new row to datagridview
            this.dgrdBKServerGroup.AllowUserToAddRows = false;
            this.dgrdBKServerGroup.AllowUserToResizeColumns = false;
            this.dgrdBKServerGroup.AllowUserToResizeRows = false;
            this.dgrdBKServerGroup.Rows.Clear();

            //init table data
            IBackupServerGroupService ts = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
            IList<BackupServerGroup> lists = ts.GetBackupServerGroupList();
            int i = 1;
            foreach (BackupServerGroup list in lists)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdBKServerGroup.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = list.id;
                dgvr.Cells[1].Value = i.ToString();
                dgvr.Cells[2].Value = list.backupServerGroupName;
                dgvr.Cells[3].Value = list.memo;
                dgvr.Cells[4].Value = "有効";
                dgvr.Cells[5].Value = "編集";
                dgvr.Cells[6].Value = "削除";
                dgvr.Cells[7].Value = "設定";
                this.dgrdBKServerGroup.Rows.Add(dgvr);
                i++;
            }
        }
        private void tabPage5_Load()
        {
            //init the log page
            this.pnlTrLog.Visible = true;
            this.pnlTrVolumn.Visible = false;
            IBackupServerGroupService ibsgs = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
            IList<BackupServerGroup> bsgList = ibsgs.GetBackupServerGroupList();

            List<ComboBoxItem> cbiList = new List<ComboBoxItem>();
            cbiList.Add(new ComboBoxItem("-1", "すべて"));
            foreach (BackupServerGroup bgs in bsgList)
            {
                cbiList.Add(new ComboBoxItem(bgs.id.ToString(), bgs.backupServerGroupName));
            }
            this.cobBackupServerGroupName.DisplayMember = "Text";
            this.cobBackupServerGroupName.ValueMember = "Value";
            this.cobBackupServerGroupName.DataSource = cbiList;

            this.dtpStartDate.Value = DateTime.Now;
            this.dtpEndDate.Value = DateTime.Now;

            //get the log List
            if (this.rbtnOpint7.Checked)
            {
                dgrdLogDataFormat();
            }
            else
            {
                this.rbtnOpint7.Checked = true;
            }

            if (rbtnOpint1.Checked)
            {
                timerRealTimeDo.Interval = SynchronizingTime;
                timerRealTimeDo.Elapsed += new System.Timers.ElapsedEventHandler(SynchronizingWatchStart);
                timerRealTimeDo.Start();
            }
        }

        /// <summary>
        /// リアルタイム同期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynchronizingWatchStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 同期のTIMER中止
            timerRealTimeDo.Enabled = false;
            conditionSearch();

            timerRealTimeDo.Enabled = true;
        }

        private void tabPage6_Load()
        {
            //init dgrdAuthority 
            //not allowed user add new row to datagridview
            this.dgrdAuthority.AllowUserToAddRows = false;
            this.dgrdAuthority.AllowUserToResizeColumns = false;
            this.dgrdAuthority.AllowUserToResizeRows = false;
            this.dgrdAuthority.Rows.Clear();

            //init table data
            IUserInfoService us = BLLFactory.ServiceAccess.CreateUserInfoService();
            IList<UserInfo> lists = us.GetUserInfoList();
            int i = 1;
            foreach (UserInfo list in lists)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdAuthority.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = list.id;
                dgvr.Cells[1].Value = i.ToString();
                dgvr.Cells[2].Value = list.loginID;
                dgvr.Cells[3].Value = list.name;
                dgvr.Cells[4].Value = list.mail;
                dgvr.Cells[5].Value = "編集";
                dgvr.Cells[6].Value = "削除";
                this.dgrdAuthority.Rows.Add(dgvr);
                i++;
            }
        }
        private void buttonListenSave_Click(object sender, EventArgs e)
        {

        }
        private void buttonAddListen_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dgrdMain.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    int CIndex = e.ColumnIndex;
                    //get the column index 
                    //get the monitor server record id 
                    string id = dgrdMain[0, e.RowIndex].Value.ToString();
                    if (CIndex == 7)
                    {
                        this.Hide();
                        FrmObject fo = new FrmObject(id);
                        fo.FormClosed += new FormClosedEventHandler(this.Form_Closed);
                        fo.ShowDialog();
                    }
                    if (CIndex == 8)
                    {
                        if (MsgHelper.QuestionMsg(ValidationRegex.Q004, ValidationRegex.publicTitle))
                        {
                            try
                            {
                                int flag = this.imsSvc.DeleteMonitorServer(Convert.ToInt32(id), FrmLogin.userinfo.loginID);
                                if (flag > -1)
                                {
                                    int delFlag = this.imsfSvc.DeleteMonitorServerFolderByServerId(Convert.ToInt32(id));
                                    MsgHelper.InfoMsg(ValidationRegex.D001, ValidationRegex.publicTitle);
                                    tabPage1_Load();
                                    loadFlg = true;
                                }
                                else
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D002, ValidationRegex.publicTitle);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        }
                    }
                }
            }
        }
        private void dgrdBackupServer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dgrdBackupServer.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    int CIndex = e.ColumnIndex;
                    //get the column index 
                    //get the monitor server record id 
                    string id = dgrdBackupServer[0, e.RowIndex].Value.ToString();
                    if (CIndex == 6)
                    {
                        this.Hide();
                        FrmTransfer fo = new FrmTransfer(id);
                        fo.FormClosed += new FormClosedEventHandler(this.Form_Closed);
                        fo.ShowDialog();
                    }
                    if (CIndex == 7)
                    {
                        if (MsgHelper.QuestionMsg(ValidationRegex.Q004, ValidationRegex.publicTitle))
                        {
                            try
                            {
                                int bsId = Convert.ToInt32(id);
                                IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
                                int flag = bs.DeleteBackupServer(bsId, FrmLogin.userinfo.loginID);

                                //転送先対象グループ　サービス
                                IBackupServerGroupService groupSerivce = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
                                BackupServerGroup backupServerGroup = groupSerivce.GetBackupServerGroupByBackupServerID(bsId);
                                flag = groupSerivce.DeleteBackupServerGroup(Convert.ToInt32(backupServerGroup.id), FrmLogin.userinfo.loginID);

                                //delete relation detail
                                //転送先対象グループ明細　サービス
                                IBackupServerGroupDetailService groubDetailService = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
                                flag = groubDetailService.DeleteBackupServerGroupDetail(bsId, Convert.ToInt32(backupServerGroup.id), FrmLogin.userinfo.loginID);

                                if (flag > -1)
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D001, ValidationRegex.publicTitle);
                                    tabPage3_Load();
                                }
                                else
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D002, ValidationRegex.publicTitle);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        }
                    }
                }
            }
        }
        private void dgrdBKServerGroup_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dgrdBKServerGroup.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    int CIndex = e.ColumnIndex;
                    //get the column index 
                    //get the monitor server record id 
                    string id = dgrdBKServerGroup[0, e.RowIndex].Value.ToString();
                    if (CIndex == 5)
                    {
                        this.Hide();
                        FrmGroupTransfer fo = new FrmGroupTransfer(id);
                        fo.FormClosed += new FormClosedEventHandler(this.Form_Closed);
                        fo.ShowDialog();
                    }
                    if (CIndex == 6)
                    {
                        if (MsgHelper.QuestionMsg(ValidationRegex.Q004, ValidationRegex.publicTitle))
                        {
                            try
                            {
                                IBackupServerGroupService bs = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
                                int flag = bs.DeleteBackupServerGroup(Convert.ToInt32(id), FrmLogin.userinfo.loginID);
                                if (flag > -1)
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D001, ValidationRegex.publicTitle);
                                    tabPage4_Load();
                                }
                                else
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D002, ValidationRegex.publicTitle);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        }
                    }
                    //if (CIndex == 7)
                    //{
                    //    this.Hide();
                    //    FrmGroupDetail fo = new FrmGroupDetail(id);
                    //    fo.FormClosed += new FormClosedEventHandler(this.Form_Closed);
                    //    fo.ShowDialog();
                    //}
                }
            }
        }
        private void dgrdAuthority_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dgrdAuthority.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    int CIndex = e.ColumnIndex;
                    //get the column index 
                    //get the monitor server record id 
                    string id = dgrdAuthority[0, e.RowIndex].Value.ToString();
                    if (CIndex == 5)
                    {
                        this.Hide();
                        FrmAuth fa = new FrmAuth(id);
                        fa.ShowDialog();
                    }
                    if (CIndex == 6)
                    {
                        if (MsgHelper.QuestionMsg(ValidationRegex.Q004, ValidationRegex.publicTitle))
                        {
                            try
                            {
                                IUserInfoService us = BLLFactory.ServiceAccess.CreateUserInfoService();
                                int flag = us.DeleteUserInfo(Convert.ToInt32(id), FrmLogin.userinfo.loginID);
                                if (flag > -1)
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D001, ValidationRegex.publicTitle);
                                    tabPage6_Load();
                                }
                                else
                                {
                                    MsgHelper.InfoMsg(ValidationRegex.D002, ValidationRegex.publicTitle);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// tab controll select init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            try
            {
                if (e.TabPage == this.tabPage1)
                {
                    tabPage1_Load();
                }
                if (e.TabPage == this.tabPage2)
                {
                    if (loadFlg)
                    {
                        tabPage2_Load();
                        loadFlg = false;
                    }
                    else
                    {
                        this.tabPage2.Show();
                    }
                }
                if (e.TabPage == this.tabPage3)
                {
                    tabPage3_Load();
                }
                if (e.TabPage == this.tabPage4)
                {
                    tabPage4_Load();
                }
                if (e.TabPage == this.tabPage5)
                {
                    tabPage5_Load();
                }
                if (e.TabPage == this.tabPage6)
                {
                    tabPage6_Load();
                }
                if (e.TabPage == this.tabPage7)
                {
                    tabPage7_Load();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void btnMonitorServer_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmObject frmObj = new FrmObject();
            frmObj.FormClosed += new FormClosedEventHandler(this.Form_Closed);
            frmObj.ShowDialog();
            loadFlg = true;
        }
        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            tabPage1_Load();
            tabPage2_Load(this.cobMonitorServerList.SelectedValue);
            tabPage3_Load();
            tabPage4_Load();
            this.Show();
        }
        private void btnBackupServer_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmTransfer frmtran = new FrmTransfer();
            frmtran.FormClosed += new FormClosedEventHandler(this.Form_Closed);
            frmtran.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmGroupTransfer frmgroupTran = new FrmGroupTransfer();
            frmgroupTran.FormClosed += new FormClosedEventHandler(this.Form_Closed);
            frmgroupTran.ShowDialog();
        }
        /// <summary>
        /// treeList int method
        /// </summary>
        private void treeListInit()
        {
            try
            {
                //DataTable dt = new DataTable();
                //dt.Columns.Add("ID", typeof(string));
                //dt.Columns.Add("FileName", typeof(string));
                //dt.Columns.Add("Size", typeof(string));
                //dt.Columns.Add("UpdateTime", typeof(string));
                //dt.Columns.Add("NodePath", typeof(string));
                //dt.Columns.Add("ParentID", typeof(string));

                DataTable dt1 = new DataTable();
                dt1.Columns.Add("ID", typeof(string));
                dt1.Columns.Add("Name", typeof(string));
                dt1.Columns.Add("ParentID", typeof(string));
                //get the root path from the system
                if (this.cobMonitorServerList.SelectedValue != null)
                {
                    string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
                    MonitorServer ms = this.imsSvc.GetMonitorServerById(Int32.Parse(monitorServerId));
                    //string root = System.IO.Directory.GetDirectoryRoot(ms.monitorDrive+":");
                    //dt1.Rows.Add("tr1_" + 1, root, 0);
                    //dt1.Rows.Add("tr1_" + 1, root.Replace("\\", ""), 0);
                    // 20140321 wang web対応
                    //string root = ms.monitorDrive;
                    string root = @"\\" + ms.monitorServerIP + @"\" + ms.startFile.TrimEnd('\\').TrimStart('\\');
                    //dt1.Rows.Add("tr1_" + 1, root, 0);
                    dt1.Rows.Add("tr1_" + 1, root, 0);
                }
                this.treeList1.DataSource = dt1;
                if (this.treeList1.Nodes.FirstNode != null)
                {
                    this.treeList1.Nodes.FirstNode.Expanded = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// treeList1 執行クリック(画面表示する前に)
        /// 順番：一番
        /// </summary>
        private void treeList1_BeforeFocusNode(object sender, DevExpress.XtraTreeList.BeforeFocusNodeEventArgs e)
        {
            if (e.Node == null || this.cobMonitorServerList.SelectedValue == null) return;

            #region
            //get the name and id of the selected node
            string name = e.Node.GetValue("Name").ToString();
            string id = Convert.ToString(e.Node.GetValue("ID"));
            //get the directory of the current node
            DevExpress.XtraTreeList.Nodes.TreeListNode node = e.Node;

            string sb = getCurrentNodePath(node);
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();

            DevExpress.XtraTreeList.TreeList tr = sender as DevExpress.XtraTreeList.TreeList;
            if (tr != null)
            {
                if (System.IO.Directory.Exists(sb))
                {
                    try
                    {
                        //the left treelist datasource
                        DataTable dt1 = tr.DataSource as DataTable;

                        DirectoryInfo di = new DirectoryInfo(sb);
                        //文件夹
                        DirectoryInfo[] subDirs = di.GetDirectories();
                        for (int i = 0; i < subDirs.Length; i++)
                        {
                            string subName = subDirs[i].Name.Replace("'", "");
                            string dirPath = subDirs[i].FullName;
                            string condition = "Name='" + subDirs[i].Name + "' and ParentID='" + id + "'";

                            if (dt1.Select(condition).Count() <= 0)
                                dt1.Rows.Add(id + "_" + i, subDirs[i].Name, id);
                        }

                        //文件
                        FileInfo[] subFiles = di.GetFiles();
                        for (int i = 0; i < subFiles.Length; i++)
                        {
                            string path = subFiles[i].FullName.Substring(0, subFiles[i].FullName.LastIndexOf("\\"));
                            FileTypeSet fileTypeSet = iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(monitorServerId, path);

                            if (fileTypeSet.systemFileFlg == "1")
                            {
                                if ((subFiles[i].Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                            if (fileTypeSet.hiddenFileFlg == "1")
                            {
                                if ((subFiles[i].Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }

                            //if (fileTypeSet.exceptAttribute1 != null && fileTypeSet.exceptAttribute1 != ""
                            //    && subFiles[i].Extension == fileTypeSet.exceptAttribute1 && fileTypeSet.exceptAttributeFlg1 == "1")
                            //{
                            //    continue;
                            //}
                            //if (fileTypeSet.exceptAttribute2 != null && fileTypeSet.exceptAttribute2 != ""
                            //    && subFiles[i].Extension == fileTypeSet.exceptAttribute2 && fileTypeSet.exceptAttributeFlg2 == "1")
                            //{
                            //    continue;
                            //}
                            //if (fileTypeSet.exceptAttribute3 != null && fileTypeSet.exceptAttribute3 != ""
                            //    && subFiles[i].Extension == fileTypeSet.exceptAttribute3 && fileTypeSet.exceptAttributeFlg3 == "1")
                            //{
                            //    continue;
                            //}
                            string subName = subFiles[i].Name.Replace("'", "");
                            string dirPath = subFiles[i].FullName;
                            string condition = "Name='" + subFiles[i].Name + "' and ParentID='" + id + "'";

                            if (dt1.Select(condition).Count() <= 0)
                                dt1.Rows.Add(id + "_" + i, subFiles[i].Name, id);
                        }

                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        logger.Error(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
            }
            if (lists.Count == 0)
            {
                IList<MonitorServerFolder> msfLists = this.imsfSvc.GetMonitorFolderByServerIDAndInitFlg(monitorServerId);
                lists = msfLists;
            }
            if (doFlag)
            {
                doFlag = false;
                getAndInitPageDefault();
            }

            //在点击Checkbox时执行过滤，所以不再次过滤——2014-7-15 wjd commented
            //formatListsByFileTypeSet();

            init_nodeCheck(this.treeList1.Nodes.FirstNode, true);
            if (this.treeList1.Nodes.FirstNode != null)
            {
                this.treeList1.Nodes.FirstNode.Expanded = true;
            }


            #endregion
        }

        /// <summary>
        /// treeList1 執行クリック(画面表示する前に)
        /// 順番：二番
        /// </summary>
        private void treeList1_BeforeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e)
        {
            if (e.Node.CheckState == CheckState.Checked)
            {
                if (e.Node.HasChildren)
                {
                    foreach (TreeListNode tln in e.Node.Nodes)
                    {
                        tln.CheckState = CheckState.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// treeList1 執行クリック　(画面表示する前に)
        /// 順番：三番また四番
        /// </summary>
        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            //if (this.treeList1.FocusedNode != null && this.treeList1.FocusedNode.Checked == true)
            //{
            //    this.treeList2.CheckAll();
            //}
            //string sb = getCurrentNodePath(this.treeList1.FocusedNode);
            //if (this.treeList1.FocusedNode != null)
            //{
            //    foreach (TreeListNode tln in this.treeList2.Nodes)
            //    {
            //        string name = tln.GetValue("FileName").ToString();
            //        foreach (MonitorServerFolder msfl in lists)
            //        {
            //            if (msfl.monitorFileName == name && msfl.monitorFilePath == sb)
            //            {
            //                tln.Checked = true;
            //            }
            //        }
            //        //foreach (TreeListNode tn in this.treeList1.FocusedNode.Nodes) 
            //        //{
            //        //    string nodePath = getCurrentNodePath(tn);
            //        //    string name = tn.GetValue("Name").ToString();
            //        //    foreach (MonitorServerFolder msfl in lists)
            //        //    {= 
            //        //        string filePath = msfl.monitorFilePath + "\\" + msfl.monitorFileName;
            //        //        if (nodePath == filePath)
            //        //        {
            //        //            tn.Checked = true;
            //        //        }
            //        //        else if (nodePath != filePath && filePath.IndexOf(nodePath) > -1 && msfl.monitorFileType == "99") 
            //        //        {
            //        //            tn.CheckState = CheckState.Indeterminate;
            //        //        }
            //        //    }
            //        //}
            //    }
            //}
        }

        /// <summary>
        /// treeList1 執行クリック　(画面表示する前に)
        /// 順番：三番また四番
        /// </summary>
        private void treeList1_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            if (this.cobMonitorServerList.SelectedValue == null)
            {
                return;
            }

            treeList2_check_init();
            getAndInitPageDefault();
            init_nodeCheck(e.Node, false);

            //展开此节点后，若父节点到根节点含除外条件时，此节点的子节点，执行除外条件——2014-8-30 wjd add
            #region 使用除外条件
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();

            List<string> exceptedExt = new List<string>();

            TreeListNode node = e.Node;
            while (node != null)
            {
                string path = getCurrentNodePath(node);
                FileTypeSet fileTypeSet = iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(monitorServerId, path);
                string ea1 = fileTypeSet.exceptAttribute1;
                bool flag1 = fileTypeSet.exceptAttributeFlg1 == "1";
                string ea2 = fileTypeSet.exceptAttribute2;
                bool flag2 = fileTypeSet.exceptAttributeFlg2 == "1";
                string ea3 = fileTypeSet.exceptAttribute3;
                bool flag3 = fileTypeSet.exceptAttributeFlg3 == "1";

                if (flag1 && ea1 != "")
                {
                    exceptedExt.Add(ea1);
                }
                if (flag2 && ea2 != "")
                {
                    exceptedExt.Add(ea2);
                }
                if (flag3 && ea3 != "")
                {
                    exceptedExt.Add(ea3);
                }

                node = node.ParentNode;
            }
            //记录当前节点是否选中
            bool isChecked = e.Node.Checked;

            //至少一个除外条件
            if (exceptedExt.Count > 0)
            {
                int uncheckedNum = 0;
                foreach (TreeListNode childnode in e.Node.Nodes)
                {
                    //先设置全部选中
                    if (isChecked)
                    {
                        childnode.Checked = true;
                    }
                    FileInfo fi = new FileInfo(getCurrentNodePath(childnode));
                    if (fi.Exists)
                    {
                        if (exceptedExt.Contains(fi.Extension))
                        {
                            //删除原有的
                            MonitorServerFolder childmsf = getObjByNode(childnode);
                            lists_remove(childmsf);

                            childnode.Checked = false;
                            e.Node.CheckState = CheckState.Indeterminate;
                        }
                        //记录未选中的
                        if (!childnode.Checked)
                        {
                            uncheckedNum++;
                        }
                    }
                }

                //当子节点全不选中时，取消此节点选中状态
                if (uncheckedNum > 0 && uncheckedNum == e.Node.Nodes.Count)
                {
                    e.Node.CheckState = CheckState.Unchecked;
                }
            }
            #endregion
        }
        /// <summary>
        /// get except condition
        /// </summary>
        private void formatListsByFileTypeSet()
        {
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
            IList<FileTypeSet> fileTypeSetList = this.iftsSvc.GetFileTypeSetByMonitorServerID(monitorServerId);
            foreach (FileTypeSet fileTypeSet in fileTypeSetList)
            {
                if (!Directory.Exists(fileTypeSet.monitorServerFolderName))
                {
                    continue;
                }
                List<string> exList = new List<string>();
                if (fileTypeSet.exceptAttributeFlg1 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute1))
                {
                    exList.Add(fileTypeSet.exceptAttribute1);
                }
                if (fileTypeSet.exceptAttributeFlg2 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute2))
                {
                    exList.Add(fileTypeSet.exceptAttribute2);
                }
                if (fileTypeSet.exceptAttributeFlg3 == "1" && !string.IsNullOrEmpty(fileTypeSet.exceptAttribute3))
                {
                    exList.Add(fileTypeSet.exceptAttribute3);
                }
                DirectoryInfo dir = new DirectoryInfo(fileTypeSet.monitorServerFolderName);
                FileInfo[] files = dir.GetFiles();
                if (files.Count() > 0)
                {
                    foreach (FileInfo fileInfo in files)
                    {
                        string extension = fileInfo.Extension;
                        if (!exList.Contains(extension))
                        {
                            continue;
                        }
                        string fullPath = fileInfo.FullName.IndexOf('\\') > -1 ? fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('\\')) : fileInfo.FullName;
                        MonitorServerFolder msf = new MonitorServerFolder();
                        msf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                        msf.monitorFilePath = fullPath;
                        msf.monitorFileName = fileInfo.Name;
                        msf.monitorFileType = fileInfo.Extension;
                        formatListsData(msf);
                    }
                }
            }
        }
        private static List<MonitorServerFolder> tempRemoveLists = new List<MonitorServerFolder>();
        /// <summary>
        /// format except list and remove
        /// </summary>
        /// <param name="msf"></param>
        private void formatListsData(MonitorServerFolder msf)
        {
            string msfFullPath = msf.monitorFilePath + "\\" + msf.monitorFileName;
            List<MonitorServerFolder> tempLists = new List<MonitorServerFolder>();
            //List<MonitorServerFolder> tempRemoveLists = new List<MonitorServerFolder>();
            if (lists.Count > 0)
            {
                foreach (MonitorServerFolder innerMsf in lists)
                {
                    string listFullPath = innerMsf.monitorFilePath + "\\" + innerMsf.monitorFileName;
                    if (msfFullPath.IndexOf(listFullPath) > -1)
                    {
                        if (lists_exist(tempRemoveLists, innerMsf) <= -1)
                        {
                            tempRemoveLists.Add(innerMsf);
                        }
                        if (lists_exist(tempRemoveLists, msf) <= -1)
                        {
                            tempRemoveLists.Add(msf);
                        }
                        if (Directory.Exists(listFullPath))
                        {
                            DirectoryInfo dir = new DirectoryInfo(listFullPath);
                            FileSystemInfo[] fileSystemInfos = dir.GetFileSystemInfos();
                            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
                            {
                                string fullPath = fileSystemInfo.FullName.IndexOf('\\') > -1 ? fileSystemInfo.FullName.Substring(0, fileSystemInfo.FullName.LastIndexOf('\\')) : fileSystemInfo.FullName;
                                string extension = fileSystemInfo.Extension;
                                if (Directory.Exists(fileSystemInfo.FullName))
                                {
                                    extension = "99";
                                }
                                MonitorServerFolder msfl = new MonitorServerFolder();
                                msfl.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                                msfl.monitorFilePath = fullPath;
                                msfl.monitorFileName = fileSystemInfo.Name;
                                msfl.monitorFileType = extension;
                                if (msfFullPath.IndexOf(fileSystemInfo.FullName) > -1 && !Directory.Exists(fileSystemInfo.FullName))
                                {
                                    continue;
                                }
                                else
                                {
                                    tempLists.Add(msfl);
                                }
                            }
                        }
                    }
                }
                if (tempLists.Count > 0)
                {
                    foreach (MonitorServerFolder innerMsf in tempLists)
                    {
                        lists_contain(innerMsf);
                        lists.Add(innerMsf);
                    }
                }
                if (tempRemoveLists.Count > 0)
                {
                    foreach (MonitorServerFolder innerMsf in tempRemoveLists)
                    {
                        lists_contain(innerMsf);
                    }
                }
            }
        }

        /// <summary>
        /// treeList1 ノードチェックの初期化関数
        /// </summary>
        /// <param name="e">节点</param>
        /// <param name="flag">true：点击前；false：点击后</param>
        private void init_nodeCheck(TreeListNode e, bool flag)
        {
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
            if (lists.Count > 0)
            {
                foreach (MonitorServerFolder msf in lists)
                {
                    string path = msf.monitorFilePath;
                    string fileName = msf.monitorFileName;
                    string pathFile = msf.monitorFilePath + "\\" + msf.monitorFileName;
                    if (fileName == "")
                    {
                        pathFile = msf.monitorFilePath;
                    }
                    if (e != null)
                    {
                        string nodeName = e.GetValue("Name").ToString();
                        string sbInner = getCurrentNodePath(e);
                        if (sbInner == pathFile)
                        {
                            e.CheckState = CheckState.Checked;
                            if (e.ParentNode != null)
                            {
                                e.ParentNode.CheckState = CheckState.Indeterminate;
                            }
                        }
                        else if (sbInner != pathFile && path.IndexOf(sbInner) > -1 && System.IO.Directory.Exists(sbInner))
                        {
                            e.CheckState = CheckState.Indeterminate;
                        }
                    }
                    //foreach (TreeListNode tln in this.treeList2.Nodes)
                    //{
                    //    string nodeName = tln.GetValue("FileName").ToString();
                    //    string nodePath = tln.GetValue("NodePath").ToString();
                    //    string nodePathName = nodePath + "\\" + nodeName;
                    //    if (nodePathName == pathFile)
                    //    {
                    //        tln.CheckState = CheckState.Checked;
                    //    }
                    //    else if (nodePathName != pathFile && path.IndexOf(nodePathName) > -1)
                    //    {
                    //        tln.CheckState = CheckState.Indeterminate;
                    //    }
                    //    //FileInfo fi = new FileInfo(pathFile);
                    //    //if (this.checkBox1.Checked == true)
                    //    //{
                    //    //    if ((fi.Attributes & FileAttributes.System) == FileAttributes.System)
                    //    //    {
                    //    //        tln.Visible = false;
                    //    //    }
                    //    //}
                    //    //if (this.checkBox2.Checked == true)
                    //    //{
                    //    //    if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    //    //    {
                    //    //        tln.Visible = false;
                    //    //    }
                    //    //}
                    //}
                    foreach (TreeListNode tln in e.Nodes)
                    {
                        string nodeName = tln.GetValue("Name").ToString();
                        string sbInner = getCurrentNodePath(tln);
                        if (sbInner == pathFile)
                        {
                            if (tln.CheckState == CheckState.Unchecked)
                            {
                                tln.CheckState = CheckState.Checked;
                            }
                            if (tln.ParentNode != null)
                            {
                                tln.ParentNode.CheckState = CheckState.Indeterminate;
                            }
                        }
                        else if (sbInner != pathFile && path.IndexOf(sbInner) > -1 && System.IO.Directory.Exists(sbInner) && tln.CheckState == CheckState.Unchecked)
                        {
                            tln.CheckState = CheckState.Indeterminate;
                        }
                    }
                }

                //当子节点全部选中时，父节点标为选中状态
                int nodeCount = 0;
                if (e != null)
                {
                    if (e.Nodes != null)
                    {
                        nodeCount = e.Nodes.Count;
                        int checkedNum = 0;
                        foreach (TreeListNode tln in e.Nodes)
                        {
                            if (tln.CheckState == CheckState.Checked)
                            {
                                checkedNum++;
                            }
                        }
                        if (nodeCount > 0 && checkedNum == nodeCount && e.CheckState != CheckState.Checked)
                        {
                            e.CheckState = CheckState.Checked;
                        }
                        else if (checkedNum > 0 && checkedNum < nodeCount && e.CheckState != CheckState.Indeterminate)
                        {
                            e.CheckState = CheckState.Indeterminate;
                        }
                    }
                    if (e.ParentNode != null)
                    {
                        nodeCount = e.ParentNode.Nodes.Count;
                        int checkedNum = 0;
                        foreach (TreeListNode tln in e.ParentNode.Nodes)
                        {
                            if (tln.CheckState == CheckState.Checked)
                            {
                                checkedNum++;
                            }
                        }
                        if (nodeCount > 0 && checkedNum == nodeCount && e.ParentNode.CheckState != CheckState.Checked)
                        {
                            e.ParentNode.CheckState = CheckState.Checked;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// treeList2 ノードチェックの初期化関数
        /// </summary>
        private void treeList2_check_init()
        {
            //if (this.treeList1.FocusedNode != null)
            //{
            //    foreach (TreeListNode tn in this.treeList1.FocusedNode.Nodes)
            //    {
            //        string path = getCurrentNodePath(tn);
            //        //if (System.IO.File.Exists(path))
            //        //{
            //        //    tn.Visible = false;
            //        //}
            //        if (tn.HasChildren)
            //        {
            //            foreach (TreeListNode tln in tn.Nodes)
            //            {
            //                string innerFullPath = getCurrentNodePath(tln);
            //                if (path == innerFullPath)
            //                {
            //                    tln.CheckState = tn.CheckState;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// treeList1 チェックをクリックしたとき　執行
        /// 画面表示以後
        /// 順番：一番
        /// </summary>
        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            if (e.PrevState == CheckState.Checked || e.PrevState == CheckState.Indeterminate)
            {
                e.State = CheckState.Unchecked;
            }
            else
            {
                e.State = CheckState.Checked;
            }

            //the influence to the right treelist checkbox
            //string filename = e.Node.GetValue("Name").ToString();

            //bool father = false;
            //if (treeList2.Nodes.Count > 0)
            //{
            //    if (treeList2.Nodes[0].GetValue("ParentName").Equals(filename)) father = true;
            //    foreach (TreeListNode node in treeList2.Nodes)
            //    {
            //        if (father) { node.CheckState = e.State; continue; }
            //        if (node.GetValue("FileName").Equals(filename)) node.CheckState = e.State;
            //    }
            //}
        }

        /// <summary>
        /// treeList1 チェックをクリックし後のとき　執行
        /// 画面表示以後
        /// 順番：二番
        /// </summary>
        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            this.Node_AfterChecked(e.Node);
        }

        /// <summary>
        /// Run when the Node After Checked
        /// </summary>
        /// <param name="node"></param>
        private void Node_AfterChecked(TreeListNode node)
        {
            //init the treeList2 and set check state
            treeList2_check_init();

            CheckState flag = node.CheckState;
            MonitorServerFolder msfl = new MonitorServerFolder();
            //msfl.monitorFileName = node.GetValue("Name").ToString();
            string path = getCurrentNodePath(node);
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
            MonitorServer ms = this.imsSvc.GetMonitorServerById(Int32.Parse(monitorServerId));
            // 20140321 wang web対応
            string monitorRootPath = @"\\" + ms.monitorServerIP + @"\" + ms.startFile.TrimEnd('\\').TrimStart('\\');
            string nodeName = node.GetValue("Name").ToString();
            msfl.monitorFileName = nodeName;
            //if (path == ms.monitorDrive && nodeName == ms.monitorDrive)

            bool isRootNode = false;
            if (path == monitorRootPath && nodeName == monitorRootPath)
            {
                msfl.monitorFilePath = path;
                msfl.monitorFileName = "";
                isRootNode = true;
            }
            else
            {
                msfl.monitorFilePath = path.Substring(0, path.LastIndexOf("\\"));
            }
            msfl.monitorServerID = Convert.ToInt32(monitorServerId);
            msfl.monitorFileType = "99";
            if (flag == CheckState.Checked)
            {
                TreeListNode tln = node;

                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    //使用除外条件
                    FileTypeSet fileTypeSet = iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(monitorServerId, path);
                    string ea1 = fileTypeSet.exceptAttribute1;
                    bool flag1 = fileTypeSet.exceptAttributeFlg1 == "1";
                    string ea2 = fileTypeSet.exceptAttribute2;
                    bool flag2 = fileTypeSet.exceptAttributeFlg2 == "1";
                    string ea3 = fileTypeSet.exceptAttribute3;
                    bool flag3 = fileTypeSet.exceptAttributeFlg3 == "1";
                    //未选中的子节点
                    int uncheckedNum = 0;
                    foreach (TreeListNode childnode in node.Nodes)
                    {
                        FileInfo fi = new FileInfo(getCurrentNodePath(childnode));
                        if (fi.Exists)
                        {
                            //删除原有的
                            MonitorServerFolder childmsf = getObjByNode(childnode);
                            lists_remove(childmsf);

                            if (flag1 && fi.Extension == ea1 || flag2 && fi.Extension == ea2 || flag3 && fi.Extension == ea3)
                            {
                                childnode.Checked = false;
                                node.CheckState = CheckState.Indeterminate;
                            }
                            else
                            {
                                lists.Add(childmsf);
                            }
                            //记录未选中的
                            if (!childnode.Checked)
                            {
                                uncheckedNum++;
                            }
                        }
                    }
                    //当子节点全不选中时，取消此节点选中状态
                    if (uncheckedNum > 0 && uncheckedNum == node.Nodes.Count)
                    {
                        node.CheckState = CheckState.Unchecked;
                        //删除原有的
                        MonitorServerFolder currmsf = getObjByNode(node);
                        lists_remove(currmsf);
                    }

                    //当此节点为末节点，父节点选中时，只保存父节点路径
                    while (tln.ParentNode != null && tln.ParentNode.CheckState == CheckState.Checked)
                    {
                        tln = tln.ParentNode;
                    }

                    //当前选中了根节点，保存第一层子节点所有文件夹——2014-8-30 wjd add
                    if (isRootNode)
                    {
                        foreach (TreeListNode childnode in node.Nodes)
                        {
                            DirectoryInfo di = new DirectoryInfo(getCurrentNodePath(childnode));
                            if (di.Exists)
                            {
                                //删除原有的
                                MonitorServerFolder childmsf = getObjByNode(childnode);
                                lists_remove(childmsf);
                                lists.Add(childmsf);
                            }
                        }
                    }
                }

                MonitorServerFolder msf = getObjByNode(tln);
                lists_remove(msf);
                lists.Add(msf);
            }
            if (flag == CheckState.Unchecked)
            {
                lists_contain(msfl);
                if (node.ParentNode != null)
                {
                    foreach (TreeListNode tln in node.ParentNode.Nodes)
                    {
                        string fullPath = getCurrentNodePath(tln);
                        MonitorServerFolder innerMsf = new MonitorServerFolder();
                        innerMsf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                        innerMsf.monitorFilePath = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                        innerMsf.monitorFileName = tln.GetValue("Name").ToString();
                        innerMsf.monitorFileType = "99";
                        if (tln.CheckState == CheckState.Checked)
                        {
                            if (fullPath.IndexOf("\\") > -1)
                            {
                                lists_contain(innerMsf);
                                lists.Add(innerMsf);
                            }
                        }
                        if (tln.CheckState == CheckState.Unchecked)
                        {
                            lists_remove(innerMsf);
                        }
                    }
                    TreeListNode currentNode = node.ParentNode;
                    findParentOperation(currentNode);
                }
            }
            if (flag == CheckState.Indeterminate)
            {
                if (node.Nodes != null)
                {
                    foreach (TreeListNode childnode in node.Nodes)
                    {
                        childnode.CheckState = CheckState.Checked;
                    }
                }
            }

            SetCheckedChildNodes(node, node.CheckState);
            SetCheckedParentNodes(node, node.CheckState);
        }

        /// <summary>
        /// set the state of the child nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            //for (int i = 0; i < node.Nodes.Count; i++)
            //{
            //    node.Nodes[i].CheckState = check;

            //    SetCheckedChildNodes(node.Nodes[i], check);
            //}
        }
        /// <summary>
        /// set the state of the parent nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            //if (node.ParentNode != null)
            //{
            //    bool b = false;
            //    CheckState state;
            //    for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
            //    {
            //        state = (CheckState)node.ParentNode.Nodes[i].CheckState;
            //        if (!check.Equals(state))
            //        {
            //            b = !b;
            //            break;
            //        }
            //    }
            //    node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
            //    SetCheckedParentNodes(node.ParentNode, check);
            //}
        }

        /// <summary>
        /// 画面設定（除外条件の表示）
        /// </summary>
        private void getAndInitPageDefault()
        {
            this.panel6.Controls.Clear();
            this.checkBox1.Visible = this.checkBox1.Checked = false;
            this.checkBox2.Visible = this.checkBox2.Checked = false;
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
            if (lists.Count == 0)
            {
                IList<MonitorServerFolder> msfLists = this.imsfSvc.GetMonitorFolderByServerIDAndInitFlg(monitorServerId);
                lists = msfLists;
            }

            TreeListNode treeListNode = this.treeList1.Nodes.FirstNode;
            if (this.treeList1.FocusedNode != null)
            {
                treeListNode = this.treeList1.FocusedNode;
            }
            //treeListNode.Expanded = true;
            string treeListNodeName = treeListNode.GetValue("Name").ToString();

            string folderPath = getCurrentNodePath(treeListNode);
            ftsList = this.iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(monitorServerId, folderPath);
            if (ftsList.id != "" && ftsList.id != null)
            {
                string label1 = ftsList.exceptAttribute1;
                string label2 = ftsList.exceptAttribute2;
                string label3 = ftsList.exceptAttribute3;
                string labelFlg1 = ftsList.exceptAttributeFlg1;
                string labelFlg2 = ftsList.exceptAttributeFlg2;
                string labelFlg3 = ftsList.exceptAttributeFlg3;
                if (label1 != "" && label1 != null && labelFlg1 == "1")
                {
                    Label exceptLabel1 = new Label();
                    exceptLabel1.AutoSize = true;
                    exceptLabel1.ForeColor = System.Drawing.Color.Red;
                    exceptLabel1.Location = new System.Drawing.Point(2, 2);
                    exceptLabel1.Name = "exceptLabel1";
                    exceptLabel1.Text = label1;
                    this.panel6.Controls.Add(exceptLabel1);
                }
                if (label2 != "" && label2 != null && labelFlg2 == "1")
                {
                    Label exceptLabel2 = new Label();
                    exceptLabel2.AutoSize = true;
                    exceptLabel2.ForeColor = System.Drawing.Color.Red;
                    if (this.panel6.Controls.ContainsKey("exceptLabel1"))
                    {
                        exceptLabel2.Location = new System.Drawing.Point(52, 2);
                    }
                    else
                    {
                        exceptLabel2.Location = new System.Drawing.Point(2, 2);
                    }
                    exceptLabel2.Name = "exceptLabel2";
                    exceptLabel2.Text = label2;
                    this.panel6.Controls.Add(exceptLabel2);
                }
                if (label3 != "" && label3 != null && labelFlg3 == "1")
                {
                    Label exceptLabel3 = new Label();
                    exceptLabel3.AutoSize = true;
                    exceptLabel3.ForeColor = System.Drawing.Color.Red;
                    if (this.panel6.Controls.ContainsKey("exceptLabel1") && this.panel6.Controls.ContainsKey("exceptLabel2"))
                    {
                        exceptLabel3.Location = new System.Drawing.Point(104, 2);
                    }
                    else if (this.panel6.Controls.ContainsKey("exceptLabel1") || this.panel6.Controls.ContainsKey("exceptLabel2"))
                    {
                        exceptLabel3.Location = new System.Drawing.Point(52, 2);
                    }
                    else
                    {
                        exceptLabel3.Location = new System.Drawing.Point(2, 2);
                    }
                    exceptLabel3.Name = "exceptLabel3";
                    exceptLabel3.Text = label3;
                    this.panel6.Controls.Add(exceptLabel3);
                }
                if (ftsList.systemFileFlg == "1")
                {
                    this.checkBox1.Visible = this.checkBox1.Checked = true;
                }
                else
                {
                    this.checkBox1.Visible = this.checkBox1.Checked = false;
                }
                if (ftsList.hiddenFileFlg == "1")
                {
                    this.checkBox2.Visible = this.checkBox2.Checked = true;
                }
                else
                {
                    this.checkBox2.Visible = this.checkBox2.Checked = false;
                }
            }
        }

        /// <summary>
        /// 変更ボタンのクリック
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.cobMonitorServerList.SelectedValue != null)
            {
                this.Hide();
                TreeListNode tln = null;
                if (this.treeList1.FocusedNode == null)
                {
                    tln = this.treeList1.Nodes.FirstNode;
                }
                else
                {
                    tln = this.treeList1.FocusedNode;
                }
                if (tln != null)
                {
                    string folderPath = getCurrentNodePath(tln);

                    //Save the status.——2014-8-19 wjd add
                    bool isExpanded = tln.Expanded;

                    //Save Name of the selected node.
                    List<string> paths = new List<string>();
                    string name = tln.GetValue("Name").ToString();
                    paths.Add(name);
                    while (tln.ParentNode != null)
                    {
                        tln = tln.ParentNode;
                        name = tln.GetValue("Name").ToString();
                        paths.Add(name);
                    }

                    FrmGroupFile frmGroupFile = new FrmGroupFile(folderPath, this.cobMonitorServerList.SelectedValue.ToString());
                    frmGroupFile.FormClosed += new FormClosedEventHandler(this.Form_Closed);
                    DialogResult dialogResult = frmGroupFile.ShowDialog();

                    //Expand the node(reloaded treelist) that privous selected.
                    if (this.treeList1.FocusedNode == null)
                    {
                        tln = this.treeList1.Nodes.FirstNode;
                    }
                    else
                    {
                        tln = this.treeList1.FocusedNode;
                    }
                    tln = tln.RootNode;
                    tln.Selected = true;

                    for (int i = paths.Count - 2; i >= 0; i--)
                    {
                        if (paths[i] != null && tln.Nodes != null)
                        {
                            foreach (TreeListNode node in tln.Nodes)
                            {
                                if (paths[i].Equals(node.GetValue("Name").ToString()))
                                {
                                    node.Selected = true;

                                    tln = node;
                                    break;
                                }
                            }
                        }
                    }

                    tln.Expanded = isExpanded;
                    //Use the condition——2014-8-27 wjd add
                    if (dialogResult == DialogResult.OK)
                    {
                        tln.CheckAll();
                        this.Node_AfterChecked(tln);
                    }
                }
                else
                {
                    this.Show();
                }
            }
        }


        /// <summary>
        /// format the table data
        /// </summary>
        private void dgrdLogDataFormat(int pindex = 1)
        {
            //get the log List
            this.dgrdLog.AllowUserToAddRows = false;
            //this.dgrdLog.AllowUserToResizeColumns = false;
            this.dgrdLog.AllowUserToResizeRows = false;
            this.dgrdLog.Rows.Clear();
            ILogService ils = BLLFactory.ServiceAccess.CreateLogService();

            int displayFlg = this.rbtnOpint1.Checked == true ? 0 : 1;
            int transferFlg = this.rbtnOpint3.Checked == true ? 0 : 1;

            int backupFlg = 2;
            if (this.rbtnOpint6.Checked)
            {
                backupFlg = 0;
            }
            else if (this.rbtnOpint5.Checked)
            {
                backupFlg = 1;
            }

            int logFlg = this.rbtnOpint7.Checked == true ? 0 : 1;

            string selectServerGroupID = this.cobBackupServerGroupName.SelectedValue.ToString();
            DateTime startDate = this.dtpStartDate.Value.Date;
            DateTime endDate = this.dtpEndDate.Value.Date;
            string startTime = this.dtpLogStartTime.Text.ToString();
            string endTime = this.dtpLogEndTime.Text.ToString();
            string fileName = this.txtLogFile.Text.Trim();
            //IList<Log> logLists = ils.GetLogList(fileName, transferFlg, backupFlg, selectServerGroupID, startDate, endDate, startTime, endTime);

            //the same to Web
            if (pindex < 1)
            {
                pindex = 1;
            }
            int pagesize = 20;
            IList<Log> logLists = ils.GetLogListByProc(pindex, pagesize, selectServerGroupID, startDate, endDate, startTime, endTime, displayFlg, transferFlg, backupFlg, logFlg, fileName);
            int totalCount = 0;
            foreach (Log l in logLists)
            {
                totalCount = l.totalCount;
                break;
            }
            int pagecount = (int)Math.Ceiling((double)totalCount / pagesize);

            int i = 1;
            foreach (Log list in logLists)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in this.dgrdLog.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = list.id;
                dgvr.Cells[1].Value = (i + (pindex - 1) * pagesize).ToString();
                dgvr.Cells[2].Value = list.backupServerFileName;
                string fileSize = "";
                if (!String.IsNullOrEmpty(list.backupServerFileSize))
                {
                    fileSize = String.Format("{0:N0}", Convert.ToInt64(list.backupServerFileSize));
                    fileSize = fileSize + " Byte";
                }
                //
                dgvr.Cells[3].Value = list.monitorFilePath;
                dgvr.Cells[4].Value = fileSize;
                dgvr.Cells[5].Value = list.copyStartTime;
                dgvr.Cells[6].Value = list.copyEndTime;
                dgvr.Cells[7].Value = list.backupStartTime;
                dgvr.Cells[8].Value = list.backupEndTime;
                dgvr.Cells[9].Value = list.backupTime + "秒";
                dgvr.Cells[10].Value = list.backupFlg == 1 ? "OK" : "NG";
                dgvr.Cells[11].Value = list.monitorFileStatus;
                this.dgrdLog.Rows.Add(dgvr);
                i++;
            }

            #region Page Info

            this.pnlPage.Controls.Clear();

            if (pagecount > 1)
            {
                if (pindex == 1)
                {
                    Label lbl = new Label()
                    {
                        Name = "prevPage",
                        Text = "<< 前のページ",
                        Location = new Point(1, 10),
                        AutoSize = true
                    };
                    this.pnlPage.Controls.Add(lbl);
                }
                else
                {
                    LinkLabel lnk = new LinkLabel()
                    {
                        Name = (pindex - 1).ToString(),
                        Text = "<< 前のページ",
                        Location = new Point(1, 10),
                        AutoSize = true
                    };
                    lnk.Click += new EventHandler(PageNum_Click);
                    this.pnlPage.Controls.Add(lnk);
                }
                if (pagecount >= 5)
                {
                    if (pindex == 1)
                    {
                        for (i = 1; i < 6; i++)
                        {
                            if (i == pindex)
                            {
                                Label lbl = new Label()
                                {
                                    Name = "currPage",
                                    Text = i.ToString(),
                                    Location = new Point(80 + i * 20, 10),
                                    AutoSize = true
                                };
                                this.pnlPage.Controls.Add(lbl);
                            }
                            else
                            {
                                LinkLabel lnk = new LinkLabel()
                                {
                                    Name = i.ToString(),
                                    Text = i.ToString(),
                                    Location = new Point(80 + i * 20, 10),
                                    AutoSize = true
                                };
                                lnk.Click += new EventHandler(PageNum_Click);
                                this.pnlPage.Controls.Add(lnk);
                            }
                        }
                    }
                    else if ((pindex + 4) - pagecount > 0)
                    {
                        for (i = pagecount - 4; i < pagecount + 1; i++)
                        {
                            if (i == pindex)
                            {
                                Label lbl = new Label()
                                {
                                    Name = "currPage",
                                    Text = i.ToString(),
                                    Location = new Point(80 + (i - (pagecount - 4) + 1) * 20, 10),
                                    AutoSize = true
                                };
                                this.pnlPage.Controls.Add(lbl);
                            }
                            else
                            {
                                LinkLabel lnk = new LinkLabel()
                                {
                                    Name = i.ToString(),
                                    Text = i.ToString(),
                                    Location = new Point(80 + (i - (pagecount - 4) + 1) * 20, 10),
                                    AutoSize = true
                                };
                                lnk.Click += new EventHandler(PageNum_Click);
                                this.pnlPage.Controls.Add(lnk);
                            }
                        }
                    }
                    else
                    {
                        for (i = (pindex - 1); i < (pindex + 4); i++)
                        {
                            if (i == pindex)
                            {
                                Label lbl = new Label()
                                {
                                    Name = "currPage",
                                    Text = i.ToString(),
                                    Location = new Point(80 + (i - (pindex - 1) + 1) * 20, 10),
                                    AutoSize = true
                                };
                                this.pnlPage.Controls.Add(lbl);
                            }
                            else
                            {
                                LinkLabel lnk = new LinkLabel()
                                {
                                    Name = i.ToString(),
                                    Text = i.ToString(),
                                    Location = new Point(80 + (i - (pindex - 1) + 1) * 20, 10),
                                    AutoSize = true
                                };
                                lnk.Click += new EventHandler(PageNum_Click);
                                this.pnlPage.Controls.Add(lnk);
                            }
                        }
                    }
                }
                else
                {
                    int x_Start = this.pnlPage.Width / 2 - pagecount * 20 / 2 + 5;
                    for (i = 1; i < pagecount + 1; i++)
                    {
                        if (i == pindex)
                        {
                            Label lbl = new Label()
                            {
                                Name = "currPage",
                                Text = i.ToString(),
                                Location = new Point(x_Start + (i - 1) * 20, 10),
                                AutoSize = true
                            };
                            this.pnlPage.Controls.Add(lbl);
                        }
                        else
                        {
                            LinkLabel lnk = new LinkLabel()
                            {
                                Name = i.ToString(),
                                Text = i.ToString(),
                                Location = new Point(x_Start + (i - 1) * 20, 10),
                                AutoSize = true
                            };
                            lnk.Click += new EventHandler(PageNum_Click);
                            this.pnlPage.Controls.Add(lnk);
                        }
                    }
                }
                if (pindex == pagecount)
                {
                    Label lbl = new Label()
                    {
                        Name = "nextPage",
                        Text = "次のページ >>",
                        Location = new Point(220, 10),
                        AutoSize = true
                    };
                    this.pnlPage.Controls.Add(lbl);
                }
                else
                {
                    LinkLabel lnk = new LinkLabel()
                    {
                        Name = (pindex + 1).ToString(),
                        Text = "次のページ >>",
                        Location = new Point(220, 10),
                        AutoSize = true
                    };
                    lnk.Click += new EventHandler(PageNum_Click);
                    this.pnlPage.Controls.Add(lnk);
                }
            }
            #endregion
        }

        /// <summary>
        /// jump page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageNum_Click(object sender, EventArgs e)
        {
            LinkLabel lnk = (LinkLabel)sender;
            int pindex = Convert.ToInt32(lnk.Name);
            dgrdLogDataFormat(pindex);
        }

        private void btnAuthAdd_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmAuth frmAuth = new FrmAuth();
            frmAuth.ShowDialog();
        }

        public void FormatLogCountpnl()
        {
            this.pnlTrVolumn.Controls.Clear();
            //init the panel, according to the database table to generate the DataGridView dynamically
            Label lblLogTranSize = new Label();
            lblLogTranSize.Text = "転送容量表示";
            lblLogTranSize.Size = new Size(77, 12);
            lblLogTranSize.Location = new Point(3, 9);
            this.pnlTrVolumn.Controls.Add(lblLogTranSize);

            //ILogService ils = BLLFactory.ServiceAccess.CreateLogService();
            ITransferLogService transferLogService = BLLFactory.ServiceAccess.CreateTransferLogService();

            //string transferFlg = this.rbtnOpint3.Checked == true ? "0" : "1";
            string backupFlg = "";
            if (this.rbtnOpint5.Checked)
            {
                backupFlg = "1";
            }
            else if (this.rbtnOpint6.Checked)
            {
                backupFlg = "0";
            }

            string selectServerGroupID = this.cobBackupServerGroupName.SelectedValue.ToString();
            DateTime startDate = this.dtpStartDate.Value.Date;
            DateTime endDate = this.dtpEndDate.Value.Date;
            string startTime = this.dtpLogStartTime.Text.ToString();
            string endTime = this.dtpLogEndTime.Text.ToString();
            string fileName = this.txtLogFile.Text.Trim();
            //IList<Log> logLists = ils.GetLogList(fileName, transferFlg, backupFlg, selectServerGroupID, startDate, endDate, startTime, endTime);

            IList<TransferLog> transferLogList = transferLogService.GetTransferLogList(selectServerGroupID, startDate, endDate, startTime, endTime, fileName,
                backupFlg);

            Dictionary<String, Dictionary<String, LogCount>> logCountList = mergeList(transferLogList);
            int j = 0;

            string tempStartTime = this.dtpLogStartTime.Text;
            string[] tstArr = tempStartTime.Split(':');
            string tempEndTime = this.dtpLogEndTime.Text;
            string[] tetArr = tempEndTime.Split(':');
            int start = 0;
            int end = 23;
            if (tstArr[0] != "" && Convert.ToInt32(tstArr[0]) >= 0 && Convert.ToInt32(tstArr[0]) <= 23)
            {
                start = Convert.ToInt32(tstArr[0]);
            }
            if (tetArr[0] != "" && Convert.ToInt32(tetArr[0]) >= 0 && Convert.ToInt32(tetArr[0]) <= 23)
            {
                end = Convert.ToInt32(tetArr[0]);
            }

            foreach (KeyValuePair<String, Dictionary<String, LogCount>> item in logCountList)
            {
                Dictionary<String, LogCount> logDic = new Dictionary<String, LogCount>();

                //定义一个datatable表
                DataTable dt = new DataTable();
                dt.TableName = item.Key;
                dt.Columns.Add(new DataColumn("日付", typeof(string)));
                dt.Columns.Add(new DataColumn("時間帯", typeof(string)));
                dt.Columns.Add(new DataColumn("処理ファイル数", typeof(string)));
                dt.Columns.Add(new DataColumn("転送容量", typeof(string)));

                for (int i = start; i <= end; i++)
                {
                    bool flg = true;
                    DataRow dr2 = dt.NewRow();
                    foreach (KeyValuePair<String, LogCount> itemInner in item.Value)
                    {
                        if (Convert.ToInt32(itemInner.Key) == i)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = itemInner.Value.date;
                            dr[1] = itemInner.Value.time + "時";
                            dr[2] = itemInner.Value.filecount;
                            string fileSize = String.Format("{0:N0}", itemInner.Value.volumn);
                            dr[3] = fileSize + " Byte";
                            dt.Rows.Add(dr);
                            flg = false;
                            break;
                        }
                    }
                    if (flg == true)
                    {
                        dr2[0] = item.Key.ToString();
                        dr2[1] = i + "時";
                        dr2[2] = 0;
                        dr2[3] = 0 + " Byte";
                        dt.Rows.Add(dr2);
                    }
                }

                DataGridView dg = new DataGridView();
                dg.DataSource = dt;
                dg.AllowUserToAddRows = false;
                dg.Location = new Point(3, 24 + 578 * j);
                dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dg.RowHeadersVisible = false;
                dg.Width = 770;
                dg.Height = 578;
                dg.AllowUserToResizeRows = false;
                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dg.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(128, 255, 255);
                dg.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
                dg.ReadOnly = true;
                this.pnlTrVolumn.Controls.Add(dg);
                j++;
            }
            if (logCountList.Count == 0)
            {
                for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
                {
                    //定义一个datatable表
                    DataTable dt = new DataTable();
                    dt.TableName = d.ToString("yyyy-MM-dd");
                    dt.Columns.Add(new DataColumn("日付", typeof(string)));
                    dt.Columns.Add(new DataColumn("時間帯", typeof(string)));
                    dt.Columns.Add(new DataColumn("処理ファイル数", typeof(string)));
                    dt.Columns.Add(new DataColumn("転送容量", typeof(string)));

                    for (int i = start; i <= end; i++)
                    {
                        DataRow dr2 = dt.NewRow();
                        dr2[0] = d.ToString("yyyy-MM-dd");
                        dr2[1] = i + "時";
                        dr2[2] = 0;
                        dr2[3] = 0 + " Byte";
                        dt.Rows.Add(dr2);
                    }

                    DataGridView dg = new DataGridView();
                    dg.DataSource = dt;
                    dg.AllowUserToAddRows = false;
                    dg.Location = new Point(3, 24 + 578 * j);
                    dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dg.RowHeadersVisible = false;
                    dg.Width = 770;
                    dg.Height = 578;
                    dg.AllowUserToResizeRows = false;
                    dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dg.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(128, 255, 255);
                    dg.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
                    dg.ReadOnly = true;
                    this.pnlTrVolumn.Controls.Add(dg);
                    j++;
                }

                //DataGridView dg = new DataGridView();
                //dg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                //        | System.Windows.Forms.AnchorStyles.Left)
                //        | System.Windows.Forms.AnchorStyles.Right)));

                //dg.DataSource = dt;
                //dg.AllowUserToAddRows = false;
                //dg.Location = new Point(3, 24);
                //dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //dg.RowHeadersVisible = false;
                //dg.Width = 787;
                //dg.Height = 365;
                //dg.AllowUserToResizeRows = false;
                //dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //dg.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(128, 255, 255);
                //dg.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
                //dg.ReadOnly = true;
                //this.pnlTrVolumn.Controls.Add(dg);
            }

            this.pnlTrVolumn.AutoScroll = true;
            this.pnlTrVolumn.AutoScrollMinSize = new Size(710, 353);
        }


        public Dictionary<String, Dictionary<String, LogCount>> mergeList(IList<Log> list)
        {
            Dictionary<String, Dictionary<String, LogCount>> logDic = new Dictionary<String, Dictionary<String, LogCount>>();
            int i = 1;
            foreach (Log l in list)
            {
                string logStartTime = l.backupStartTime;
                DateTime logDateTime = DateTime.Parse(logStartTime);
                string date = CommonUtil.ToShortDateString(logDateTime);
                string hour = logDateTime.Hour.ToString();
                string index = date;
                string indexInner = hour;
                if (logDic.ContainsKey(index))
                {
                    if (logDic[index].ContainsKey(indexInner))
                    {
                        logDic[index][indexInner].date = date;
                        logDic[index][indexInner].time = hour;
                        logDic[index][indexInner].filecount += i++;
                        if (!String.IsNullOrEmpty(l.backupServerFileSize))
                        {
                            logDic[index][indexInner].volumn += Convert.ToInt64(l.backupServerFileSize);
                        }
                        else
                        {
                            logDic[index][indexInner].volumn += 0;
                        }
                    }
                    else
                    {
                        LogCount lc = new LogCount();
                        lc.filecount = 1;
                        if (!String.IsNullOrEmpty(l.backupServerFileSize))
                        {
                            lc.volumn = Convert.ToInt64(l.backupServerFileSize);
                        }
                        else
                        {
                            lc.volumn = 0;
                        }
                        lc.date = date;
                        lc.time = hour;
                        logDic[index][indexInner] = lc;
                    }

                }
                else
                {
                    Dictionary<String, LogCount> logDicInner = new Dictionary<String, LogCount>();
                    LogCount lc = new LogCount();
                    lc.filecount = 1;
                    if (!String.IsNullOrEmpty(l.backupServerFileSize))
                    {
                        lc.volumn = Convert.ToInt64(l.backupServerFileSize);
                    }
                    else
                    {
                        lc.volumn = 0;
                    }
                    lc.date = date;
                    lc.time = hour;
                    logDicInner.Add(indexInner, lc);
                    logDic.Add(index, logDicInner);
                }

            }
            return logDic;
        }

        public Dictionary<String, Dictionary<String, LogCount>> mergeList(IList<TransferLog> list)
        {
            Dictionary<String, Dictionary<String, LogCount>> logDic = new Dictionary<String, Dictionary<String, LogCount>>();
            //int i = 1;
            foreach (TransferLog l in list)
            {
                if (l.transferTime.IndexOf(":") < 0)
                {
                    l.transferTime += ":00";
                }
                string logStartTime = l.transferDate + " " + l.transferTime;
                DateTime logDateTime = DateTime.Parse(logStartTime);
                string date = CommonUtil.ToShortDateString(logDateTime);
                string hour = logDateTime.Hour.ToString();
                string index = date;
                string indexInner = hour;
                if (logDic.ContainsKey(index))
                {
                    if (logDic[index].ContainsKey(indexInner))
                    {
                        logDic[index][indexInner].date = date;
                        logDic[index][indexInner].time = hour;
                        logDic[index][indexInner].filecount += int.Parse(l.transferFileCount);
                        if (!String.IsNullOrEmpty(l.transferFileSize))
                        {
                            logDic[index][indexInner].volumn += Convert.ToInt64(l.transferFileSize);
                        }
                        else
                        {
                            logDic[index][indexInner].volumn += 0;
                        }
                    }
                    else
                    {
                        LogCount lc = new LogCount();
                        lc.filecount = int.Parse(l.transferFileCount);
                        if (!String.IsNullOrEmpty(l.transferFileSize))
                        {
                            lc.volumn = Convert.ToInt64(l.transferFileSize);
                        }
                        else
                        {
                            lc.volumn = 0;
                        }
                        lc.date = date;
                        lc.time = hour;
                        logDic[index][indexInner] = lc;
                    }

                }
                else
                {
                    Dictionary<String, LogCount> logDicInner = new Dictionary<String, LogCount>();
                    LogCount lc = new LogCount();
                    lc.filecount = int.Parse(l.transferFileCount);
                    if (!String.IsNullOrEmpty(l.transferFileSize))
                    {
                        lc.volumn = Convert.ToInt64(l.transferFileSize);
                    }
                    else
                    {
                        lc.volumn = 0;
                    }
                    lc.date = date;
                    lc.time = hour;
                    logDicInner.Add(indexInner, lc);
                    logDic.Add(index, logDicInner);
                }

            }
            return logDic;
        }

        private void conditionSearch()
        {
            if (this.rbtnOpint7.Checked == true)
            {
                this.pnlTrVolumn.Visible = false;
                this.pnlTrLog.Visible = true;
                dgrdLogDataFormat();
            }
            if (this.rbtnOpint8.Checked == true)
            {
                this.pnlTrLog.Visible = false;
                this.pnlTrVolumn.Visible = true;
                FormatLogCountpnl();
            }
        }
        //方法tabPage5_Load()中的代码会触发rbtnOpint7和rbtnOpint8选中状态变化的事件
        private void rbtnOpint7_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rbtnOpint8_CheckedChanged(object sender, EventArgs e)
        {
            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(conditionSearch));
        }

        private void rbtnOpint5_CheckedChanged(object sender, EventArgs e)
        {
            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(conditionSearch));
        }
        private void rbtnOpint6_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rbtnOpint9_CheckedChanged(object sender, EventArgs e)
        {
            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(conditionSearch));
        }
        private void rbtnOpint3_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rbtnOpint4_CheckedChanged(object sender, EventArgs e)
        {
            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(conditionSearch));
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(conditionSearch));
        }


        private string getCurrentNodePath(TreeListNode treeListNode)
        {
            string name = treeListNode.GetValue("Name").ToString();
            while (treeListNode.ParentNode != null)
            {
                treeListNode = treeListNode.ParentNode;
                if (treeListNode.GetValue("Name").ToString().IndexOf("\\") > -1)
                {
                    name = string.Format("{0}\\{1}", treeListNode.GetValue("Name").ToString().Trim(), name);
                }
                else
                {
                    name = string.Format("{0}\\{1}", treeListNode.GetValue("Name").ToString().Trim(), name);
                }

            }
            return name;
        }

        private void findParentOperation(TreeListNode treeListNode)
        {
            if (treeListNode != null)
            {
                MonitorServerFolder msf = getObjByNode(treeListNode);
                lists_contain(msf);
                foreach (TreeListNode tln in treeListNode.Nodes)
                {
                    string fullPath = getCurrentNodePath(tln);
                    MonitorServerFolder innerMsf = new MonitorServerFolder();
                    innerMsf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                    innerMsf.monitorFilePath = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                    innerMsf.monitorFileName = tln.GetValue("Name").ToString();
                    innerMsf.monitorFileType = "99";
                    if (tln.CheckState == CheckState.Checked)
                    {
                        if (fullPath.IndexOf("\\") > -1)
                        {
                            lists_contain(innerMsf);
                            lists.Add(innerMsf);
                        }
                    }
                    if (tln.CheckState == CheckState.Unchecked)
                    {
                        lists_remove(innerMsf);
                    }
                }
                TreeListNode currentNode = treeListNode.ParentNode;
                findParentOperation(currentNode);
            }

        }
        private MonitorServerFolder getObjByNode(TreeListNode tln)
        {
            string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
            MonitorServer ms = this.imsSvc.GetMonitorServerById(Int32.Parse(monitorServerId));
            // 20140321 wang web対応
            string monitorRootPath = @"\\" + ms.monitorServerIP + @"\" + ms.startFile.TrimEnd('\\').TrimStart('\\');
            MonitorServerFolder msf = new MonitorServerFolder();
            string path = "";
            string nodeName = "";

            path = getCurrentNodePath(tln);

            if (path != monitorRootPath)
            {
                path = path.Substring(0, path.LastIndexOf("\\"));
            }
            nodeName = tln.GetValue("Name").ToString();

            if (path == nodeName)
            {
                nodeName = "";
            }
            msf.monitorFilePath = path;
            msf.monitorFileName = nodeName;
            msf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
            //设置后缀名
            string fullPath = path + "\\" + nodeName;
            DirectoryInfo di = new DirectoryInfo(fullPath);
            if (di.Exists)
            {
                msf.monitorFileType = "99";
            }
            else
            {
                FileInfo fi = new FileInfo(fullPath);
                if (fi.Exists)
                {
                    msf.monitorFileType = fi.Extension;
                }
            }
            return msf;
        }

        private int lists_contain(MonitorServerFolder list)
        {
            int index = -1;
            for (int i = 0; i < lists.Count; i++)
            {
                if (lists[i].monitorServerID == list.monitorServerID
                    && lists[i].monitorFileName == list.monitorFileName
                    && lists[i].monitorFilePath == list.monitorFilePath)
                {
                    index = i;
                    lists.RemoveAt(i);
                    break;
                }
            }
            return index;
        }
        private int lists_exist(List<MonitorServerFolder> tempRemoveLists, MonitorServerFolder list)
        {
            int index = -1;
            for (int i = 0; i < tempRemoveLists.Count; i++)
            {
                if (tempRemoveLists[i].monitorServerID == list.monitorServerID
                    && tempRemoveLists[i].monitorFileName == list.monitorFileName
                    && tempRemoveLists[i].monitorFilePath == list.monitorFilePath)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        static IEnumerable<System.IO.FileInfo> GetFiles(string path)
        {
            if (!System.IO.Directory.Exists(path)) return null;

            string[] fileNames = null;
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();

            fileNames = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string name in fileNames)
            {
                files.Add(new System.IO.FileInfo(name));
            }
            return files;
        }

        static IEnumerable<System.IO.DirectoryInfo> GetDirs(string path)
        {
            if (!System.IO.Directory.Exists(path)) return null;

            string[] dirNames = null;
            List<System.IO.DirectoryInfo> dirs = new List<System.IO.DirectoryInfo>();

            dirNames = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
            foreach (string name in dirNames)
            {
                dirs.Add(new System.IO.DirectoryInfo(name));
            }
            return dirs;
        }


        private void exceptAndIncludeHandle()
        {
            IList<FileTypeSet> fileTypeSetLists = this.iftsSvc.GetFileTypeSetByMonitorServerID(this.cobMonitorServerList.SelectedValue.ToString());
            List<MonitorServerFolder> monitorServerFolderLists = new List<MonitorServerFolder>();
            foreach (FileTypeSet fts in fileTypeSetLists)
            {
                if (System.IO.Directory.Exists(fts.monitorServerFolderName))
                {
                    DirectoryInfo di = new DirectoryInfo(fts.monitorServerFolderName);
                    FileInfo[] files = di.GetFiles();
                    int sysAndhidCount = 0;
                    if (fts.hiddenFileFlg == "0" && fts.systemFileFlg == "1")
                    {
                        sysAndhidCount = getFileCountByFlag("0", di);
                    }
                    else if (fts.hiddenFileFlg == "1" && fts.systemFileFlg == "0")
                    {
                        sysAndhidCount = getFileCountByFlag("1", di);
                    }
                    else if (fts.hiddenFileFlg == "1" && fts.systemFileFlg == "1")
                    {
                        sysAndhidCount = getFileCountByFlag("2", di);
                    }
                    int conditionCount = 0;
                    if (fts.exceptAttribute1 != "" && fts.exceptAttribute1 != null)
                    {
                        conditionCount += getConditionCount(fts.exceptAttribute1, di);
                    }
                    if (fts.exceptAttribute2 != "" && fts.exceptAttribute2 != null)
                    {
                        conditionCount += getConditionCount(fts.exceptAttribute2, di);
                    }
                    if (fts.exceptAttribute3 != "" && fts.exceptAttribute3 != null)
                    {
                        conditionCount += getConditionCount(fts.exceptAttribute3, di);
                    }
                    int fileSysCount = di.GetFileSystemInfos().Count() - sysAndhidCount - conditionCount;
                    if ((fts.exceptAttribute1 != "" && fts.exceptAttribute1 != null)
                        || (fts.exceptAttribute2 != "" && fts.exceptAttribute2 != null)
                        || (fts.exceptAttribute3 != "" && fts.exceptAttribute3 != null))
                    {
                        foreach (FileInfo fi in files)
                        {
                            MonitorServerFolder msf = monitorFolderDataFormat(fi, "1");
                            //if (fts.includeAttribute1 != "" && fts.includeAttribute1 != null && fi.Extension == fts.includeAttribute1)
                            //{
                            //    if (lists_exist(msf) > -1)
                            //    {
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        addParentAndRemoveChild(fileSysCount, msf, fts);
                            //    }
                            //}
                            //if (fts.includeAttribute2 != "" && fts.includeAttribute2 != null && fi.Extension == fts.includeAttribute2)
                            //{
                            //    if (lists_exist(msf) > -1)
                            //    {
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        addParentAndRemoveChild(fileSysCount, msf, fts);
                            //    }
                            //}
                            //if (fts.includeAttribute3 != "" && fts.includeAttribute3 != null && fi.Extension == fts.includeAttribute3)
                            //{
                            //    if (lists_exist(msf) > -1)
                            //    {
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        addParentAndRemoveChild(fileSysCount, msf, fts);
                            //    }
                            //}
                        }
                    }
                }
            }
        }
        private void addParentAndRemoveChild(int fileSysCount, MonitorServerFolder msf, FileTypeSet fts)
        {
            lists.Add(msf);
            int checkCount = file_check_count(fts.monitorServerFolderName);
            if (fileSysCount == checkCount)
            {
                //delete child and add parent
                MonitorServerFolder innerMsf = new MonitorServerFolder();
                if (fts.monitorServerFolderName.IndexOf("\\") > -1)
                {
                    innerMsf.monitorFileName = fts.monitorServerFolderName.Substring(fts.monitorServerFolderName.LastIndexOf("\\") + 1);
                    innerMsf.monitorFilePath = fts.monitorServerFolderName.Substring(0, fts.monitorServerFolderName.LastIndexOf("\\"));
                    innerMsf.monitorFileType = "99";
                    innerMsf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                    lists.Add(innerMsf);
                    lists_child_remove(fts.monitorServerFolderName);
                    findParentFolder(innerMsf.monitorFilePath);
                }
            }
        }
        private void findParentFolder(string folderPath)
        {
            FileTypeSet fts = this.iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(this.cobMonitorServerList.SelectedValue.ToString(), folderPath);
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] files = di.GetFiles();
            int sysAndhidCount = 0;
            if (fts.hiddenFileFlg == "0" && fts.systemFileFlg == "1")
            {
                sysAndhidCount = getFileCountByFlag("0", di);
            }
            else if (fts.hiddenFileFlg == "1" && fts.systemFileFlg == "0")
            {
                sysAndhidCount = getFileCountByFlag("1", di);
            }
            else if (fts.hiddenFileFlg == "1" && fts.systemFileFlg == "1")
            {
                sysAndhidCount = getFileCountByFlag("2", di);
            }
            int fileSysCount = di.GetFileSystemInfos().Count() - sysAndhidCount;
            int checkCount = file_check_count(folderPath);
            if (fileSysCount == checkCount)
            {
                //delete child and add parent
                MonitorServerFolder innerMsf = new MonitorServerFolder();
                if (fts.monitorServerFolderName.IndexOf("\\") > -1)
                {
                    innerMsf.monitorFileName = fts.monitorServerFolderName.Substring(fts.monitorServerFolderName.LastIndexOf("\\") + 1);
                    innerMsf.monitorFilePath = fts.monitorServerFolderName.Substring(0, fts.monitorServerFolderName.LastIndexOf("\\"));
                    innerMsf.monitorFileType = "99";
                    innerMsf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
                    lists.Add(innerMsf);
                    lists_child_remove(fts.monitorServerFolderName);
                    if (innerMsf.monitorFilePath.IndexOf("\\") > -1)
                    {
                        findParentFolder(innerMsf.monitorFilePath.Substring(0, innerMsf.monitorFilePath.LastIndexOf("\\")));
                    }
                }
            }
        }
        private void lists_child_remove(string path)
        {
            for (int i = lists.Count - 1; i >= 0; i--)
            {
                if (path == lists[i].monitorFilePath)
                {
                    lists.RemoveAt(i);
                }
            }
        }
        private int file_check_count(string path)
        {
            int count = 0;
            foreach (MonitorServerFolder msf in lists)
            {
                if (msf.monitorFilePath == path)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// get file count
        /// </summary>
        /// <param name="flag">"0":system; "1":hidden; "2":system and hidden</param>
        /// <param name="di"></param>
        /// <returns></returns>
        private int getFileCountByFlag(string flag, DirectoryInfo di)
        {
            int count = 0;
            FileSystemInfo[] fileSystemInfoLists = di.GetFileSystemInfos();
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfoLists)
            {
                if (flag == "0")
                {
                    if ((fileSystemInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        count++;
                    }
                }
                else if (flag == "1")
                {
                    if ((fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        count++;
                    }
                }
                else if (flag == "2")
                {
                    if ((fileSystemInfo.Attributes & FileAttributes.System) == FileAttributes.System
                        || (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        private int getConditionCount(string extention, DirectoryInfo di)
        {
            int count = 0;
            FileSystemInfo[] fileSystemInfoLists = di.GetFileSystemInfos();
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfoLists)
            {
                if (fileSystemInfo.Extension == extention)
                {
                    count++;
                }
            }
            return count;
        }
        private int lists_exist(MonitorServerFolder list)
        {
            int index = -1;
            for (int i = 0; i < lists.Count; i++)
            {
                if (lists[i].monitorServerID == list.monitorServerID
                    && lists[i].monitorFileName == list.monitorFileName
                    && lists[i].monitorFilePath == list.monitorFilePath)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }


        private bool saveMonitorFile(FileInfo file)
        {
            string sb = file.FullName.Substring(0, file.FullName.LastIndexOf('\\'));
            FileTypeSet fileTypeSet = this.iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(this.cobMonitorServerList.SelectedValue.ToString(), sb);
            bool saveFlg = true;
            if (file.Exists)
            {
                List<String> strLists = new List<string>();
                if (fileTypeSet != null)
                {
                    if (fileTypeSet.exceptAttributeFlg1 == "1" && fileTypeSet.exceptAttribute1 != null && fileTypeSet.exceptAttribute1 != "")
                    {
                        strLists.Add(fileTypeSet.exceptAttribute1);
                    }
                    if (fileTypeSet.exceptAttributeFlg2 == "1" && fileTypeSet.exceptAttribute2 != null && fileTypeSet.exceptAttribute2 != "")
                    {
                        strLists.Add(fileTypeSet.exceptAttribute2);
                    }
                    if (fileTypeSet.exceptAttributeFlg3 == "1" && fileTypeSet.exceptAttribute3 != null && fileTypeSet.exceptAttribute3 != "")
                    {
                        strLists.Add(fileTypeSet.exceptAttribute3);
                    }
                }
                //if (this.panel6.Controls.ContainsKey("exceptLabel1"))
                //{
                //    Label exceptLabel1 = (Label)this.panel6.Controls.Find("exceptLabel1", true)[0];
                //    string name1 = exceptLabel1.Text;
                //    strLists.Add(name1);
                //}
                //if (this.panel6.Controls.ContainsKey("exceptLabel2"))
                //{
                //    Label exceptLabel2 = (Label)this.panel6.Controls.Find("exceptLabel2", true)[0];
                //    string name2 = exceptLabel2.Text;
                //    strLists.Add(name2);
                //}
                //if (this.panel6.Controls.ContainsKey("exceptLabel3"))
                //{
                //    Label exceptLabel3 = (Label)this.panel6.Controls.Find("exceptLabel3", true)[0];
                //    string name3 = exceptLabel3.Text;
                //    strLists.Add(name3);
                //}
                string fileExtention = file.Extension;
                if (strLists.Count > 0)
                {
                    foreach (String fe in strLists)
                    {
                        if (fe == fileExtention)
                        {
                            saveFlg = false;
                            break;
                        }
                    }
                }
            }
            return saveFlg;
        }
        private MonitorServerFolder monitorFolderDataFormat(FileSystemInfo fileInfo, string initFlg)
        {
            string sb = fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('\\'));
            FileTypeSet fileTypeSet = this.iftsSvc.GetFileTypeSetByMonitorServerIdAndFolderName(this.cobMonitorServerList.SelectedValue.ToString(), sb);
            if (fileTypeSet != null)
            {
                if (fileTypeSet.hiddenFileFlg == "1")
                {//隠しファイル
                    if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        return null;
                    }
                }
                if (fileTypeSet.systemFileFlg == "1")
                {//システムファイル
                    if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        return null;
                    }
                }
            }
            MonitorServerFolder msf = new MonitorServerFolder();
            msf.monitorServerID = Convert.ToInt32(this.cobMonitorServerList.SelectedValue.ToString());
            if (fileInfo is DirectoryInfo)
            {
                if (fileInfo.Name == fileInfo.FullName)
                {
                    msf.monitorFilePath = fileInfo.Name.TrimEnd('\\');
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


        private void lists_remove(MonitorServerFolder msf)
        {
            for (int i = lists.Count - 1; i >= 0; i--)
            {
                if ((lists[i].monitorFilePath + "\\" + lists[i].monitorFileName).Equals(msf.monitorFilePath + "\\" + msf.monitorFileName))
                {
                    lists.RemoveAt(i);
                }
            }
        }

        private void cobMonitorServerList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lists != null && lists.Count > 0)
            {
                lists.Clear();
            }
            if (this.cobMonitorServerList.SelectedValue != null && this.cobMonitorServerList.SelectedIndex > 0)
            {
                getAndInitPageDefault();
                //formatListsByFileTypeSet();
            }
            treeListInit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cobMonitorServerList_SelectedValueChanged(sender, e);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            doFlag = true;
            loadFlg = true;
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
        }

        /// <summary>
        /// 保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.cobMonitorServerList.SelectedValue != null)
            {
                this.btnSave.Enabled = false;
                if (MsgHelper.QuestionMsg(ValidationRegex.Q001, ValidationRegex.publicTitle))
                {
                    ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(SaveOperation));
                    MsgHelper.InfoMsg(ValidationRegex.C003, ValidationRegex.publicTitle);
                }
                this.btnSave.Enabled = true;
            }
        }

        /// <summary>
        /// 画面設定はDBに保存する
        /// </summary>
        private void SaveOperation()
        {
            if (lists == null)
            {
                this.btnSave.Enabled = true;
                return;
            }
            try
            {
                string monitorServerId = this.cobMonitorServerList.SelectedValue.ToString();
                MonitorServer ms = this.imsSvc.GetMonitorServerById(Int32.Parse(monitorServerId));
                // 20140321 wang web対応
                string monitorRootPath = @"\\" + ms.monitorServerIP + @"\" + ms.startFile.TrimEnd('\\').TrimStart('\\');
                int del = this.imsfSvc.DeleteMonitorServerFolderByServerId(Convert.ToInt32(monitorServerId));
                List<System.IO.FileInfo> saveFiles = new List<System.IO.FileInfo>();
                List<System.IO.DirectoryInfo> saveDirs = new List<System.IO.DirectoryInfo>();
                if (del > -1)
                {
                    tempRemoveLists.Clear();

                    //取消根节点时，清空。
                    if (this.treeList1.Nodes.Count > 0 && this.treeList1.Nodes.FirstNode.CheckState == CheckState.Unchecked)
                    {
                        lists.Clear();
                    }

                    //允许用户添加除外的文件，所以不再次过滤——2014-7-15 wjd commented
                    //formatListsByFileTypeSet();

                    foreach (MonitorServerFolder msfl in lists)
                    {
                        string sb = "";
                        //if ((msfl.monitorFilePath + "\\" + msfl.monitorFileName).Equals(ms.monitorDrive))
                        if ((msfl.monitorFilePath + "\\" + msfl.monitorFileName).Equals(monitorRootPath))
                        {
                            msfl.monitorFileName = "";
                            sb = msfl.monitorFilePath;
                        }
                        else
                        {
                            sb = msfl.monitorFilePath + "\\" + msfl.monitorFileName;
                        }

                        if (System.IO.File.Exists(sb))
                        {
                            FileInfo file = new FileInfo(sb);
                            if (file.Exists)
                            {
                                //允许用户添加除外的文件，所以不再次过滤——2014-7-15 wjd commented
                                //if (saveMonitorFile(file))
                                {
                                    MonitorServerFolder msf = monitorFolderDataFormat(file, "1");
                                    if (msf == null) { continue; }
                                    int id = this.imsfSvc.InsertMonitorServerFolder(msf);
                                    // チェックしたファイル
                                    saveFiles.Add(file);
                                }
                            }
                        }
                        else if (System.IO.Directory.Exists(sb))
                        {
                            DirectoryInfo di = new DirectoryInfo(sb);
                            MonitorServerFolder msf = monitorFolderDataFormat(di, "1");
                            if (msf == null) { continue; }
                            int id = this.imsfSvc.InsertMonitorServerFolder(msf);

                            //取消递归子节点——2014-8-28 wjd commented
                            //getChildNode(di);

                            // チェックしたフォルダー
                            //saveDirs.Add(di);
                            //// フォルダーの下のファイル
                            //IEnumerable<System.IO.FileInfo> diFilelist = GetFiles(di.FullName);
                            //foreach (FileInfo checkedFileInfo in diFilelist)
                            //{
                            //    saveFiles.Add(checkedFileInfo);
                            //}
                        }
                    }
                }
                // 削除チェックを外すローカル対象

                //DirectoryInfo localDirInfo = new DirectoryInfo(ms.monitorLocalPath);
                //IEnumerable<System.IO.FileInfo> allFilelist = GetFiles(localDirInfo.FullName);
                //IEnumerable<System.IO.DirectoryInfo> allDirlist = GetDirs(localDirInfo.FullName);

                // ファイルの削除
                //var queryFileDelList = from sfile in allFilelist
                //                       where !(
                //                           from allfile in allFilelist
                //                           from savefile in saveFiles
                //                           where allfile.Name == savefile.Name
                //                           select allfile).Contains(sfile)
                //                       select sfile;
                //if (queryFileDelList.Count() > 0)
                //{
                //    foreach (var v in queryFileDelList)
                //    {
                //        try
                //        {
                //            if (v.Exists)
                //            {
                //                v.IsReadOnly = false;
                //                v.Delete();
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            logger.Error(v.FullName + ex.Message);
                //            continue;
                //        }
                //    }
                //}

                // フォルダーの削除
                //var queryDirDelList = from sdir in allDirlist
                //                      where !(
                //                          from alldir in allDirlist
                //                          from savedir in saveDirs
                //                          where alldir.Name == savedir.Name
                //                          select alldir).Contains(sdir)
                //                      select sdir;
                //if (queryDirDelList.Count() > 0)
                //{
                //    foreach (var v in queryDirDelList)
                //    {
                //        try
                //        {
                //            if (System.IO.Directory.Exists(v.FullName))
                //            {
                //                v.Delete(true);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            logger.Error(v.FullName + ex.Message);
                //            continue;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// サブノードはDBに保存する
        /// </summary>
        private void getChildNode(DirectoryInfo sb)
        {
            try
            {
                if (sb is DirectoryInfo)
                {
                    FileSystemInfo[] fsis = sb.GetFileSystemInfos();
                    if (fsis.Count() > 0)
                    {
                        foreach (FileSystemInfo fsi in fsis)
                        {
                            try
                            {
                                if (fsi is FileInfo)
                                {
                                    if (fsi.Exists)
                                    {
                                        if (saveMonitorFile((FileInfo)fsi))
                                        {
                                            MonitorServerFolder msf = monitorFolderDataFormat(fsi, "0");
                                            if (msf == null) { continue; }
                                            int id = this.imsfSvc.InsertMonitorServerFolder(msf);
                                        }
                                    }
                                }
                                else
                                {
                                    MonitorServerFolder msf = monitorFolderDataFormat(fsi, "0");
                                    if (msf == null) { continue; }
                                    int id = this.imsfSvc.InsertMonitorServerFolder(msf);
                                    getChildNode((DirectoryInfo)fsi);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message + " \r\n\t   FileSystemInfo fsi: " + fsi.FullName + "\r\n");
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
                logger.Error(ex.Message + " \r\n\t   DirectoryInfo sb: " + sb.FullName + "\r\n");
            }
        }

        /// <summary>
        /// 加载其他程序配置中的时间
        /// </summary>
        private void tabPage7_Load()
        {
            try
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "").Replace(@"bin\Release\", "");
                //BBS起動時間
                var bbsConfigPath = Path.Combine(basePath, @"..\BudFileListen\BudFileListen.exe.config");
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(bbsConfigPath);
                var bbsStartNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='BatchStartTime']");
                if (bbsStartNode != null)
                {
                    txtBBSStartTime.Text = bbsStartNode.Attributes["value"].Value;
                }

                //日志确认时间
                var logConfirmNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='LogConfirmTime']");
                if (logConfirmNode != null)
                {
                    txtLogConfirmTime.Text = logConfirmNode.Attributes["value"].Value;
                }

                //SSH起動時間
                var sshConfigPath = Path.Combine(basePath, @"..\BudSSH\BudSSH.exe.config");
                xmlDoc.Load(sshConfigPath);
                var sshStartNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='SSHBootTime']");
                if (sshStartNode != null)
                {
                    txtSSHStartTime.Text = sshStartNode.Attributes["value"].Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetermine_Click(object sender, EventArgs e)
        {
            try
            {
                var bbsStartTimeError = ValidationRegex.ValidateTime("BBS起動時間", txtBBSStartTime.Text);
                if (bbsStartTimeError != string.Empty)
                {
                    MsgHelper.WarningMsg(bbsStartTimeError, ValidationRegex.publicTitle);
                    return;
                }
                var sshStartTimeError = ValidationRegex.ValidateTime("SSH起動時間", txtSSHStartTime.Text);
                if (sshStartTimeError != string.Empty)
                {
                    MsgHelper.WarningMsg(sshStartTimeError, ValidationRegex.publicTitle);
                    return;
                }
                var logConfirmTimeError = ValidationRegex.ValidateTime("ログ確認時間", txtLogConfirmTime.Text);
                if (logConfirmTimeError != string.Empty)
                {
                    MsgHelper.WarningMsg(logConfirmTimeError, ValidationRegex.publicTitle);
                    return;
                }

                var basePath = AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "").Replace(@"bin\Release\", "");

                //BBS起動時間
                var bbsConfigPath = Path.Combine(basePath, @"..\BudFileListen\BudFileListen.exe.config");
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(bbsConfigPath);
                var bbsStartNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='BatchStartTime']");
                if (bbsStartNode != null)
                {
                    bbsStartNode.Attributes["value"].Value = CommonUtil.ToShortTimeString(txtBBSStartTime.Text);
                }
                var bbsEndNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='BatchEndTime']");
                if (bbsEndNode != null)
                {
                    bbsEndNode.Attributes["value"].Value = DateTime.Parse(txtBBSStartTime.Text).AddHours(6).ToString("HH:mm");
                }

                //日志确认时间
                var logConfirmNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='LogConfirmTime']");
                if (logConfirmNode != null)
                {
                    logConfirmNode.Attributes["value"].Value = CommonUtil.ToShortTimeString(txtLogConfirmTime.Text);
                }
                else
                {
                    //没有则添加
                    var appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");
                    if (appSettingsNode != null)
                    {
                        //注释
                        var comment = xmlDoc.CreateComment(" ログ確認時間");
                        appSettingsNode.AppendChild(comment);
                        //节点
                        logConfirmNode = xmlDoc.CreateElement("add");
                        logConfirmNode.Attributes.Append(xmlDoc.CreateAttribute("key"));
                        logConfirmNode.Attributes["key"].Value = "LogConfirmTime";
                        logConfirmNode.Attributes.Append(xmlDoc.CreateAttribute("value"));
                        logConfirmNode.Attributes["value"].Value = CommonUtil.ToShortTimeString(txtLogConfirmTime.Text);
                        appSettingsNode.AppendChild(logConfirmNode);
                    }
                }
                xmlDoc.Save(bbsConfigPath);

                //SSH起動時間
                var sshConfigPath = Path.Combine(basePath, @"..\BudSSH\BudSSH.exe.config");
                xmlDoc.Load(sshConfigPath);
                var sshBootNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='SSHBootTime']");
                if (sshBootNode != null)
                {
                    sshBootNode.Attributes["value"].Value = CommonUtil.ToShortTimeString(txtSSHStartTime.Text);
                }
                var sshLocalNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='SSHLocalSyncTime']");
                if (sshLocalNode != null)
                {
                    sshLocalNode.Attributes["value"].Value = DateTime.Parse(txtSSHStartTime.Text).AddHours(1).ToString("HH:mm");
                }
                var sshDBSyncNode = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='DBSyncTime']");
                if (sshDBSyncNode != null)
                {
                    sshDBSyncNode.Attributes["value"].Value = DateTime.Parse(txtSSHStartTime.Text).AddHours(2).ToString("HH:mm");
                }
                xmlDoc.Save(sshConfigPath);

                MsgHelper.WarningMsg(ValidationRegex.I001, ValidationRegex.publicTitle);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                MsgHelper.WarningMsg("保存に失敗しました。", ValidationRegex.publicTitle);
            }
        }
    }
}
