using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace BudFileCheckListen
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
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
                MessageBox.Show("BudFileCheckListenRemoteプログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BudFileCheckListen());
            }
        }
    }
}
