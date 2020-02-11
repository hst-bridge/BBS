using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace BudFileListen.Common
{
    public class FileSystem
    {
        /// <summary>
        /// チェック無用のファイル
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
         public static bool CheckSpecialFile(string fileName)
        {
            bool checkResult = true;

            if (String.Compare(fileName, ".DS_Store", true) == 0) 
            {
                checkResult = false;
            }
            else if (String.Compare(fileName, ".com.apple.timemachine.supported", true) == 0)
            {
                checkResult = false;
            }
            else if (String.Compare(fileName, "ICON", true) == 0)
            {
                checkResult = false;
            }
            else if (String.Compare(fileName, "Icon", true) == 0)
            {
                checkResult = false;
            }

            return checkResult;
        }

        /// <summary>
        /// COPY
        /// </summary>
        /// <param name="sPath">パスの元</param>
        /// <param name="dPath">パスの先</param>
        public static bool FileCopy(string sPath, string dPath)
        {
            try
            {
                if (Directory.Exists(sPath))
                {
                    if (!Directory.Exists(dPath))
                    {
                        Thread.Sleep(1000);
                        Directory.CreateDirectory(dPath);
                    }
                }
                else
                {
                    if (!Directory.Exists(Path.GetDirectoryName(dPath)))
                    {
                        Thread.Sleep(1000);
                        Directory.CreateDirectory(Path.GetDirectoryName(dPath));
                        Thread.Sleep(1000);
                        //「'XXX' へのアクセスが拒否されました」バグ対応 2014/01/27 王丹
                        if (File.Exists(dPath))
                        {
                            if (IsFileReadOnly(dPath))
                            {
                                SetFileReadAccess(dPath, false);
                            }
                        }
                        File.Copy(sPath, dPath, true);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        //「'XXX' へのアクセスが拒否されました」バグ対応 2014/01/27 王丹
                        if (File.Exists(dPath))
                        {
                            if (IsFileReadOnly(dPath))
                            {
                                SetFileReadAccess(dPath, false);
                            }
                        }
                        File.Copy(sPath, dPath, true);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="dPath">パスの先</param>
        public static bool FileDelete(string dPath)
        {
            try
            {
                if (Path.GetExtension(dPath) == string.Empty)
                {
                    if (Directory.Exists(dPath))
                    {
                        //Directory.Delete(dPath, true);
                        DirectoryInfo info = new DirectoryInfo(dPath);
                        DeleteFileByDirectory(info);
                    }
                    else if (File.Exists(dPath))
                    {
                        File.Delete(dPath);
                    }
                }
                else
                {
                    if (Directory.Exists(Path.GetDirectoryName(dPath)) && File.Exists(dPath))
                    {
                        File.Delete(dPath);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        ///   dir delete
        /// </summary>
        public static void DeleteFileByDirectory(DirectoryInfo info)
        {
            foreach (DirectoryInfo newInfo in info.GetDirectories())
            {
                DeleteFileByDirectory(newInfo);
            }
            foreach (FileInfo newInfo in info.GetFiles())
            {
                newInfo.Attributes = newInfo.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                newInfo.Delete();
            }
            info.Attributes = info.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
            info.Delete();
        }

        //「'XXX' へのアクセスが拒否されました」バグ対応 2014/01/27 王丹
        // Sets the read-only value of a file.
        public static void SetFileReadAccess(string FileName, bool SetReadOnly)
        {
            // Create a new FileInfo object.
            FileInfo fInfo = new FileInfo(FileName);
            // Set the IsReadOnly property.
            fInfo.IsReadOnly = SetReadOnly;
        }

        //「'XXX' へのアクセスが拒否されました」バグ対応 2014/01/27 王丹
        // Returns wether a file is read-only.
        public static bool IsFileReadOnly(string FileName)
        {
            // Create a new FileInfo object.
            FileInfo fInfo = new FileInfo(FileName);
            // Return the IsReadOnly property value.
            return fInfo.IsReadOnly;
        }
    }
}
