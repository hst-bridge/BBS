using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BudLogManage.Exception;
using BudLogManage.Common.Util;
using System.Reflection;
using log4net;
using System.Diagnostics;

namespace BudLogManage
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
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
                    MessageBox.Show("BudLogManageプログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            catch (System.Exception ex)
            {
                MessageBox.Show(MessageUtil.GetMessage("SystemErrorMsg"), MessageUtil.GetMessage("SystemErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Fatal(ex.Message);
            }
        }

        private readonly static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
