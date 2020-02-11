using BudFileListen.BLL;
using BudFileListen.Common;
using BudFileListen.Entities;
using BudFileListen.Models;
using Common;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BudFileListen
{
    public partial class BudFileListen : Form
    {
        /// <summary>
        /// SynchronizingTime
        /// </summary>
        private int SynchronizingTime = int.Parse(ConfigurationManager.AppSettings["SynchronizingTime"]);
        /// <summary>
        /// monitorServerTopDirList
        /// </summary>
        private Hashtable monitorServerTopDirList = new Hashtable();
        /// <summary>
        /// monitorServerManager
        /// </summary>
        MonitorServerManager monitorServerManager = new MonitorServerManager();
        /// <summary>
        /// monitorServerFolderManager
        /// </summary>
        MonitorServerFolderManager monitorServerFolderManager = new MonitorServerFolderManager();
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 共有
        /// </summary>
        private List<string> shareFolderConnetList = new List<string>();
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string MTNum = ConfigurationManager.AppSettings["MTNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string RNum = ConfigurationManager.AppSettings["RNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string WNum = ConfigurationManager.AppSettings["WNum"];
        /// <summary>
        /// CopyListenTime
        /// </summary>
        private string copyListenTime = ConfigurationManager.AppSettings["CopyListenTime"].ToString();
        /// <summary>
        /// TransferTime
        /// </summary>
        private string transferTime = ConfigurationManager.AppSettings["TransferTime"].ToString();
        /// <summary>
        /// BatchStartTime
        /// </summary>
        private string batchStartTime = ConfigurationManager.AppSettings["BatchStartTime"].ToString();
        /// <summary>
        /// BatchEndTime
        /// </summary>
        private string batchEndTime = ConfigurationManager.AppSettings["BatchEndTime"].ToString();
        /// <summary>
        /// LogConfirmTime
        /// </summary>
        private string logConfirmTime = ConfigurationManager.AppSettings["LogConfirmTime"];
        /// <summary>
        /// 自動　手動
        /// </summary>
        private string controlFlg = ConfigurationManager.AppSettings["ControlFlg"].ToString();
        /// <summary>
        /// 監視対象のリスト
        /// </summary>
        List<monitorServer> monitorServerList = new List<monitorServer>();
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
        const string DEFAULTCHAR_VALUE = "";
        /// <summary>
        /// 日付時間フィールドのディフォルト値
        /// </summary>
        DateTime DEFAULTDATETIME_VALUE = new DateTime(1900, 1, 1, 0, 0, 0);
        /// <summary>
        /// 数字フィールドのディフォルト値
        /// </summary>
        const int DEFAULTINT_VALUE = 0;

        /// <summary>
        /// 只传送
        /// </summary>
        private bool IsTransferOnly = false;
        /// <summary>
        /// 転送プロセスList
        /// </summary>
        private Hashtable transferProcessIDList = new Hashtable();
        /// <summary>
        /// 削除トップフォルダーリスト(削除エラー、次に再度削除)
        /// </summary>
        private List<string> DelDirForErrorDelList = new List<string>();
        /// <summary>
        /// ログ報告実行
        /// </summary>
        private bool roBocopyDoFlg = true;
        /// <summary>
        /// 実行開始時間
        /// </summary>
        private DateTime EXEDoStartTime;
        /// <summary>
        /// 実行完了時間
        /// </summary>
        private DateTime EXEDoEndTime;
        /// <summary>
        /// 監視対象Conhostのリスト
        /// </summary>
        List<int> processIDConhostList = new List<int>();
        /// <summary>
        /// 監視対象Cmdのリスト
        /// </summary>
        List<int> processIDCmdList = new List<int>();
        /// <summary>
        /// 再拷贝的服务器ID
        /// </summary>
        List<int> reCopyServerIDList = new List<int>();
        /// <summary>
        /// Form Loaded DateTime
        /// </summary>
        DateTime formLoadedTime;
        /// <summary>
        /// Day Doing Open
        /// </summary>
        bool DayDoingOpenFlg = false;

        public BudFileListen()
        {
            InitializeComponent();
            formLoadedTime = DateTime.Now;
        }
        /// <summary>
        /// winform loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudFileListen_Load(object sender, EventArgs e)
        {
            try
            {
                //Process[] ps = Process.GetProcesses();
                //foreach (Process item in ps)
                //{
                //    if (item.ProcessName.Equals("Robocopy"))
                //    {
                //        item.Kill();
                //    }
                //}
                Process[] allRobocopyProcesses = Process.GetProcessesByName("Robocopy");
                if (allRobocopyProcesses.Count() > 0)
                {
                    foreach (Process roboProcess in allRobocopyProcesses)
                    {
                        roboProcess.Kill();
                    }
                }
                // 自動
                if (controlFlg.Equals("1"))
                {
                    DateTime dtstart = DateTime.Parse(batchStartTime);
                    DateTime dtend = DateTime.Parse(batchEndTime);
                    labelBatchDisplay.Text = "執行期間：" + dtstart.ToString("HH:mm") + "から" + dtend.ToString("HH:mm") + "まで";
                    EXEDoStartTime = DateTime.Now;
                    //if (DateTime.Now >= dtstart && DateTime.Now <= dtend)
                    //{
                    //    monitorServerList = monitorServerManager.GetMonitorServerList();
                    //    TabelLayout(monitorServerList);
                    //    tablePanel.Visible = true;
                    //}
                }
                //
                monitorServerList = monitorServerManager.GetMonitorServerList();
                TabelLayout(monitorServerList);
                tablePanel.Visible = true;
                //
                timerForRoboCopy.Interval = SynchronizingTime;
                timerForRoboCopy.Elapsed += new System.Timers.ElapsedEventHandler(SynchronizingWatchStart);
                timerForRoboCopy.Start();
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
        private void BudFileListen_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        /// <summary>
        /// 窗口最小化到状态栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudFileListen_SizeChanged(object sender, EventArgs e)
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
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            try
            {
                // RoboCopyのクリア
                Process[] psRobocopyList = Process.GetProcessesByName("Robocopy");
                foreach (Process item in psRobocopyList)
                {
                    item.Kill();
                }
                // cmd
                Process[] psCmdList = Process.GetProcessesByName("cmd");
                foreach (Process item in psCmdList)
                {
                    if (processIDCmdList.Contains(item.Id))
                    {
                        item.Kill();
                        processIDCmdList.Remove(item.Id);
                    }
                }
                // conhost
                Process[] psConhostList = Process.GetProcessesByName("conhost");
                foreach (Process item in psConhostList)
                {
                    if (processIDConhostList.Contains(item.Id))
                    {
                        item.Kill();
                        processIDConhostList.Remove(item.Id);
                    }
                }

                foreach (monitorServer monitorServerInfo in monitorServerList)
                {
                    if (monitorServerInfo.deleteFlg == 1)
                    {
                        monitorServerInfo.deleteFlg = 0;
                        monitorServerManager.Edit(monitorServerInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
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
        /// <param name="monitorServer"></param>
        private void TabelLayout(List<Entities.monitorServer> monitorServerList)
        {
            int line = monitorServerList.Count() + 1;
            int columns = 5;
            int width = 150;
            int height = 40;
            tablePanel.Controls.Clear();
            Size a = new Size();
            a.Width = this.Width - 80;
            a.Height = line * height;
            tablePanel.Location = new Point(30, 60);
            tablePanel.BackColor = Color.White;
            tablePanel.Size = a;
            tablePanel.Refresh();
            tablePanel.RowCount = line;
            tablePanel.ColumnCount = columns;
            for (int h = 0; h < tablePanel.RowStyles.Count; h++)
            {
                tablePanel.RowStyles[h].SizeType = SizeType.Absolute;
                tablePanel.RowStyles[h].Height = height;
            }
            for (int c = 0; c < tablePanel.ColumnStyles.Count; c++)
            {
                tablePanel.ColumnStyles[c].SizeType = SizeType.Absolute;
                tablePanel.ColumnStyles[c].Width = width;
            }
            //バックアップ元名
            Label title1 = new Label();
            title1.Text = "バックアップ元名称";
            title1.Anchor = AnchorStyles.None;
            title1.TextAlign = ContentAlignment.MiddleCenter;
            title1.AutoSize = false;
            title1.Height = 30;
            title1.Width = 150;
            tablePanel.Controls.Add(title1, 0, 0);
            //IPアドレス
            Label title2 = new Label();
            title2.Text = "IPアドレス";
            title2.Anchor = AnchorStyles.None;
            title2.TextAlign = ContentAlignment.MiddleCenter;
            title2.AutoSize = false;
            title2.Height = 30;
            title2.Width = 150;
            tablePanel.Controls.Add(title2, 1, 0);
            //ステータス
            Label title3 = new Label();
            title3.Text = "ステータス";
            title3.Anchor = AnchorStyles.None;
            title3.TextAlign = ContentAlignment.MiddleCenter;
            title3.AutoSize = false;
            title3.Height = 30;
            title3.Width = 150;
            tablePanel.Controls.Add(title3, 2, 0);
            //button
            Label title4 = new Label();
            title4.Text = "始める";
            title4.Anchor = AnchorStyles.None;
            title4.TextAlign = ContentAlignment.MiddleCenter;
            title4.AutoSize = false;
            title4.Height = 30;
            title4.Width = 100;
            tablePanel.Controls.Add(title4, 3, 0);
            //button
            Label title5 = new Label();
            title5.Text = "停止";
            title5.Anchor = AnchorStyles.None;
            title5.TextAlign = ContentAlignment.MiddleCenter;
            title5.AutoSize = false;
            title5.Height = 30;
            title5.Width = 100;
            tablePanel.Controls.Add(title5, 4, 0);

            for (int i = 0; i < monitorServerList.Count(); i++)
            {
                //バックアップ元名称
                Label name = new Label();
                name.Name = "name" + monitorServerList[i].id;
                name.Text = monitorServerList[i].monitorServerName;
                name.Anchor = AnchorStyles.None;
                name.TextAlign = ContentAlignment.MiddleCenter;
                name.AutoSize = false;
                name.Height = 30;
                name.Width = 150;
                tablePanel.Controls.Add(name, 0, i + 1);
                //IPアドレス
                Label ip = new Label();
                ip.Name = "ip" + monitorServerList[i].id;
                ip.Text = monitorServerList[i].monitorServerIP;
                ip.Anchor = AnchorStyles.None;
                ip.TextAlign = ContentAlignment.MiddleCenter;
                ip.AutoSize = false;
                ip.Height = 30;
                ip.Width = 150;
                tablePanel.Controls.Add(ip, 1, i + 1);
                //ステータス
                Label status = new Label();
                status.Name = "status" + monitorServerList[i].id;
                status.Text = "未バックアップ";
                status.Anchor = AnchorStyles.None;
                status.TextAlign = ContentAlignment.MiddleCenter;
                status.AutoSize = false;
                status.BackColor = Color.Red;
                status.Height = 30;
                status.Width = 150;
                tablePanel.Controls.Add(status, 2, i + 1);
                //button
                Button listen = new Button();
                listen.Name = "L" + monitorServerList[i].id.ToString();
                listen.Text = "始める";
                listen.Anchor = AnchorStyles.None;
                listen.TextAlign = ContentAlignment.MiddleCenter;
                listen.Height = 30;
                listen.Width = 80;
                listen.MouseClick += new MouseEventHandler(this.ButtonListenStart);
                tablePanel.Controls.Add(listen, 3, i + 1);
                //button
                Button stop = new Button();
                stop.Name = "T" + monitorServerList[i].id.ToString();
                stop.Text = "停止";
                stop.Anchor = AnchorStyles.None;
                stop.TextAlign = ContentAlignment.MiddleCenter;
                stop.Height = 30;
                stop.Width = 80;
                stop.MouseClick += new MouseEventHandler(this.ButtonListenStop);
                tablePanel.Controls.Add(stop, 4, i + 1);
                // 自动バックアップ始める
                this.ListenStart(int.Parse(monitorServerList[i].id.ToString()));
            }
        }
        /// <summary>
        /// Button click バックアップ始める
        /// </summary>
        /// <param name="m"></param>
        private void ButtonListenStart(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int monitorServerId = int.Parse(b.Name.TrimStart('L'));
            bool listen = this.ListenStart(monitorServerId);
            if (!listen)
            {
                MsgHelper.InfoMsg("バックアップが失敗しました", "失敗提示");
            }
        }

        /// <summary>
        /// バックアップstop
        /// </summary>
        /// <param name="m"></param>
        private void ButtonListenStop(object sender, EventArgs e)
        {
            try
            {
                Button b = (Button)sender;
                int monitorServerId = int.Parse(b.Name.TrimStart('T'));
                // 監視中止
                monitorServer MonitorServer = monitorServerManager.GetMonitorServerById(monitorServerId);
                MonitorServer.deleteFlg = 1;
                monitorServerManager.Edit(MonitorServer);
                //
                this.ListenStatus(monitorServerId.ToString(), "未バックアップ", Color.Red);
                logger.Info("[" + MonitorServer.monitorServerIP + "]はバックアップが停止しました");
                this.ListenButton("L" + monitorServerId, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// バックアップ始める
        /// </summary>
        /// <param name="monitorServerId"></param>
        /// <returns></returns>
        private bool ListenStart(int monitorServerId)
        {
            bool result = false;
            try
            {
                this.ListenButton("L" + monitorServerId, false);
                monitorServer MonitorServer = monitorServerManager.GetMonitorServerById(monitorServerId);
                // 監視復旧
                if (MonitorServer.deleteFlg == 1)
                {
                    MonitorServer.deleteFlg = 0;
                    monitorServerManager.Edit(MonitorServer);
                }
                // MACの場合
                if (MonitorServer.monitorSystem == 1)
                {
                    string monitorSrcPath = @"\\" + MonitorServer.monitorServerIP + @"\" + MonitorServer.startFile.TrimStart('\\');
                    string monitorTargetPath = MonitorServer.monitorLocalPath.TrimEnd('\\') + "\\";
                    // フォルダー情報取得
                    DirectoryInfo directoryInfo = new DirectoryInfo(monitorSrcPath);
                    // Bug管理表13番
                    if (monitorServerTopDirList.ContainsKey(directoryInfo.FullName))
                    {
                        monitorServerTopDirList.Remove(directoryInfo.FullName);
                        monitorServerTopDirList.Add(directoryInfo.FullName, getDirectoryFullInfo(directoryInfo));
                    }
                    else
                    {
                        monitorServerTopDirList.Add(directoryInfo.FullName, getDirectoryFullInfo(directoryInfo));
                    }
                    // IP判断
                    if (!shareFolderConnetList.Contains(MonitorServer.monitorServerIP))
                    {
                        shareFolderConnetList.Add(MonitorServer.monitorServerIP);
                    }
                    try
                    {
                        // ここで実際に利用しない
                        List<string> topDirDelList = new List<string>();
                        // 自動
                        if (controlFlg.Equals("1"))
                        {
                            DateTime dtstart = DateTime.Parse(batchStartTime);
                            DateTime dtend = DateTime.Parse(batchEndTime);
                            //设置的拷贝开始时间晚于程序启动时间时，执行拷贝
                            if (DateTime.Now >= dtstart && DateTime.Now <= dtend && dtstart > formLoadedTime)
                            {
                                RoboCopyStart(monitorServerId, MonitorServer.monitorServerIP, MonitorServer.account, MonitorServer.password, monitorSrcPath, monitorTargetPath, MonitorServer.copyInit, topDirDelList);
                            }
                        }
                        else
                        {
                            RoboCopyStart(monitorServerId, MonitorServer.monitorServerIP, MonitorServer.account, MonitorServer.password, monitorSrcPath, monitorTargetPath, MonitorServer.copyInit, topDirDelList);
                        }
                        // 
                        //RoboCopyStart(monitorServerId, MonitorServer.monitorServerIP, MonitorServer.account, MonitorServer.password, monitorSrcPath, monitorTargetPath, MonitorServer.copyInit, topDirDelList);
                        //
                        this.ListenStatus(monitorServerId.ToString(), "バックアップ中", Color.Green);
                        logger.Info("[" + MonitorServer.monitorServerIP + "]はバックアップが始めしました");
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        this.ListenButton("L" + monitorServerId, true);
                        logger.Info("[" + MonitorServer.monitorServerIP + "]はバックアップが失敗しました");
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ListenButton("L" + monitorServerId, true);
                logger.Error(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// RoboCopy同期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynchronizingWatchStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                #region 
                // 同期のTIMER中止
                timerForRoboCopy.Enabled = false;
                // エラー削除執行
                if (DelDirForErrorDelList.Count > 0)
                {
                    foreach (string errorDirPath in DelDirForErrorDelList)
                    {
                        TopDirDirectDelete(errorDirPath);
                    }
                }
                // RoboCopyのプロセス確認
                bool robocopyProcessFlg = false;
                // プロセスIDリスト
                List<int> rocoCopyProcessIDList = new List<int>();
                Process[] ps = Process.GetProcessesByName("Robocopy");
                if (ps.Count() > 0)
                {
                    foreach (Process processTask in ps)
                    {
                        rocoCopyProcessIDList.Add(processTask.Id);
                    }
                    // RoboCopyのプロセス判断——代表RoboCopy进程全是传送进程，即拷贝已完成
                    if (rocoCopyProcessIDList.Count == transferProcessIDList.Count)
                    {
                        robocopyProcessFlg = false;
                    }
                    else
                    {
                        robocopyProcessFlg = true;
                    }
                    // RoboCopyのプロセスがない場合、同期化執行
                    if (!robocopyProcessFlg)
                    {
                        // 自動
                        if (controlFlg.Equals("1"))
                        {
                            DateTime dtstart = DateTime.Parse(batchStartTime);
                            DateTime dtend = DateTime.Parse(batchEndTime);
                            //设置的拷贝开始时间晚于程序启动时间时，执行拷贝
                            if (DateTime.Now >= dtstart && DateTime.Now <= dtend && dtstart > formLoadedTime)
                            {
                                if (roBocopyDoFlg)
                                {
                                    EXEDoEndTime = DateTime.Now;
                                    // 完了ログファイル名
                                    string doLogFileName = "執行結果Log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                                    string doLogDir = "執行結果報告";
                                    // 今のプログラムパス
                                    string doLogPath = System.AppDomain.CurrentDomain.BaseDirectory + doLogDir + "\\" + doLogFileName;
                                    CreateBAT batClass = new CreateBAT(doLogPath);
                                    batClass.ResultLog(EXEDoStartTime, EXEDoEndTime);
                                    roBocopyDoFlg = false;
                                }
                            }
                            else
                            {
                                Process[] psDoList = Process.GetProcesses();
                                try
                                {
                                    // RoboCopyのクリア
                                    Process[] psRobocopyList = Process.GetProcessesByName("Robocopy");
                                    foreach (Process item in psRobocopyList)
                                    {
                                        item.Kill();
                                    }
                                    // cmd
                                    Process[] psCmdList = Process.GetProcessesByName("cmd");
                                    foreach (Process item in psCmdList)
                                    {
                                        if (processIDCmdList.Contains(item.Id))
                                        {
                                            item.Kill();
                                            processIDCmdList.Remove(item.Id);
                                        }
                                    }
                                    // conhost
                                    Process[] psConhostList = Process.GetProcessesByName("conhost");
                                    foreach (Process item in psConhostList)
                                    {
                                        if (processIDConhostList.Contains(item.Id))
                                        {
                                            item.Kill();
                                            processIDConhostList.Remove(item.Id);
                                        }
                                    }
                                    //
                                    transferProcessIDList.Clear();
                                    this.IsTransferOnly = false;
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                                }
                            }
                        }
                        else
                        {
                            AllServerDoRoboCopy();
                        }

                        #region log复制
                        //执行完成后，log复制到BK中——2014-7-30 wjd add
                        // ログフォルダーパス
                        string datenowDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd");
                        string datenowDirBK = "RoboCopyLogToCopyBK-" + DateTime.Now.ToString("yyyy-MM-dd");
                        // 今のログフォルダーパス
                        string logDirPath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir;
                        string logDirPathBK = System.AppDomain.CurrentDomain.BaseDirectory + datenowDirBK;
                        // ログ実行
                        if (!Directory.Exists(logDirPath))
                        {
                            Directory.CreateDirectory(logDirPath);
                        }
                        // ログバックアップのフオルダー
                        if (!Directory.Exists(logDirPathBK))
                        {
                            Directory.CreateDirectory(logDirPathBK);
                        }
                        // ログバックアップ
                        DirectoryInfo logDirInfo = new DirectoryInfo(logDirPath);
                        FileInfo[] logFileInfoList = logDirInfo.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                        foreach (FileInfo logFileInfo in logFileInfoList)
                        {
                            try
                            {
                                string logFileBKPath = Path.Combine(logDirPathBK, logFileInfo.FullName.Substring(logFileInfo.FullName.LastIndexOf(@"\") + 1));
                                logFileInfo.CopyTo(logFileBKPath, true);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                                continue;
                            }
                        }
                        //XCopy(logDirPath, logDirPathBK);
                        // ログクリア
                        //DeleteFile(logDirPath);——2014-7-30 wjd commented
                        // BATフォルダーパス
                        string batDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                        string batDirPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir;
                        if (!Directory.Exists(batDirPath))
                        {
                            Directory.CreateDirectory(batDirPath);
                        }
                        // ログクリア
                        //DeleteFile(batDirPath);——2014-7-30 wjd commented
                        #endregion
                    }
                }
                else
                {
                    // 自動
                    if (controlFlg.Equals("1"))
                    {
                        DateTime dtstart = DateTime.Parse(batchStartTime);
                        DateTime dtend = DateTime.Parse(batchEndTime);
                        //设置的拷贝开始时间晚于程序启动时间时，执行拷贝
                        if (DateTime.Now >= dtstart && DateTime.Now <= dtend && dtstart > formLoadedTime)
                        {
                            if (!DayDoingOpenFlg)
                            {
                                EXEDoStartTime = DateTime.Now;
                                AllServerDoRoboCopy();
                                //
                                roBocopyDoFlg = true;
                                //
                                DayDoingOpenFlg = true;
                            }
                            else if (!this.IsTransferOnly)
                            {
                                this.IsTransferOnly = true;
                                AllServerDoRoboCopy();
                            }
                        }
                        else
                        {
                            //
                            DayDoingOpenFlg = false;
                        }
                    }
                }

                #region 删除job端已删除的文件夹
                DirectorClearManager.Clean();
                #endregion
                // 同期のTIMER起動
                timerForRoboCopy.Enabled = true;
                #endregion

            }
            catch (Exception exb)
            {
                logger.Error(MessageUtil.GetExceptionMsg(exb, ""));
                timerForRoboCopy.Enabled = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void AllServerDoRoboCopy()
        {
            // 今回執行
            List<Entities.monitorServer> monitorServerList = monitorServerManager.GetMonitorServerList();
            int monitorNum = monitorServerList.Count();
            if (monitorNum > 0)
            {
                for (int i = 0; i < monitorNum; i++)
                {
                    if (monitorServerList[i].monitorSystem == 1)
                    {
                        string monitorSrcPath = @"\\" + monitorServerList[i].monitorServerIP + @"\" + monitorServerList[i].startFile.TrimStart('\\');
                        string monitorTargetPath = monitorServerList[i].monitorLocalPath.TrimEnd('\\') + "\\";
                        // 削除のフオルダーリスト
                        List<string> topDirDelList = new List<string>();

                        try
                        {
                            // 削除処理
                            //DateTime dtstart = DateTime.Parse(batchStartTime);
                            //DateTime dtend = DateTime.Parse(batchEndTime);
                            //if (DateTime.Now >= dtstart && DateTime.Now <= dtend)
                            //{
                            //    TopDirDelete(monitorTargetPath);
                            //}
                            //
                            if (monitorServerTopDirList.ContainsKey(monitorSrcPath))
                            {
                                // 最新のリスト
                                DirectoryInfo newDirectoryInfo = new DirectoryInfo(monitorSrcPath);
                                List<string> newDirList = getDirectoryFullInfo(newDirectoryInfo);
                                // 元のリスト
                                List<string> oldDirList = (List<string>)monitorServerTopDirList[monitorSrcPath];
                                // 新規
                                //var queryListCreate = from newtopdir in newDirList
                                //                      where !(
                                //                          from newtopdirectory in newDirList
                                //                          from oldtopdirectory in oldDirList
                                //                          where newtopdirectory == oldtopdirectory
                                //                          select newtopdirectory).Contains(newtopdir)
                                //                      select newtopdir;
                                List<string> queryListCreate = newDirList.Except(oldDirList).ToList();
                                if (queryListCreate.Count() > 0)
                                {
                                    // 検索結果
                                    List<monitorServerFolder> resultList = monitorServerFolderManager.GetByMonitorTopFolder(monitorServerList[i].id, String.Empty, monitorSrcPath, "99", "1", 0);
                                    bool topFolderFlg = false;
                                    if (resultList.Count > 0)
                                    {
                                        topFolderFlg = true;
                                    }
                                    //
                                    foreach (string addInfo in queryListCreate)
                                    {
                                        // 追加
                                        MonitorServerFolderAdd(monitorServerList[i].id, addInfo, topFolderFlg);
                                    }
                                }
                                // 削除
                                //var queryListDel = from oldtopdir in oldDirList
                                //                   where !(
                                //                       from oldtopdirectory in oldDirList
                                //                       from newtopdirectory in newDirList
                                //                       where oldtopdirectory == newtopdirectory
                                //                       select oldtopdirectory).Contains(oldtopdir)
                                //                   select oldtopdir;
                                List<string> queryListDel = oldDirList.Except(newDirList).ToList();
                                if (queryListDel.Count() > 0)
                                {
                                    foreach (string delInfo in queryListDel)
                                    {
                                        topDirDelList.Add(delInfo);
                                        // コピー先
                                        string delDirPath = PathFileConvert(monitorSrcPath, monitorTargetPath, delInfo);
                                        //DirDel(delDirPath);
                                        // 削除の機能
                                        TopDirDirectDelete(delDirPath);
                                        // RaNameの機能
                                        //DirRename(delDirPath);
                                    }
                                }
                                // リスト更新
                                if (queryListCreate.Count() > 0 || queryListDel.Count() > 0)
                                {
                                    monitorServerTopDirList.Remove(monitorSrcPath);
                                    monitorServerTopDirList.Add(monitorSrcPath, newDirList);
                                }
                            }
                            // RoboCopy
                            RoboCopyStart(monitorServerList[i].id, monitorServerList[i].monitorServerIP, monitorServerList[i].account, monitorServerList[i].password, monitorSrcPath, monitorTargetPath, monitorServerList[i].copyInit, topDirDelList);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message, ex);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// フォルダー設定のDB更新
        /// </summary>
        /// <param name="monitorID"></param>
        /// <param name="topDirPath"></param>
        /// <param name="topFolderFlg"></param>
        private void MonitorServerFolderAdd(int monitorID, string topDirPath, bool topFolderFlg)
        {
            monitorServerFolder monitorServerFolderInfo = new monitorServerFolder();
            DirectoryInfo topDirectoryInfo = new DirectoryInfo(topDirPath);
            try
            {
                monitorServerFolderInfo.monitorServerID = monitorID;
                monitorServerFolderInfo.monitorFileName = topDirectoryInfo.Name;
                monitorServerFolderInfo.monitorFilePath = topDirectoryInfo.FullName.Substring(0, topDirectoryInfo.FullName.LastIndexOf("\\"));
                monitorServerFolderInfo.monitorFileType = "99";
                if (topFolderFlg)
                {
                    monitorServerFolderInfo.initFlg = 0;
                }
                else
                {
                    monitorServerFolderInfo.initFlg = 1;
                }
                monitorServerFolderInfo.monitorFlg = 0;
                monitorServerFolderInfo.monitorStatus = "新規";
                monitorServerFolderInfo.deleteFlg = 0;
                monitorServerFolderInfo.deleter = DEFAULTCHAR_VALUE;
                monitorServerFolderInfo.deleteDate = DEFAULTDATETIME_VALUE;
                monitorServerFolderInfo.creater = "exe";
                monitorServerFolderInfo.createDate = DateTime.Now;
                monitorServerFolderInfo.updater = "exe";
                monitorServerFolderInfo.updateDate = DateTime.Now;
                monitorServerFolderInfo.restorer = "";
                monitorServerFolderInfo.restoreDate = DEFAULTDATETIME_VALUE;
                monitorServerFolderManager.Insert(monitorServerFolderInfo);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// バックアップステータス
        /// </summary>
        /// <param name="m"></param>
        private void ListenStatus(string id, string text, Color status)
        {
            foreach (Control ctrl in tablePanel.Controls)
            {
                if (ctrl.Name == ("status" + id))
                {
                    Label lbl = ctrl as Label;
                    lbl.Text = text;
                    lbl.BackColor = status;
                }
            }
        }
        /// <summary>
        /// バックアップ开始Button状态
        /// </summary>
        /// <param name="m"></param>
        private void ListenButton(string name, bool enable)
        {
            foreach (Control ctrl in tablePanel.Controls)
            {
                if (ctrl.Name == name)
                {
                    Button lbl = ctrl as Button;
                    lbl.Enabled = enable;
                }
            }
        }

        /// <summary>
        /// バックアップ起動
        /// 创建拷贝连接
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_ipaddress"></param>
        /// <param name="_loginuser"></param>
        /// <param name="_loginpassword"></param>
        /// <param name="_monitorSrcPath"></param>
        /// <param name="_monitorTargetPath"></param>
        /// <param name="_topDirCopyFlg"></param>
        /// <param name="_topDirDelList"></param>
        public void RoboCopyStart(int _id, string _ipaddress, string _loginuser, string _loginpassword, string _monitorSrcPath, string _monitorTargetPath, int _topDirCopyFlg, List<string> _topDirDelList)
        {
            try
            {
                if (!shareFolderConnetList.Contains(_ipaddress))
                {
                    //接続テスト
                    bool result = true;
                    int connectTime = 0;
                    NetWorkFileShare netWorkFileShare = new NetWorkFileShare();
                    while (result)
                    {
                        if (netWorkFileShare.ConnectState(_monitorSrcPath, _loginuser, _loginpassword))
                        {
                            result = false;
                            connectTime = 0;
                            try
                            {
                                // コピー先
                                if (!Directory.Exists(_monitorTargetPath))
                                {
                                    Directory.CreateDirectory(_monitorTargetPath);
                                }
                                // コピー
                                // パラメータ
                                FileCopyListen(_id, _monitorSrcPath, _monitorTargetPath, _topDirCopyFlg, _topDirDelList);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                            }
                        }
                        else
                        {
                            if (connectTime > 20)
                            {
                                logger.Info("[" + _ipaddress + "]は接続できません");
                                result = false;
                            }
                            connectTime++;
                        }
                    }
                }
                else
                {
                    try
                    {
                        // コピー先
                        if (!Directory.Exists(_monitorTargetPath))
                        {
                            Directory.CreateDirectory(_monitorTargetPath);
                        }
                        // コピー
                        // パラメータ
                        FileCopyListen(_id, _monitorSrcPath, _monitorTargetPath, _topDirCopyFlg, _topDirDelList);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// ファイルの比較
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="startSrcPath"></param>
        /// <param name="startBakPath"></param>
        /// <param name="topDirCopyFlg"></param>
        /// <param name="topDirDelList"></param>
        private void FileCopyListen(int _id, string startSrcPath, string startBakPath, int topDirCopyFlg, List<string> topDirDelList)
        {
            MonitorServerFolderManager monitorServerFolderManager = new MonitorServerFolderManager();
            MonitorFileListenManager monitorFileListenManager = new MonitorFileListenManager();
            FileTypeSetManager fileTypeSetManager = new FileTypeSetManager();
            NetWorkFileShare netWorkFileShare = new NetWorkFileShare();
            // 転送情報の抽出
            try
            {
                // 監視対象の抽出
                List<monitorServerFolder> monitorPathlist = monitorServerFolderManager.GetByAllMonitorObject(_id, 1, 0);

                // Xcopy
                // XCopy(startSrcPath.Substring(0, startSrcPath.LastIndexOf("\\")).TrimEnd('\\'), startBakPath);
                if (topDirCopyFlg == 1 && !this.IsTransferOnly)
                {
                    DeleteFile(startBakPath);
                    XCopy(startSrcPath.TrimEnd('\\'), startBakPath);
                }
                //
                if (monitorPathlist.Count > 0)
                {
                    // linq
                    var dirList = monitorPathlist.ToList().Distinct(new DirectoryNameComparer());
                    // 親パスが同じの場合
                    if (dirList.Count() > 0)
                    {
                        //
                        bool topDirListen = false;
                        bool subDirAllFlg = false;
                        foreach (var dirinfo in dirList)
                        {
                            if (startSrcPath.TrimEnd('\\').Equals(dirinfo.monitorFilePath.TrimEnd('\\')))
                            {
                                topDirListen = true;
                                break;
                            }
                        }
                        //发送邮件——2014-8-6 wjd add
                        SendMail sendMail = new SendMail();
                        string context = "";
                        if (topDirListen && !this.IsTransferOnly)
                        {
                            // フォルダー情報取得
                            DirectoryInfo directoryInfo = new DirectoryInfo(startSrcPath.TrimEnd('\\'));
                            // 監視対象のフォルダー情報取得
                            var subDirList = (from subdir in monitorPathlist
                                              where subdir.monitorFilePath.Equals(directoryInfo.FullName) && subdir.monitorFileType.Equals("99") && subdir.initFlg.ToString().Equals("1")
                                              select subdir.monitorFilePath + "\\" + subdir.monitorFileName).Distinct();
                            List<string> subTopDirInfoList = new List<string>();
                            if (subDirList.Count() == 1 && subDirList.Single().TrimEnd('\\').Equals(directoryInfo.FullName))
                            {
                                // 削除のフォルダーの判断
                                if (topDirDelList.Count > 0)
                                {
                                    subTopDirInfoList = getDirectoryFullInfo(directoryInfo).Except(topDirDelList).ToList();
                                }
                                else
                                {
                                    subTopDirInfoList = getDirectoryFullInfo(directoryInfo);
                                }
                                subDirAllFlg = true;
                            }
                            else
                            {
                                List<string> subTopDirForDBList = new List<string>();
                                // DB抽出
                                foreach (string topDirInfo in subDirList)
                                {
                                    subTopDirForDBList.Add(topDirInfo);
                                }
                                // 削除のフォルダーの判断
                                if (topDirDelList.Count > 0)
                                {
                                    subTopDirInfoList = subTopDirForDBList.Except(topDirDelList).ToList();
                                }
                                else
                                {
                                    subTopDirInfoList = subTopDirForDBList;
                                }
                            }
                            // ファイル除外リスト
                            List<string> fileExtensionList = new List<string>();
                            FileSetInfo fileSetInfo = fileTypeSetManager.CheckFileTypeSet(directoryInfo.FullName, _id, 0);
                            if (fileSetInfo.DirectoryPathList.Count() > 0)
                            {
                                if (fileSetInfo.DirectoryPathList.Contains(directoryInfo.FullName))
                                {
                                    // ファイル除外リスト
                                    fileExtensionList = fileSetInfo.DirectoryFileExtensionList[directoryInfo.FullName] as List<string>;
                                }
                            }
                            foreach (string subTopDirInfo in subTopDirInfoList)
                            {
                                // サブファイルの設定
                                List<string> subFileExtensionList = new List<string>();
                                FileSetInfo subFileSetInfo = fileTypeSetManager.CheckFileTypeSet(subTopDirInfo, _id, 0);
                                if (subFileSetInfo.DirectoryPathList.Count() > 0)
                                {
                                    if (subFileSetInfo.DirectoryPathList.Contains(subTopDirInfo))
                                    {
                                        // ファイル除外リスト
                                        subFileExtensionList = subFileSetInfo.DirectoryFileExtensionList[directoryInfo.FullName] as List<string>;
                                    }
                                }
                                if (subFileExtensionList != null && subFileExtensionList.Count() > 0)
                                {
                                    foreach (string subFileExtensionInfo in subFileExtensionList)
                                    {
                                        if (!fileExtensionList.Contains(subFileExtensionInfo))
                                        {
                                            fileExtensionList.Add(subFileExtensionInfo);
                                        }
                                    }
                                }
                                // コピー先
                                string subCopyTargetPath = PathFileConvert(startSrcPath, startBakPath, subTopDirInfo);

                                // ローカルコピー
                                int run = 0;
                                while (run < 3)
                                {
                                    bool isOK = CopyListenExclusionFile(subTopDirInfo, subCopyTargetPath, fileExtensionList, copyListenTime, MTNum, RNum, WNum);
                                    run++;

                                    if (isOK)
                                    {
                                        break;
                                    }
                                    else if (run == 3)
                                    {
                                        //Send Email
                                        context += "\r\nコピーを行う方法：CopyListenExclusionFile\r\n"
                                            + "\r\n源パス：" + subTopDirInfo
                                            + "\r\nターゲットパス：" + subCopyTargetPath;
                                    }
                                }
                            }
                            // 
                            Thread.Sleep(2000);
                        }
                        // サブフォルダーの設定だけの場合
                        if (!subDirAllFlg && !this.IsTransferOnly)
                        {
                            foreach (var dirinfo in dirList)
                            {
                                if (!startSrcPath.TrimEnd('\\').Equals(dirinfo.monitorFilePath.TrimEnd('\\')))
                                {
                                    // フォルダー情報取得
                                    DirectoryInfo directoryInfo = new DirectoryInfo(dirinfo.monitorFilePath);
                                    // コピー先
                                    string copyTargetPath = PathFileConvert(startSrcPath, startBakPath, directoryInfo.FullName);
                                    //
                                    List<string> exceptionDirInfoList;
                                    // 監視対象のフォルダー情報取得
                                    var subDirList = (from subdir in monitorPathlist
                                                      where subdir.monitorFilePath.Equals(dirinfo.monitorFilePath) && subdir.monitorFileType.Equals("99")
                                                      select subdir.monitorFileName).Distinct();
                                    List<string> listenDirList = new List<string>();
                                    if (subDirList.Count() > 0)
                                    {
                                        foreach (var listenDirInfo in subDirList)
                                        {
                                            listenDirList.Add(listenDirInfo);
                                        }
                                    }
                                    //
                                    List<string> subDirInfoList = getDirectoryInfo(directoryInfo);
                                    //
                                    exceptionDirInfoList = subDirInfoList.Except(listenDirList).ToList();
                                    // ファイル除外リスト
                                    List<string> fileExtensionList = new List<string>();
                                    FileSetInfo fileSetInfo = fileTypeSetManager.CheckFileTypeSet(directoryInfo.FullName, _id, 0);
                                    if (fileSetInfo.DirectoryPathList.Count() > 0)
                                    {
                                        if (fileSetInfo.DirectoryPathList.Contains(directoryInfo.FullName))
                                        {
                                            // ファイル除外リスト
                                            fileExtensionList = fileSetInfo.DirectoryFileExtensionList[directoryInfo.FullName] as List<string>;
                                        }
                                    }
                                    // ローカルコピー
                                    int run = 0;
                                    while (run < 3)
                                    {
                                        bool isOK = CopyListenExclusionFolderFile(directoryInfo.FullName.TrimEnd('\\'), copyTargetPath.TrimEnd('\\'), exceptionDirInfoList, fileExtensionList, copyListenTime, MTNum, RNum, WNum);
                                        run++;

                                        if (isOK)
                                        {
                                            break;
                                        }
                                        else if (run == 3)
                                        {
                                            //Send Email
                                            context += "\r\nコピーを行う方法：CopyListenExclusionFolderFile\r\n"
                                                + "\r\n源パス：" + directoryInfo.FullName
                                                + "\r\nターゲットパス：" + copyTargetPath;
                                        }
                                    }
                                }
                            }
                        }
                        // 転送コピー——执行条件：手动或已拷贝完成(未完成)
                        if (controlFlg == "0" || this.IsTransferOnly)
                        {
                            BackupServerGroupManager backupServerGroupManager = new BackupServerGroupManager();
                            backupServerGroup backupServerGroup = backupServerGroupManager.GetBackupServerGroup(_id);

                            BackupServerGroupDetailManager backupServerGroupDetailManager = new BackupServerGroupDetailManager();
                            List<backupServerGroupDetail> backupServerGroupDetailList = backupServerGroupDetailManager.GetBackupServerGroupDetailList(backupServerGroup.id);

                            BackupServerManager backupServerManager = new BackupServerManager();

                            for (int j = 0; j < backupServerGroupDetailList.Count; j++)
                            {
                                backupServer backupServer = backupServerManager.GetBackupServerInfo(backupServerGroupDetailList[j].backupServerID);
                                if (backupServer == null) continue;
                                string TransferCopyPath = @"\\" + backupServer.backupServerIP + @"\" + backupServer.startFile.TrimStart('\\');
                                // 転送先
                                //string TransferTargetPath = PathFileConvert(startBakPath, TransferCopyPath, subCopyTargetPath);
                                if (!transferProcessIDList.ContainsKey(TransferCopyPath))
                                {
                                    int run = 0;
                                    while (run < 3)
                                    {
                                        bool isOK = CopyListen(startBakPath.TrimEnd('\\'), TransferCopyPath.TrimEnd('\\'), transferTime, MTNum, RNum, WNum);
                                        run++;

                                        if (isOK)
                                        {
                                            break;
                                        }
                                        else if (run == 3)
                                        {
                                            //Send Email
                                            context += "\r\nコピーを行う方法：CopyListen\r\n"
                                                + "\r\n源パス：" + startBakPath
                                                + "\r\nターゲットパス：" + TransferCopyPath;
                                        }
                                    }
                                }
                            }
                        }

                        //有内容则发送邮件
                        if (context.Length > 0)
                        {
                            sendMail.SendCopyReport(context);

                            //启动再执行拷贝方法
                            StartReCopyMethod(_id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(MessageUtil.GetExceptionMsg(e, ""));
            }
        }

        #region 再执行拷贝

        /// <summary>
        /// 启动再执行拷贝方法
        /// </summary>
        /// <param name="_id"></param>
        private void StartReCopyMethod(int _id)
        {
            if (!string.IsNullOrWhiteSpace(logConfirmTime))
            {
                if (!reCopyServerIDList.Contains(_id))
                {
                    reCopyServerIDList.Add(_id);

                    if (reCopyTimer == null)
                    {
                        reCopyTimer = new System.Timers.Timer();
                        reCopyTimer.Interval = 60000;
                        reCopyTimer.Elapsed += ReCopyTimer_Elapsed;
                        reCopyTimer.Start();
                    }
                }
                else
                {
                    reCopyServerIDList.Remove(_id);
                }
            }
        }

        System.Timers.Timer reCopyTimer = null;

        private void ReCopyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            reCopyTimer.Enabled = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(logConfirmTime))
                {
                    if (DateTime.Parse(logConfirmTime) <= DateTime.Now)
                    {
                        var array = new int[] { };
                        reCopyServerIDList.CopyTo(array);
                        foreach (int id in array)
                        {
                            ListenStart(id);
                            reCopyServerIDList.Remove(id);
                        }

                        //执行完把自己关闭
                        if (reCopyServerIDList.Count == 0)
                        {
                            reCopyTimer.Stop();
                            reCopyTimer.Close();
                            reCopyTimer = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            reCopyTimer.Enabled = true;
        }

        #endregion

        /// <summary>
        /// 削除のファイル名のリネーム
        /// </summary>
        /// <param name="dirpath"></param>
        void TopDirDelete(string dirpath)
        {
            DirectoryInfo dir = new DirectoryInfo(@dirpath);
            DirectorySecurity dirsec = dir.GetAccessControl();
            // 管理者の権限
            NTAccount admin = new NTAccount("administrators");
            dirsec.SetOwner(admin);
            dirsec.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            dir.SetAccessControl(dirsec);
            try
            {
                DirectoryInfo[] deldirlist = dir.GetDirectories(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") + "_*", SearchOption.TopDirectoryOnly);
                if (deldirlist.Count() > 0)
                {
                    foreach (DirectoryInfo deldirinfo in deldirlist)
                    {
                        deldirinfo.Delete(true);
                    }
                }
                //string delDir = dir.FullName.Substring(0, dir.FullName.LastIndexOf("\\")) + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + dir.Name;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 削除のファイル名のリネーム
        /// </summary>
        /// <param name="dirpath"></param>
        void TopDirDirectDelete(string dirpath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(@dirpath);
                //文件夹不存在，退出方法
                if (!dir.Exists)
                {
                    return;
                }

                DirectorySecurity dirsec = dir.GetAccessControl();
                // 管理者の権限
                NTAccount admin = new NTAccount("administrators");
                dirsec.SetOwner(admin);
                dirsec.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                dir.SetAccessControl(dirsec);

                dir.Delete(true);
                if (DelDirForErrorDelList.Contains(dirpath))
                {
                    DelDirForErrorDelList.Remove(dirpath);
                }
            }
            catch (Exception ex)
            {
                if (!DelDirForErrorDelList.Contains(dirpath))
                {
                    logger.Error(ex.Message);
                    DelDirForErrorDelList.Add(dirpath);
                }
            }
        }

        /// <summary>
        /// 削除のファイル名のリネーム
        /// </summary>
        /// <param name="dirpath"></param>
        void DirRename(string dirpath)
        {
            DirectoryInfo dir = new DirectoryInfo(@dirpath);
            DirectorySecurity dirsec = dir.GetAccessControl();
            // 管理者の権限
            NTAccount admin = new NTAccount("administrators");
            dirsec.SetOwner(admin);
            dirsec.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            dir.SetAccessControl(dirsec);
            try
            {
                string delDir = dir.FullName.Substring(0, dir.FullName.LastIndexOf("\\")) + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + dir.Name;
                dir.MoveTo(delDir);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーの強制削除
        /// </summary>
        /// <param name="dirpath"></param>
        void DirDel(string dirpath)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // BAT作成
                string batFileName = "RMDIR-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.ForceDeleteDirWrite(dirpath);
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                defaultUserDeskTop.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        void XCopy(string srcpath, string exportpath)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // コマンド処理
                dircommand.Append("xcopy /h ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\"");
                // BAT作成
                string batFileName = "XCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //
                Process[] processConhostBeforeList = Process.GetProcessesByName("conhost");
                List<int> processBeforeList = new List<int>();
                foreach (Process item in processConhostBeforeList)
                {
                    processBeforeList.Add(item.Id);
                }
                defaultUserDeskTop.Start();
                Thread.Sleep(3000);
                Process[] processConhostAfterList = Process.GetProcessesByName("conhost");
                List<int> processAfterList = new List<int>();
                foreach (Process item in processConhostAfterList)
                {
                    processAfterList.Add(item.Id);
                }
                List<int> processExceptList = processAfterList.Except(processBeforeList).ToList();
                if (processExceptList.Count > 0)
                {
                    foreach (int processIDInfo in processExceptList)
                    {
                        if (!processIDConhostList.Contains(processIDInfo))
                        {
                            processIDConhostList.Add(processIDInfo);
                        }
                    }
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
                // 転送のプロセスIDの保存(CMD)
                if (!processIDCmdList.Contains(defaultUserDeskTop.Id))
                {
                    processIDCmdList.Add(defaultUserDeskTop.Id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        bool CopyListen(string srcpath, string exportpath, string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Common.RandomCode.GetCode(16) + ".log";
                string datenowDir = "RoboCopyLogToTransfer-" + DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                // コマンド処理
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                string batFileName = "ROBOCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "RoboCopyLogToTransfer-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                // プロセス設定
                //defaultUserDeskTop.StartInfo.UserName = "BudAdmin";
                //System.Security.SecureString password = new System.Security.SecureString();
                //char[] pass = "BudBk@96".ToCharArray();
                //foreach (char c in pass)
                //{
                //    password.AppendChar(c);
                //}
                //defaultUserDeskTop.StartInfo.Password = password;
                //
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                //defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //
                Process[] processConhostBeforeList = Process.GetProcessesByName("conhost");
                List<int> processBeforeList = new List<int>();
                foreach (Process item in processConhostBeforeList)
                {
                    processBeforeList.Add(item.Id);
                }
                defaultUserDeskTop.Start();
                Thread.Sleep(3000);
                Process[] processConhostAfterList = Process.GetProcessesByName("conhost");
                List<int> processAfterList = new List<int>();
                foreach (Process item in processConhostAfterList)
                {
                    processAfterList.Add(item.Id);
                }
                List<int> processExceptList = processAfterList.Except(processBeforeList).ToList();
                if (processExceptList.Count > 0)
                {
                    foreach (int processIDInfo in processExceptList)
                    {
                        if (!processIDConhostList.Contains(processIDInfo))
                        {
                            processIDConhostList.Add(processIDInfo);
                        }
                    }
                }
                // 転送のプロセスIDの保存(CMD)
                if (!processIDCmdList.Contains(defaultUserDeskTop.Id))
                {
                    processIDCmdList.Add(defaultUserDeskTop.Id);
                }
                // 転送のプロセスIDの保存
                if (!transferProcessIDList.ContainsKey(exportpath))
                {
                    transferProcessIDList.Add(exportpath, defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();

                //记录日志——2014-8-6 wjd test
                if (!File.Exists(logpath) || !File.Exists(batPath))
                {
                    string info = "\r\n\r\nCopyListen：Not Run.\r\n fileName：" + fileName + "\r\n batFileName：" + batFileName;
                    info += "\r\n srcpath：" + srcpath + "；exportpath：" + exportpath
                        + "\r\n listentime：" + listentime + "；threadnum：" + threadnum + "；retrynum：" + retrynum + "；retrytime：" + retrytime;
                    info += "\r\n\t defaultUserDeskTop.Id：" + defaultUserDeskTop.Id + "\r\n\r\n";
                    logger.Info(info);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="exclusionFolderList"></param>
        /// <param name="exclusionFileList"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        bool CopyListenExclusionFolderFile(string srcpath, string exportpath, List<string> exclusionFolderList, List<string> exclusionFileList,
            string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Common.RandomCode.GetCode(16) + ".log";
                string datenowDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                //
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                //dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                // フォルダー除外
                if (exclusionFolderList.Count > 0)
                {
                    string folderExclusionArray = "";
                    foreach (string folderpath in exclusionFolderList)
                    {
                        folderExclusionArray = folderExclusionArray + "\"" + folderpath + "\"" + " ";
                    }
                    dircommand.Append(" /XD " + folderExclusionArray);
                }
                // ファイル除外
                if (exclusionFileList.Count > 0)
                {
                    string fileExclusionArray = "";
                    foreach (string folderpath in exclusionFileList)
                    {
                        fileExclusionArray = fileExclusionArray + "\"" + "*" + folderpath + "\"" + " ";
                    }
                    dircommand.Append(" /XF " + fileExclusionArray);
                }
                //
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                string batFileName = "ROBOCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                // プロセス設定
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //
                Process[] processConhostBeforeList = Process.GetProcessesByName("conhost");
                List<int> processBeforeList = new List<int>();
                foreach (Process item in processConhostBeforeList)
                {
                    processBeforeList.Add(item.Id);
                }
                defaultUserDeskTop.Start();
                Thread.Sleep(3000);
                Process[] processConhostAfterList = Process.GetProcessesByName("conhost");
                List<int> processAfterList = new List<int>();
                foreach (Process item in processConhostAfterList)
                {
                    processAfterList.Add(item.Id);
                }
                List<int> processExceptList = processAfterList.Except(processBeforeList).ToList();
                if (processExceptList.Count > 0)
                {
                    foreach (int processIDInfo in processExceptList)
                    {
                        if (!processIDConhostList.Contains(processIDInfo))
                        {
                            processIDConhostList.Add(processIDInfo);
                        }
                    }
                }
                // プロセスIDの保存(CMD)
                if (!processIDCmdList.Contains(defaultUserDeskTop.Id))
                {
                    processIDCmdList.Add(defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();

                //记录日志——2014-8-6 wjd test
                if (!File.Exists(logpath) || !File.Exists(batPath))
                {
                    string info = "\r\n\r\nCopyListenExclusionFolderFile：Not Run.\r\n fileName：" + fileName + "\r\n batFileName：" + batFileName;
                    info += "\r\n srcpath：" + srcpath + "；exportpath：" + exportpath + "；exclusionFileList：" + String.Concat(exclusionFileList)
                        + "\r\n listentime：" + listentime + "；threadnum：" + threadnum + "；retrynum：" + retrynum + "；retrytime：" + retrytime;
                    info += "\r\n\t defaultUserDeskTop.Id：" + defaultUserDeskTop.Id + "\r\n\r\n";
                    logger.Info(info);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="exclusionFileList"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        bool CopyListenExclusionFile(string srcpath, string exportpath, List<string> exclusionFileList,
            string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Common.RandomCode.GetCode(16) + ".log";
                string datenowDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                //dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                // ファイル除外
                if (exclusionFileList.Count > 0)
                {
                    string fileExclusionArray = "";
                    foreach (string folderpath in exclusionFileList)
                    {
                        fileExclusionArray = fileExclusionArray + "\"" + "*" + folderpath + "\"" + " ";
                    }
                    dircommand.Append(" /XF " + fileExclusionArray);
                }
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                string batFileName = "ROBOCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                // プロセス設定
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //
                Process[] processConhostBeforeList = Process.GetProcessesByName("conhost");
                List<int> processBeforeList = new List<int>();
                foreach (Process item in processConhostBeforeList)
                {
                    processBeforeList.Add(item.Id);
                }
                defaultUserDeskTop.Start();
                Thread.Sleep(3000);
                Process[] processConhostAfterList = Process.GetProcessesByName("conhost");
                List<int> processAfterList = new List<int>();
                foreach (Process item in processConhostAfterList)
                {
                    processAfterList.Add(item.Id);
                }
                List<int> processExceptList = processAfterList.Except(processBeforeList).ToList();
                if (processExceptList.Count > 0)
                {
                    foreach (int processIDInfo in processExceptList)
                    {
                        if (!processIDConhostList.Contains(processIDInfo))
                        {
                            processIDConhostList.Add(processIDInfo);
                        }
                    }
                }
                // プロセスIDの保存(CMD)
                if (!processIDCmdList.Contains(defaultUserDeskTop.Id))
                {
                    processIDCmdList.Add(defaultUserDeskTop.Id);
                }
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();

                //记录日志——2014-8-6 wjd test
                if (!File.Exists(logpath) || !File.Exists(batPath))
                {
                    string info = "\r\n\r\nCopyListenExclusionFile：Not Run.\r\n fileName：" + fileName + "\r\n batFileName：" + batFileName;
                    info += "\r\n srcpath：" + srcpath + "；exportpath：" + exportpath + "；exclusionFileList：" + String.Concat(exclusionFileList)
                        + "\r\n listentime：" + listentime + "；threadnum：" + threadnum + "；retrynum：" + retrynum + "；retrytime：" + retrytime;
                    info += "\r\n\t defaultUserDeskTop.Id：" + defaultUserDeskTop.Id + "\r\n\r\n";
                    logger.Info(info);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="srcpath"></param>
        /// <param name="targetpath"></param>
        /// <param name="objectpath"></param>
        private string PathFileConvert(string srcpath, string targetpath, string objectpath)
        {
            string returnFilePath = "";
            try
            {
                string[] mainpath = srcpath.Split('\\');
                string[] subpath = objectpath.Split('\\');
                string dirpath = "";
                bool indexflg = false;
                int mainIndex = mainpath.Length - 1;
                // 一番後ろのパス取得
                for (int j = mainpath.Length - 1; j >= 0; j--)
                {
                    if (!String.IsNullOrEmpty(mainpath[j]))
                    {
                        mainIndex = j;
                        break;
                    }
                }
                // パスの繋がり
                for (int i = 0; i < subpath.Length; i++)
                {
                    if (subpath[i].Equals(mainpath[mainIndex]))
                    {
                        indexflg = true;
                        continue;
                    }

                    if (indexflg)
                    {
                        if (i < subpath.Length - 1)
                        {
                            dirpath += subpath[i] + "\\";
                        }
                    }
                }
                if (srcpath.Equals(objectpath))
                {
                    returnFilePath = Path.Combine(targetpath, dirpath);
                }
                else
                {
                    returnFilePath = Path.Combine(Path.Combine(targetpath, dirpath), subpath[subpath.Length - 1]);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="objectpath"></param>
        private string RelativePathFileConvert(string objectpath)
        {
            string returnFilePath = "";
            try
            {
                string[] subpath = objectpath.Split('\\');
                if (subpath.Count() > 0)
                {
                    returnFilePath = subpath[subpath.Length - 1];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return returnFilePath;
        }


        /// <summary> 
        /// Logファイル
        /// </summary> 
        /// <param   name= "FilePath "></param> 
        public void LogFile(string FilePath)
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            try
            {
                if (fileInfo.Exists)
                {
                    File.Delete(FilePath);
                }
                else
                {
                    if (!Directory.Exists(fileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary> 
        /// フォルダー情報
        /// </summary> 
        List<string> getDirectoryInfo(DirectoryInfo sb)
        {
            List<string> dirInfoList = new List<string>();
            try
            {
                if (sb is DirectoryInfo)
                {
                    FileSystemInfo[] fsis = sb.GetFileSystemInfos();
                    if (fsis.Count() > 0)
                    {
                        foreach (FileSystemInfo fsi in fsis)
                        {
                            if (fsi is DirectoryInfo)
                            {
                                dirInfoList.Add(fsi.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return dirInfoList;
        }

        /// <summary> 
        /// フォルダー情報
        /// </summary> 
        List<string> getDirectoryFullInfo(DirectoryInfo sb)
        {
            List<string> dirInfoList = new List<string>();
            try
            {
                if (sb is DirectoryInfo)
                {
                    FileSystemInfo[] fsis = sb.GetFileSystemInfos();
                    if (fsis.Count() > 0)
                    {
                        foreach (FileSystemInfo fsi in fsis)
                        {
                            if (fsi is DirectoryInfo)
                            {
                                dirInfoList.Add(fsi.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return dirInfoList;
        }

        /// <summary> 
        /// ファイル削除
        /// </summary> 
        /// <param   name= "DirPath "> </param> 
        public void DeleteFile(string DirPath)
        {
            try
            {
                string pattern = "*.*";
                string[] strFileName = Directory.GetFiles(DirPath, pattern);
                foreach (var item in strFileName)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
