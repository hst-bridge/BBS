using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using DiskSpaceChecker.Common.Util;

namespace DiskSpaceChecker.Exception
{
    /// <summary>
    /// グローバル異常な管理
    /// </summary>
   internal static class GlobalExceptionManager
    {
       /// <summary>
       /// グローバル異常な登録 
       /// </summary>
       public static void Init()
       {
           //アプリケーションの異常な処理を設定する：ThreadException
           Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
           //UIスレッドを異常な処理
           Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
           //非UIスレッドを異常な処理
           AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
       }

       private readonly static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

       /// <summary>
       /// UIスレッドを異常な処理
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
       {
           string str = GetExceptionMsg(e.Exception, e.ToString());
           MessageBox.Show(MessageUtil.GetMessage("SystemErrorMsg"), MessageUtil.GetMessage("SystemErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
           logger.Fatal(str);
           
           Application.ExitThread();
       }

       /// <summary>
       /// 非UIスレッドを異常な処理
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
       {
           string str = GetExceptionMsg(e.ExceptionObject as System.Exception, e.ToString());
           MessageBox.Show(MessageUtil.GetMessage("SystemErrorMsg"), MessageUtil.GetMessage("SystemErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
           logger.Fatal(str);

           Application.ExitThread();
       }

       /// <summary>
       /// カスタム異常なメッセージを生成
       /// </summary>
       /// <param name="ex">異常なオブジェクト</param>
       /// <param name="backStr">予備の異常なメッセージ：効果的なEXがnull</param>
       /// <returns>異常な文字列のテキスト</returns>
       static string GetExceptionMsg(System.Exception ex, string backStr)
       {
           StringBuilder sb = new StringBuilder();
           sb.AppendLine("****************************Message****************************");
           sb.AppendLine("【DateTime】：" + DateTime.Now.ToString());
           if (ex != null)
           {
               sb.AppendLine("【Type】：" + ex.GetType().Name);
               sb.AppendLine("【Message】：" + ex.Message);
               sb.AppendLine("【StackTrace】：" + ex.StackTrace);
           }
           else
           {
               sb.AppendLine("【Unhandled exception】：" + backStr);
           }
           sb.AppendLine("***************************************************************");
           return sb.ToString();
       }
    }
}
