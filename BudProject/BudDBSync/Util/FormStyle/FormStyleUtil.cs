using System;
using System.Runtime.InteropServices;
using BudDBSync.Util;

namespace BudDataSync.Util.FormStyle
{
    /// <summary>
    /// custom form style . xiecongwen
    /// </summary>
    class FormStyleUtil
    {
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
            catch (Exception ex)
            {
                LogManager.WriteLog(LogFile.Error,ex.Message);
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
