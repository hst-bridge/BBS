using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using BudFileCheckListen.Common.Util;
using System.Runtime.InteropServices;
using BudFileCheckListen.Models;
using System.Text.RegularExpressions;
using System.IO;

namespace BudFileCheckListen.Common.FileSystem
{
    /// <summary>
    /// 用于文件系统工具类
    /// </summary>
    class FSUtil
    {
        /// <summary>
        /// ログ
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary> 
        /// Txtファイル
        /// </summary> 
        /// <param   name= "FilePath "> </param> 
        public void CreateTxtFile(string FilePath)
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            try
            {
                if (fileInfo.Exists)
                {
                    File.Delete(FilePath);
                }
                else
                {
                    if (!Directory.Exists(fileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary> 
        /// ファイル削除
        /// </summary> 
        /// <param   name= "DirPath "> </param> 
        public void DeleteFile(string DirPath)
        {
            try
            {
                string pattern = "*.*";
                string[] strFileName = Directory.GetFiles(DirPath, pattern);
                foreach (var item in strFileName)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 一般因为文件名异常，造成Filenotfound异常
        /// 但DirectoryInfo类的EnumerateFiles函数可以判别出来
        /// 特用此方法获取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo GetFileInfoWeird(string fullpath)
        {
            bool isOK = false;
            string message = string.Empty;
            FileInfo fi = null;
            #region 如果文件路径正常,则直接返回
            try
            {
                fi = new FileInfo(fullpath);
                long size = fi.Length;
                isOK = true;
            }
            catch (Exception ex)
            {
              try{
                //如果有错
                #region 通过上级目录 扫描获取
                  if (fullpath.Contains(' '))
                  {
                      //获取正常的上级目录
                      string dp = fullpath.Substring(0, fullpath.LastIndexOf('\\'));
                      while (!" ".Equals(dp.Substring(dp.Length - 1)))
                      {

                          dp = dp.Substring(0, dp.LastIndexOf('\\'));

                      }
                      dp = dp.Substring(0, dp.LastIndexOf('\\'));

                      DirectoryInfo dif = new DirectoryInfo(dp);
                      fi = dif.EnumerateFiles("*", SearchOption.AllDirectories).FirstOrDefault(x => x.FullName.Equals(fullpath)); ;

                      if (fi != null)
                      {
                          isOK = true;
                      }



                  }
              }
              catch (Exception e) {
                  logger.Error(MessageUtil.GetExceptionMsg(e, ""));
                  isOK = false;
              }

              if (!isOK)
              {
                  logger.Error(fullpath + Environment.NewLine + MessageUtil.GetExceptionMsg(ex, ""));
                  return null;
              }
                #endregion 
            }
            #endregion

            return fi;

        }

        #region get fullpath
        /**
         * PathTooLongException
         * 指定されたパス、ファイル名、またはその両方が長すぎます。完全限定型名は 260 文字未満で指定し、ディレクトリ名は 248 未満で指定してください。
         * 
         * reflection 
         */
        static FieldInfo fld = typeof(FileSystemInfo).GetField(
                                       "FullPath",
                                        BindingFlags.Instance |
                                        BindingFlags.NonPublic);
        public static string GetFullPath(FileSystemInfo fsi)
        {
            return fld.GetValue(fsi) as string;
        }
        #endregion

        #region implement by call win32api 
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

        #region win32api import
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
        #endregion
        public static bool CheckDirectoryEmpty_Fast(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }
            try
            {
                if (Alphaleonis.Win32.Filesystem.Directory.Exists(path))
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
                throw new DirectoryNotFoundException();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// get fileinfo with win32 api findfirstfile
        /// 
        /// 20141217 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Obsolete("this function with failed when the file extension end with a backspace;for example:【aa.pdf 】,pleas use FindSpecialFileInfo method", true)]
        public static FileInfoStruct GetFileInfoBYWin32API(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            WIN32_FIND_DATA findData;
            if (!path.Substring(0, 2).Equals(@"\\")) path = String.Concat(@"\\?\", path);

            var findHandle = FindFirstFile(path, out findData);

            if (findHandle != INVALID_HANDLE_VALUE)
            {
                try
                {
                    FileInfoStruct fis = new FileInfoStruct();
                    fis.Name = findData.cFileName;
                    //extension
                    int eIndex = findData.cFileName.LastIndexOf('.');
                    if (eIndex > 0)
                    {
                        fis.Extension = findData.cFileName.Substring(eIndex);
                    }
                    else
                    {
                        fis.Extension = "";
                    }

                    //length
                    fis.Length = ((((long)findData.nFileSizeHigh) << 32) + findData.nFileSizeLow).ToString();
                    //CreationTime
                    fis.CreationTime = ConvertFromFileTime(findData.ftCreationTime);
                    //LastAccessTime
                    fis.LastAccessTime = ConvertFromFileTime(findData.ftLastAccessTime);
                    //LastWriteTime
                    fis.LastWriteTime = ConvertFromFileTime(findData.ftLastWriteTime);

                    return fis;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    FindClose(findHandle);
                }
            }

            throw new Exception("Failed to find file",
                         Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        }

        

        static DateTime ConvertFromFileTime(System.Runtime.InteropServices.ComTypes.FILETIME ft)
        {
            var dwLowDateTime = (uint)ft.dwLowDateTime;
            long hFT2 = (long)((((ulong)ft.dwHighDateTime) << 32) | dwLowDateTime);

            return DateTime.FromFileTime(hFT2);
        }

        public static FileInfoStruct FindSpecialFileInfo(string fpath)
        {
            FileInfoStruct fi = new FileInfoStruct();
            int lastIndex = fpath.LastIndexOf(Path.DirectorySeparatorChar);
            fi.Name = fpath.Substring(lastIndex + 1);

            //extension
            int eIndex = fi.Name.LastIndexOf('.');
            if (eIndex > 0)
            {
                string extension = fi.Name.Substring(eIndex);
                if (Regex.IsMatch(extension, @"^\.[a-zA-Z]{1,10}$"))
                {
                    fi.Extension = extension;
                }
                else fi.Extension = "";
            }
            else
            {
                fi.Extension = "";
            }

            //length
            try
            {
                Alphaleonis.Win32.Filesystem.FileInfo fileinfo = new Alphaleonis.Win32.Filesystem.FileInfo(fpath);
                fi.Length = fileinfo.Length.ToString();
                fi.CreationTime = fileinfo.CreationTime;
                fi.LastWriteTime = fileinfo.LastAccessTime;
                fi.LastAccessTime = fileinfo.LastAccessTime;
            }
            catch (Exception ex)
            {
                DateTime dt = DateTime.Now;
                fi.Length = "0";
                fi.CreationTime = dt;
                fi.LastWriteTime = dt;
                fi.LastAccessTime = dt;
                
            }

            return fi;
        }

        private static FileInfoStruct GetFileInfoByWin32(string fpath)
        {
            if (string.IsNullOrEmpty(fpath))
            {
                throw new ArgumentNullException(fpath);
            }

            int lastIndex = fpath.LastIndexOf(Path.DirectorySeparatorChar);
            string filename = fpath.Substring(lastIndex + 1);
            string path = fpath.Substring(0, lastIndex);

            try
            {

                if (!path.Substring(0, 2).Equals(@"\\")) path = String.Concat(@"\\?\", path);

                string basePath = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                path = basePath + "*";

                WIN32_FIND_DATA findData;

                var findHandle = FindFirstFile(path, out findData);

                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    try
                    {
                        do
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                if (filename.Equals(findData.cFileName))
                                {
                                    return GetFileInfoStruct(findData);
                                }
                            }
                        } while (FindNextFile(findHandle, out findData));
                    }
                    finally
                    {
                        FindClose(findHandle);
                    }
                }

                throw new Exception("Failed to find the file",
                    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

            }
            catch (Exception)
            {
                throw;
            }
        }

        static FileInfoStruct GetFileInfoStruct(WIN32_FIND_DATA findData)
        {
            FileInfoStruct fis = new FileInfoStruct();
            fis.Name = findData.cFileName;
            //extension
            int eIndex = findData.cFileName.LastIndexOf('.');
            if (eIndex > 0)
            {
                string extension = findData.cFileName.Substring(eIndex);
                if (Regex.IsMatch(extension, @"^\.[a-zA-Z]{1,10}$"))
                {
                    fis.Extension = extension;
                }
                else fis.Extension = "";
            }
            else
            {
                fis.Extension = "";
            }

            //length
            fis.Length = ((((long)findData.nFileSizeHigh) << 32) + findData.nFileSizeLow).ToString();
            //CreationTime
            fis.CreationTime = ConvertFromFileTime(findData.ftCreationTime);
            //LastAccessTime
            fis.LastAccessTime = ConvertFromFileTime(findData.ftLastAccessTime);
            //LastWriteTime
            fis.LastWriteTime = ConvertFromFileTime(findData.ftLastWriteTime);
            return fis;
        }
        #endregion
    }
}
