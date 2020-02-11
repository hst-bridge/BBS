using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BudCopyListen
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain(arg));
        }
    }
}
