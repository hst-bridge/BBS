using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DiskSpaceChecker.Common.Util
{
    /// <summary>
    /// custom form style . xiecongwen
    /// </summary>
    class FormStyleUtil
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// remove the maximize menu
        /// </summary>
        /// <param name="form"></param>
        public static void RemoveMaximizeMenu(System.Windows.Forms.Form form)
        {
            try
            {
                int hMenu;
                hMenu = GetSystemMenu(form.Handle.ToInt32(), 0);
                RemoveMenu(hMenu, SC_MAXIMIZE, MF_REMOVE);
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        [DllImport("USER32.DLL")]
        static extern int GetSystemMenu(int hwnd, int bRevert);

        [DllImport("USER32.DLL")]
        static extern int RemoveMenu(int hMenu, int nPosition, int wFlags);
        const int MF_REMOVE = 0x1000;
        const int SC_MAXIMIZE = 0xF030; //最大化
    }
}
