using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using IBLL;
using Model;
using System.Threading;
using System.Reflection;
using log4net;
using System.Collections;

namespace BudFileTransfer
{
    public partial class BudFileTransfer : Form
    {
        public BudFileTransfer()
        {
            InitializeComponent();
        }
        private IMonitorServerService MonitorServerService = BLLFactory.ServiceAccess.CreateMonitorServerService();
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudFileTransfer_Load(object sender, EventArgs e)
        {
            try
            {                
                dgrdMain_Load();
                ThreadTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                MsgHelper.InfoMsg("失敗しました", "失敗提示");                
                Application.Exit();
            }
        }
        /// <summary>
        /// 当双击状态栏的小图标时，使窗口恢复原来大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
        /// <summary>
        /// 窗口关闭时最小化到状态栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudFileTransfer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;            
            this.Hide();
        }
        /// <summary>
        /// 窗口最小化到状态栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudFileTransfer_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }
        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
        /// <summary>
        /// 
        /// </summary>        
        private void dgrdMain_Load()
        {
            //init dgrdMain 
            //not allowed user add new row to datagridview
            this.dgrdMain.AllowUserToAddRows = false;
            this.dgrdMain.Rows.Clear();
            //init table data
            IBackupServerGroupService bss = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
            IList<MonitorServer> lists = MonitorServerService.GetMonitorServerList();
            int i = 1;
            foreach (MonitorServer list in lists)
            {
                string monitorServerId = list.id;
                string backupGroupid = "0";
                DataGridViewRow dgvr = new DataGridViewRow();
                try
                {
                    foreach (DataGridViewColumn c in dgrdMain.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells                    
                    }

                    dgvr.Cells[0].Value = list.id;
                    dgvr.Cells[1].Value = i.ToString();
                    dgvr.Cells[2].Value = list.monitorServerName;
                    BackupServerGroup result = bss.GetBackupServerGroupByMonitorId(Int32.Parse(list.id));
                    if (result != null)
                    {
                        dgvr.Cells[3].Value = result.backupServerGroupName; dgvr.Cells[3].Tag = result.id;
                        backupGroupid = result.id;
                    }
                    else
                    {
                        dgvr.Cells[3].Value = "バックアップ先選択"; dgvr.Cells[3].Tag = 0;
                    }
                    dgvr.Cells[4].Value = "";
                    dgvr.Cells[5].Value = "転送始める"; dgvr.Cells[5].Tag = 0;
                    dgvr.Cells[6].Value = "転送停止";
                    this.dgrdMain.Rows.Add(dgvr);
                    int rowIndex = dgvr.Cells[0].RowIndex;
                    if (backupGroupid != "0" && !GlobalVariable.stopTransferThread.ContainsKey(list.id))
                    {
                        bool transfer = ButtonTansferStart(list.id, backupGroupid, rowIndex);
                    }
                    i++;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    continue;
                }
            }            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgrdMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewColumn column = dgrdMain.Columns[e.ColumnIndex];
                    if (column is DataGridViewButtonColumn)
                    {
                        int CIndex = e.ColumnIndex;
                        //get the column index 
                        //get the monitor server record id 
                        string monitorServerId = dgrdMain[0, e.RowIndex].Value.ToString();
                        string backupGroupid = dgrdMain[3, e.RowIndex].Tag.ToString();
                        if (CIndex == 5)
                        {                            
                            if (backupGroupid != "0")
                            {
                                if (dgrdMain[5, e.RowIndex].Tag.ToString() == "0")
                                {
                                    //Transfer
                                    bool result = ButtonTansferStart(monitorServerId, backupGroupid,e.RowIndex);
                                    if (!result)
                                    {
                                        MsgHelper.InfoMsg("転送が失敗しました", "失敗提示");
                                    }
                                }
                                else
                                {
                                    MsgHelper.InfoMsg("今転送中します", "成功提示");
                                }
                            }
                            else
                            {
                                MsgHelper.InfoMsg("バックアップ先未選択", "失敗提示");
                            }
                        }
                        if (CIndex == 6)
                        {
                            ButtonTansferStop(monitorServerId,e.RowIndex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorServerId"></param>
        /// <param name="backupGroupid"></param>
        /// <param name="rowIndex"></param>
        private bool ButtonTansferStart(string monitorServerId, string backupGroupid, int rowIndex)
        {
            bool result = false;
            try
            {
                //Transfer                
                if (GlobalVariable.listThread.ContainsKey(monitorServerId))
                {
                    dgrdMain[5, rowIndex].Tag = 1;
                    dgrdMain[4, rowIndex].Style.BackColor = Color.Green;
                    result = true;
                }
                else
                {
                    TransferThread TransferFolder = new TransferThread(monitorServerId, backupGroupid);
                    GlobalVariable.listTransferThread.Add(monitorServerId, TransferFolder);
                    System.Threading.Thread TransferThread = new System.Threading.Thread(new ThreadStart(TransferFolder.TransferStart));
                    TransferThread.Name = "FileTransfer" + monitorServerId;
                    GlobalVariable.listThread.Add(monitorServerId, TransferThread);
                    try
                    {
                        ((System.Threading.Thread)GlobalVariable.listThread[monitorServerId]).Start();
                    }
                    catch (System.Threading.ThreadStateException ex)
                    {
                        logger.Error(ex.Message);
                    }
                    catch (System.OutOfMemoryException ex)
                    {
                        logger.Error(ex.Message);
                    }
                    if (((System.Threading.Thread)GlobalVariable.listThread[monitorServerId]).ThreadState == ThreadState.Running)
                    {
                        dgrdMain[5, rowIndex].Tag = 1;
                        dgrdMain[4, rowIndex].Style.BackColor = Color.Green;
                        logger.Info(dgrdMain[2, rowIndex].Value.ToString() + "は転送が始めしました");
                        if (GlobalVariable.stopTransferThread.ContainsKey(monitorServerId))
                        {
                            GlobalVariable.stopTransferThread.Remove(monitorServerId);
                        }
                        result = true;
                    }
                    else
                    {
                        dgrdMain[4, rowIndex].Style.BackColor = Color.Red;
                        logger.Info(dgrdMain[2, rowIndex].Value.ToString() + "は転送が失敗しました");
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                result = false;
            }
            return result;            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorServerId"></param>
        /// <param name="backupGroupid"></param>
        /// <param name="rowIndex"></param>
        private void ButtonTansferStop(string monitorServerId, int rowIndex)
        {
            if (GlobalVariable.listThread.ContainsKey(monitorServerId))
            {
                //if (((System.Threading.Thread)GlobalVariable.listThread[monitorServerId]).ThreadState == ThreadState.Running)
                //{
                ((TransferThread)GlobalVariable.listTransferThread[monitorServerId]).TransferStop();
                GlobalVariable.listTransferThread.Remove(monitorServerId);
                ((System.Threading.Thread)GlobalVariable.listThread[monitorServerId]).Abort();
                GlobalVariable.listThread.Remove(monitorServerId);
                //记录被手动停止的对象
                GlobalVariable.stopTransferThread.Add(monitorServerId, "stop");
                dgrdMain[5, rowIndex].Tag = 0;
                dgrdMain[4, rowIndex].Style.BackColor = Color.Red;
                logger.Info(dgrdMain[2, rowIndex].Value.ToString() + "は転送が停止しました");
                //}
            }
        }
        /// <summary>
        /// 5s刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThreadTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //毎隔5s重新获取监视对象
                IList<MonitorServer> monitorServer = MonitorServerService.GetMonitorServerList();
                // ループ変数のバグ対応 2014/01/31 王丹
                Hashtable listenThreadList = (Hashtable)GlobalVariable.listThread.Clone();
                //判断是否存在被删除监视对象，存在停止传送
                if (listenThreadList != null)
                {
                    foreach (DictionaryEntry tempThread in listenThreadList)
                    {
                        bool isexit = false;
                        foreach (MonitorServer m in monitorServer)
                        {
                            if (tempThread.Key.ToString() == m.id)
                            {
                                isexit = true;
                            }
                        }
                        if (!isexit)
                        {
                            try
                            {
                                ((TransferThread)GlobalVariable.listTransferThread[tempThread.Key]).TransferStop();
                                GlobalVariable.listTransferThread.Remove(tempThread.Key);
                                ((System.Threading.Thread)GlobalVariable.listThread[tempThread.Key]).Abort();
                                GlobalVariable.listThread.Remove(tempThread.Key);
                                //如果手动停止的对象被删除了
                                if (GlobalVariable.stopTransferThread.ContainsKey(tempThread.Key))
                                {
                                    GlobalVariable.stopTransferThread.Remove(tempThread.Key);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                                continue;
                            }
                        }
                    }
                }
                dgrdMain_Load();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                //MsgHelper.InfoMsg("エラー発生しました", "失敗提示");
            }
        }        
    }
}
