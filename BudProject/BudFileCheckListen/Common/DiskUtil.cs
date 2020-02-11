using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudFileCheckListen.Common
{
    public class DiskUtil
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool GetDiskFreeSpaceEx(
            string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        /// <summary>
        /// 取得磁盘剩余空间百分比
        /// </summary>
        /// <param name="driveDirectoryName">驱动器名</param>
        /// <returns>剩余空间</returns>
        public static double GetFreeSpacePercent(string driveDirectoryName)
        {
            ulong freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;
            if (!driveDirectoryName.EndsWith(":\\"))
            {
                driveDirectoryName += ":\\";
            }
            GetDiskFreeSpaceEx(driveDirectoryName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);

            return freeBytesAvailable * 1.0 / totalNumberOfBytes;
        }

        /// <summary>
        /// 取得磁盘剩余空间 单位是G
        /// </summary>
        /// <param name="driveDirectoryName">驱动器名</param>
        /// <returns>剩余空间</returns>
        public static double GetFreeSpace(string driveDirectoryName)
        {
            ulong freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;
            if (!driveDirectoryName.EndsWith(":\\"))
            {
                driveDirectoryName += ":\\";
            }
            GetDiskFreeSpaceEx(driveDirectoryName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);

            return ((double)freeBytesAvailable) / 1073741824;
        }
    }
}
