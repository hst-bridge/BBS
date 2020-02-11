using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudBackupCopy.Models;
using BudBackupCopy.Entities;
using log4net;
using System.Reflection;
using BudBackupCopy.Common;
using System.Collections;
using Common;
using System.Diagnostics;
using System.IO;

namespace BudBackupCopy
{
    public partial class BudBackupCopy : Form
    {
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// monitorServerManager
        /// </summary>
        MonitorServerManager monitorServerManager = new MonitorServerManager();
        /// <summary>
        /// 監視対象のリスト
        /// </summary>
        List<monitorServer> monitorServerList = new List<monitorServer>();
        /// <summary>
        /// monitorServerHashList
        /// </summary>
        private Hashtable monitorServerHashList = new Hashtable();
        /// <summary>
        /// 接続テストフラグ
        /// </summary>
        private bool connectFlg;
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string MTNum = System.Configuration.ConfigurationManager.AppSettings["MTNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string RNum = System.Configuration.ConfigurationManager.AppSettings["RNum"];
        /// <summary>
        /// コピースレッド指定
        /// </summary>
        private string WNum = System.Configuration.ConfigurationManager.AppSettings["WNum"];
        /// <summary>
        /// CopyListenTime
        /// </summary>
        private string copyListenTime = System.Configuration.ConfigurationManager.AppSettings["CopyListenTime"].ToString();
        /// <summary>
        /// 監視対象のリスト
        /// </summary>
        List<int> processIDList = new List<int>();
        /// <summary>
        /// 実行開始時間
        /// </summary>
        private DateTime EXEDoStartTime;
        /// <summary>
        /// 実行完了時間
        /// </summary>
        private DateTime EXEDoEndTime;
        /// <summary>
        /// 配信のメールリスト
        /// </summary>
        private string ToMailList = System.Configuration.ConfigurationManager.AppSettings["ToMail"].ToString();

        public BudBackupCopy()
        {
            InitializeComponent();
        }

        private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void BudBackupCopy_Load(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = 10000;
                timer1.Elapsed += new System.Timers.ElapsedEventHandler(CheckStart);
                //
                monitorServerList = monitorServerManager.GetMonitorServerList();
                // 対象Binding
                List<ComboBoxItem> cbiList = new List<ComboBoxItem>();
                foreach (monitorServer ms in monitorServerList)
                {
                    cbiList.Add(new ComboBoxItem(ms.id.ToString(), ms.monitorServerName));
                    monitorServerHashList.Add(ms.id.ToString(), ms);
                }
                this.comboBoxObject.DisplayMember = "Text";
                this.comboBoxObject.ValueMember = "Value";
                this.comboBoxObject.DataSource = cbiList;
                // ディフォルトの場合
                monitorServer defaultMonitorServer = (monitorServer)monitorServerHashList[this.comboBoxObject.SelectedValue];
                textBoxJobPath.Text = @"\\" + defaultMonitorServer.monitorServerIP + @"\" + defaultMonitorServer.startFile.TrimStart('\\');
                textBoxLocalPath.Text = defaultMonitorServer.monitorLocalPath;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // 同期のTIMER中止
                timer1.Enabled = false;
                bool endFlg = true;
                if (processIDList.Count > 0)
                {
                    Process[] ps = Process.GetProcesses();
                    foreach (Process item in ps)
                    {
                        if (processIDList.Contains(item.Id))
                        {
                            endFlg = false;
                            break;
                        }
                    }
                    if (endFlg)
                    {
                        timer1.Enabled = false;
                        EXEDoEndTime = DateTime.Now;
                        //
                        SendMail smail = new SendMail();
                        //
                        StringBuilder sb = new StringBuilder();
                        sb.Append("復元結果の報告：");
                        sb.Append("\r\n");
                        sb.Append("復元の執行を完了しました。");
                        sb.Append("\r\n");
                        sb.Append("復元開始時間: " + EXEDoStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.Append("\r\n");
                        sb.Append("復元完了時間: " + EXEDoEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.Append("\r\n");

                        // Smtp Auth認証でメール送信
                        smail.SmtpSend("mail35.heteml.jp", 587, "jiangtao@bridge.vc", "88xiaoerduo", "jiangtao@bridge.vc", ToMailList,
                            "復元結果の報告", sb.ToString(), @"");
                        MessageBox.Show("復元を完了しました。", "復元結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // 同期のTIMER起動
                        timer1.Enabled = true;
                    }
                }
                else
                {
                    // 同期のTIMER起動
                    timer1.Enabled = true;
                }
            }
            catch (Exception exb)
            {
                logger.Error(exb.Message);
                timer1.Enabled = true;
            }
        }

        private void comboBoxObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 選択の場合
            monitorServer defaultMonitorServer = (monitorServer)monitorServerHashList[this.comboBoxObject.SelectedValue];
            textBoxJobPath.Text = @"\\" + defaultMonitorServer.monitorServerIP + @"\" + defaultMonitorServer.startFile.TrimStart('\\');
            textBoxLocalPath.Text = defaultMonitorServer.monitorLocalPath;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            NetWorkFileShare netWorkFileShare = new NetWorkFileShare();
            string sharePath = @"\\" + this.textBoxIP.Text.Trim();
            if (netWorkFileShare.ConnectState(sharePath, textBoxUserID.Text.Trim(), textBoxPassword.Text.Trim()))
            {
                MsgHelper.InfoMsg(ValidationRegex.C001, ValidationRegex.publicTitle);
                this.buttonTest.Enabled = false;
                this.connectFlg = true;
                this.buttonCopyStart.Enabled = true;
            }
            else
            {
                MsgHelper.InfoMsg(ValidationRegex.C002, ValidationRegex.publicTitle);
                this.buttonTest.Enabled = true;
                this.connectFlg = false;
                this.buttonCopyStart.Enabled = false;
            }
        }

        private void buttonCopyStart_Click(object sender, EventArgs e)
        {
            try
            {
                // 同期のTIMER起動
                timer1.Enabled = true;
                processIDList.Clear();
                EXEDoStartTime = DateTime.Now;
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログフォルダーパス
                string logDirPath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir;
                if (!Directory.Exists(logDirPath))
                {
                    Directory.CreateDirectory(logDirPath);
                }
                // ログクリア
                DeleteFile(logDirPath);
                // BATフォルダーパス
                string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                string batDirPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir;
                if (!Directory.Exists(batDirPath))
                {
                    Directory.CreateDirectory(batDirPath);
                }
                // ログクリア
                DeleteFile(batDirPath);
                // 選択の場合
                monitorServer selectMonitorServer = (monitorServer)monitorServerHashList[this.comboBoxObject.SelectedValue];
                // コピー元
                DirectoryInfo CopySrcDir = new DirectoryInfo(selectMonitorServer.monitorLocalPath);
                // コピー先
                string targetDirPath = @"\\" + textBoxIP.Text.Trim() + @"\" + textBoxSavePath.Text.Trim().TrimStart('\\');
                DirectoryInfo CopyTargetDir = new DirectoryInfo(targetDirPath);
                if (CopySrcDir.Exists && CopyTargetDir.Exists)
                {
                    CopyListen(CopySrcDir.FullName.TrimEnd('\\'), CopyTargetDir.FullName.TrimEnd('\\'), copyListenTime, MTNum, RNum, WNum);
                    //XCopy(CopySrcDir.FullName.TrimEnd('\\'), CopyTargetDir.FullName.TrimEnd('\\'));
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
        void XCopy(string srcpath, string exportpath)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // コマンド処理
                dircommand.Append("xcopy /s /h /d /c /y /J ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\"");
                // BAT作成
                string batFileName = "XCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
                // 今のプログラムパス
                string batPath = System.AppDomain.CurrentDomain.BaseDirectory + batDir + "\\" + batFileName;
                CreateBAT batClass = new CreateBAT(batPath);
                batClass.Write(dircommand.ToString());
                defaultUserDeskTop.StartInfo.FileName = batPath;
                defaultUserDeskTop.StartInfo.UseShellExecute = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardInput = true;
                //defaultUserDeskTop.StartInfo.RedirectStandardOutput = true;
                defaultUserDeskTop.StartInfo.CreateNoWindow = true;
                //defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                defaultUserDeskTop.Start();
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Dirコマンドフォルダーリスト出力
        /// </summary>
        /// <param name="monitorID"></param>
        /// <param name="srcpath"></param>
        /// <param name="exportpath"></param>
        /// <param name="listentime"></param>
        /// <param name="threadnum"></param>
        /// <param name="retrynum"></param>
        /// <param name="retrytime"></param>
        void CopyListen(string srcpath, string exportpath, string listentime, string threadnum, string retrynum, string retrytime)
        {
            StringBuilder dircommand = new StringBuilder();
            Process defaultUserDeskTop = new Process();
            try
            {
                // 配置ファイル名
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-" + Common.RandomCode.GetCode(16) + ".log";
                string datenowDir = DateTime.Now.ToString("yyyy-MM-dd");
                // 今のログパス
                string logpath = System.AppDomain.CurrentDomain.BaseDirectory + datenowDir + "\\" + fileName;
                LogFile(logpath);
                // コマンド処理
                dircommand.Append("robocopy ");
                dircommand.Append("\"" + srcpath + "\" ");
                dircommand.Append("\"" + exportpath + "\" ");
                dircommand.Append(" /MIR ");
                //dircommand.Append(" /MOT:" + listentime + " ");
                dircommand.Append(" /MT[:" + threadnum + "] ");
                dircommand.Append(" /R:" + retrynum + " ");
                dircommand.Append(" /W:" + retrytime + " ");
                dircommand.Append(" /LOG+:");
                dircommand.Append("\"" + logpath + "\"");
                dircommand.Append(" /NP /NDL /TEE /XJD /XJF ");
                // BAT作成
                string batFileName = "ROBOCOPY-" + Common.RandomCode.GetCode(16) + ".bat";
                string batDir = DateTime.Now.ToString("yyyy-MM-dd") + "Bat";
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
                //defaultUserDeskTop.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                defaultUserDeskTop.Start();
                // 転送のプロセスIDの保存(CMD)
                processIDList.Add(defaultUserDeskTop.Id);
                //defaultUserDeskTop.StandardInput.WriteLine("exit");
                //defaultUserDeskTop.WaitForExit();
                //defaultUserDeskTop.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
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
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
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
