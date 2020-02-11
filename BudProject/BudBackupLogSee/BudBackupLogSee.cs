using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;
using System.Reflection;
using System.Diagnostics;
using BudBackupLogSee.Common;
using System.Threading;

namespace BudBackupLogSee
{
    public partial class BudBackupLogSee : Form
    {
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// ログ 
        /// </summary>
        private string logPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];

        public BudBackupLogSee()
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

        private void BudBackupLogSee_Load(object sender, EventArgs e)
        {
            // ログ選択
            radioButtonCopy.Checked = true;
            radioButtonTransfer.Checked = false;
            //
            logtimer.Interval = 10000;
            logtimer.Elapsed += new System.Timers.ElapsedEventHandler(LogDo);
            logtimer.Start();
        }

        void CopyAndCheck()
        {
        }

        /// <summary>
        /// コピーとチェック
        /// </summary>
        void CopyAndCheck(string logsrc, string logtarget, int cfFlg)
        {
            XCopy(logsrc, logtarget);
            if (radioButtonCopy.Checked)
            {
                //Grepの実行
                string[] files = Grep(logtarget,
                    "------------------------------------------------------------------------------[\\s]*------------------------------------------------------------------------------",
                    "*.log", false);
                //結果の表示
                foreach (string delFile in files)
                {
                    try
                    {
                        File.Delete(delFile);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
            }
        }

        private void buttonSerach_Click(object sender, EventArgs e)
        {
            //
            listBoxObject.Items.Clear();
            // ログ保存
            DirectoryInfo logDir = new DirectoryInfo(logPath);
            // コピーログ
            if (radioButtonCopy.Checked) 
            {
                string logDirPath = Path.Combine(logDir.FullName, "RoboCopyLogToCopy-" + DateTime.Now.ToString("yyyy-MM-dd"));
                string logDirBKPath = Path.Combine(logDir.FullName, "RoboCopyLogToCopyBK-" + dateTimePickerLog.Value.ToString("yyyy-MM-dd"));
                if (Directory.Exists(logDirPath))
                {
                    if (!Directory.Exists(logDirBKPath))
                    {
                        Directory.CreateDirectory(logDirBKPath);
                    }
                    // ログバックアップ
                    ProgressbarEx.Progress.StartProgessBar(new ProgressbarEx.ShowProgess(CopyAndCheck));
                    //Thread.Sleep(2000);
                    // ログフォルダーパス
                    DirectoryInfo copyLogDir = new DirectoryInfo(logDirBKPath);
                    FileInfo[] copyFileLogList = copyLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo fileInfo in copyFileLogList)
                    {
                        listBoxObject.Items.Add(fileInfo);
                    }
                }
                else
                {
                    MessageBox.Show("該当日付のログが存在していません。");
                }
            }
            else if (radioButtonTransfer.Checked) 
            {
                string transferSrcDirPath = Path.Combine(logDir.FullName, "RoboCopyLogToTransfer-" + DateTime.Now.ToString("yyyy-MM-dd"));
                string transferLogDirPath = Path.Combine(logDir.FullName, "RoboCopyLogToTransferBK-" + dateTimePickerLog.Value.ToString("yyyy-MM-dd"));
                if (Directory.Exists(transferSrcDirPath))
                {
                    if (!Directory.Exists(transferLogDirPath))
                    {
                        Directory.CreateDirectory(transferLogDirPath);
                    }
                    // ログバックアップ
                    CopyAndCheck(transferSrcDirPath, transferLogDirPath, 1);
                    Thread.Sleep(3000);
                    // ログフォルダーパス
                    DirectoryInfo transferLogDir = new DirectoryInfo(transferLogDirPath);
                    FileInfo[] transferFileLogList = transferLogDir.GetFiles("*.log", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo fileInfo in transferFileLogList)
                    {
                        listBoxObject.Items.Add(fileInfo);
                    }
                }
                else
                {
                    MessageBox.Show("該当日付のログが存在していません。");
                }
            }
        }

        /// <summary>
        /// LogDo同期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogDo(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                // 同期のTIMER中止
                logtimer.Enabled = false;
                ////
                //DirectoryInfo logDir = new DirectoryInfo(logPath);
                //string logDirPathBK = Path.Combine(logDir.FullName, "RoboCopyLogToCopyBK-" + dateTimePickerLog.Value.ToString("yyyy-MM-dd"));
                //// ログバックアップのフオルダー
                //if (!Directory.Exists(logDirPathBK))
                //{
                //    Directory.CreateDirectory(logDirPathBK);
                //}
                ////Grepの実行
                //string[] files = Grep(logDirPathBK,
                //    "------------------------------------------------------------------------------[\\s]*------------------------------------------------------------------------------",
                //    "*.log", false);
                ////結果の表示
                //foreach (string delFile in files)
                //{
                //    try
                //    {
                //        File.Delete(delFile);
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error(ex.Message);
                //    }
                //}
                // 同期のTIMER起動
                logtimer.Enabled = true;
            }
            catch (Exception exb)
            {
                logger.Error(exb.Message);
                logtimer.Enabled = true;
            }
        }

        private void listBoxObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileInfo selelctedFileInfo = (FileInfo)listBoxObject.SelectedItem;
            if (selelctedFileInfo != null)
            {
                if (selelctedFileInfo.Exists)
                {
                    FileStream filestream = File.OpenRead(selelctedFileInfo.FullName);
                    int i = 0;
                    try
                    {
                        StreamReader reader = new StreamReader(filestream, System.Text.Encoding.Default, true);
                        while (i < 13)
                        {
                            reader.ReadLine();
                            i++;
                        }
                        string logContent = reader.ReadToEnd();
                        richTextBoxLogAll.Text = logContent;

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
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
        /// Grepを行う
        /// </summary>
        /// <param name="dirPath">フォルダのパス。</param>
        /// <param name="pattern">検索する正規表現のパターン。</param>
        /// <param name="fileWildcards">対象とするファイル。</param>
        /// <param name="ignoreCase">大文字小文字を区別するか。</param>
        /// <returns>見つかったファイルパスの配列。</returns>
        public static string[] Grep(
            string dirPath, string pattern, string fileWildcards, bool ignoreCase)
        {
            System.Collections.ArrayList fileCol =
                new System.Collections.ArrayList();

            //正規表現のオプションを設定
            System.Text.RegularExpressions.RegexOptions opts =
                System.Text.RegularExpressions.RegexOptions.None;
            if (ignoreCase)
                opts |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            System.Text.RegularExpressions.Regex reg =
                new System.Text.RegularExpressions.Regex(pattern, opts);

            //フォルダ内にあるファイルを取得
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(dirPath);
            System.IO.FileInfo[] files = dir.GetFiles(fileWildcards);
            foreach (System.IO.FileInfo f in files)
            {
                //一つずつファイルを調べる
                if (ContainTextInFile(f.FullName, reg))
                    fileCol.Add(f.FullName);
            }

            return (string[])fileCol.ToArray(typeof(string));
        }

        //ファイルの内容がpatternに一致するか調べる
        private static bool ContainTextInFile(
            string filePath, System.Text.RegularExpressions.Regex reg)
        {
            //ファイルを読み込む
            System.IO.StreamReader strm = null;
            string txt = "";
            try
            {
                strm = new System.IO.StreamReader(
                    filePath, System.Text.Encoding.Default, true);
                txt = strm.ReadToEnd();
            }
            finally
            {
                strm.Close();
            }

            return reg.IsMatch(txt);
        }
    }
}
