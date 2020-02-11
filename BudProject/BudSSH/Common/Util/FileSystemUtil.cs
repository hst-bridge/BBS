using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// 用于负责磁盘操作
    /// </summary>
    class FileSystemUtil
    {

        /// <summary>
        /// 创建目录 
        /// </summary>
        /// <param name="dir">绝对路径</param>
        public void MakeDir(string dir)
        {
            if (!Alphaleonis.Win32.Filesystem.Directory.Exists(dir)) Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(dir);
        }

        
        /// <summary> 
        /// ファイル削除
        /// </summary> 
        /// <param   name= "DirPath "> </param> 
        public void ClearDirectory(string DirPath)
        {
            try
            {
                string pattern = "*.log";
                string[] strFileName = Alphaleonis.Win32.Filesystem.Directory.GetFiles(DirPath, pattern);
                foreach (var item in strFileName)
                {
                    try
                    {
                        Alphaleonis.Win32.Filesystem.File.Delete(item);
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                        continue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.WriteLog(LogFile.System, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
        {
            try
            {
                Alphaleonis.Win32.Filesystem.FileInfo fileInfo = new Alphaleonis.Win32.Filesystem.FileInfo(path);
                if(fileInfo.Exists)
                    fileInfo.Delete();
            }
            catch (System.Exception ex)
            {
                LogManager.WriteLog(LogFile.System, MessageUtil.GetExceptionMsg(ex, ""));

            }
            
        }

        public void DeleteDir(string path)
        {
            try
            {
                Alphaleonis.Win32.Filesystem.DirectoryInfo di = new Alphaleonis.Win32.Filesystem.DirectoryInfo(path);
                if (di.Exists)
                    di.Delete(true);
            }
            catch (System.Exception ex)
            {
                LogManager.WriteLog(LogFile.System, MessageUtil.GetExceptionMsg(ex, ""));

            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public void DeleteFile(FileInfo fileInfo)
        {
            try
            {
                Alphaleonis.Win32.Filesystem.File.Delete(fileInfo.FullName);
                fileInfo.Refresh();
                if (fileInfo != null && fileInfo.Exists)
                {
                    LogManager.WriteLog(LogFile.System, "cannot delete file " + fileInfo.FullName);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.WriteLog(LogFile.System, fileInfo.FullName +Environment.NewLine+ MessageUtil.GetExceptionMsg(ex, ""));

            }
          
        }

        /// <summary>
        /// 获取此文件路径为准的最高层的为空的文件夹路径
        /// </summary>
        /// <param name="fpath"></param>
        /// <returns></returns>
        public string GetTheTopEmptyDirectoryPath(string fpath)
        {
            string topDP = string.Empty;
            string dirp = Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(fpath);
            while (IsDirectoryEmpty(dirp))
            {
                topDP = dirp;
                dirp = Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(dirp);
            }
            return topDP;
        }

        #region check whether directory is empty
        /// <summary>
        /// 判断是否有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsDirectoryEmpty(string dpath)
        {
            /**
             *  由于此种方式，在路径过长（大于260个字符）时报错，故放弃
             *  xiecongwen 20141216
             */
            //return !Directory.EnumerateFiles(dpath, "*", SearchOption.AllDirectories).Any();
            return CheckDirectoryEmpty_Fast(dpath);

        }


        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

        public static bool CheckDirectoryEmpty_Fast(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }
            try
            {

                string basePath = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                path = basePath + "*";

                WIN32_FIND_DATA findData;
                var findHandle = FindFirstFile(String.Concat(@"\\?\", path), out findData);

                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    try
                    {
                        bool empty = true;
                        do
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                if (findData.dwFileAttributes == 16)
                                {
                                    empty = CheckDirectoryEmpty_Fast(basePath + findData.cFileName);
                                }
                                else
                                {
                                    empty = false;
                                }
                            }
                        } while (empty && FindNextFile(findHandle, out findData));

                        return empty;
                    }
                    finally
                    {
                        FindClose(findHandle);
                    }
                }

                throw new System.Exception("Failed to get directory first file",
                    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
