using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace budbackup.CommonWeb.Helper
{
    public class FilesystemHelper
    {

        public static string getUncPath(string path)
        {
            if (path.Substring(0, 2).Equals(@"\\"))
            {
                path = string.Concat(@"\\?\UNC\", path.Substring(2));
            }
            else
            {
                path = String.Concat(@"\\?\", path);
            }

            string basePath = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            path = basePath + "*";
            return path;
        }

        public static IEnumerable<string> EnumFilesYield(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }


            WIN32_FIND_DATA findData;

            NTree<string> parent = new NTree<string>(path);
            //后序遍历树
            NTree<string> child = null;
            while (true)
            {
                string tempPath = parent.Data;
                string uncPath = getUncPath(parent.Data);
                var findHandle = FindFirstFile(uncPath, out findData);

                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    try
                    {
                        do
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                if (findData.dwFileAttributes == 16)
                                {
                                    parent.AddChild(tempPath + Path.DirectorySeparatorChar + findData.cFileName);
                                }
                                else
                                {
                                    yield return tempPath + Path.DirectorySeparatorChar + findData.cFileName;
                                }
                            }
                        } while (FindNextFile(findHandle, out findData));

                    }
                    finally
                    {
                        FindClose(findHandle);
                    }

                }

                //判断是否有子节点
                child = parent.PushChild();
                if (child == null)
                {
                //如果没有子节点，则回溯至上层,否则跳出（表示遍历结束）
                again: if (parent.Depth > 0)
                    {
                        parent = parent.Parent;
                    }
                    else yield break;

                    child = parent.PushChild();
                    if (child == null) goto again;

                }

                //作为父节点 进行循环
                parent = child;

            }

            //log
            //Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());


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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

    }
}