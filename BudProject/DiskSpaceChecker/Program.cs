using DiskSpaceChecker.Common.Util;
using DiskSpaceChecker.Exception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskSpaceChecker
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
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
                    MessageBox.Show("DiskSpaceCheckerプログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                    return;
                }

                //グローバル異常な登録 
                GlobalExceptionManager.Init();
                #region アプリケーションのメインエントランス
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                #endregion
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(MessageUtil.GetMessage("SystemErrorMsg"), MessageUtil.GetMessage("SystemErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
