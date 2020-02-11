using System;
using System.Windows.Forms;
using BudDBSync.Exceptions;
using BudDBSync.Util.Message;
using BudDBSync.Util;
using System.Diagnostics;

namespace BudDBSync
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメインエントランス
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                int count = 0;
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName == Process.GetCurrentProcess().ProcessName)
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    MessageBox.Show("BudDBSyncプログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                    return;
                }

                //グローバル異常な登録 
                GlobalExceptionManager.Init();
                #region アプリケーションのメインエントランス
                int count = 0;
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName == Process.GetCurrentProcess().ProcessName)
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    MessageBox.Show("BudDBSyncプログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmMain());
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(MessageUtil.GetMessage("SystemErrorMsg"), MessageUtil.GetMessage("SystemErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }
        }

    }
}
