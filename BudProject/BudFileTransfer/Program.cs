using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace BudFileTransfer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            int count = 0;
            Process[] processes =Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == Process.GetCurrentProcess().ProcessName)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                MessageBox.Show("転送プログラムを実行している。", "メッセージ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BudFileTransfer());
            }
        }
    }
}
