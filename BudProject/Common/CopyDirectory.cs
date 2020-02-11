using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common
{
    public class CopyDirectory
    {
        /// <summary>
        /// コピー元
        /// </summary>
        private DirectoryInfo _Source;
        /// <summary>
        /// コピー先
        /// </summary>
        private DirectoryInfo _Target;
        /// <summary>
        /// コピー元
        /// </summary>
        private string _FileSource;
        /// <summary>
        /// コピー先
        /// </summary>
        private string _FileTarget;
        /// <summary>
        /// コピー完了
        /// </summary>
        public delegate void CopyEnd();
        public event CopyEnd MyCopyEnd;
        /// <summary>
        /// エラーリスト
        /// </summary>
        public List<string> _Errorlist;
        /// <summary>
        /// ファイルとフオルダーの判断
        /// </summary>
        private string fileorFolderFlg;

        /// <summary>
        /// ファイルまたはフオルダのコピー（ファイルを含んでいる）
        /// </summary>
        /// <param name="p_SourceDirectory">コピー元</param>
        /// <param name="p_TargetDirectory">コピー先</param>
        public CopyDirectory(string p_SourceDirectory, string p_TargetDirectory)
        {
            if (Directory.Exists(p_SourceDirectory))
            {
                if (!Directory.Exists(p_TargetDirectory))
                {
                    Directory.CreateDirectory(p_TargetDirectory);
                }
                _Source = new DirectoryInfo(p_SourceDirectory);
                _Target = new DirectoryInfo(p_TargetDirectory);
                // フオルダのコピー
                fileorFolderFlg = "0";
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(p_TargetDirectory)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(p_TargetDirectory));
                }
                _FileSource = p_SourceDirectory;
                _FileTarget = p_TargetDirectory;
                // フオルダのコピー
                fileorFolderFlg = "1";
            }
            _Errorlist = new List<string>();
        }

        /// <summary>
        /// コピーの開始
        /// </summary>
        public void StarCopy()
        {
            if (fileorFolderFlg.Equals("0"))
            {
                CopyFolder(_Source, _Target, ref _Errorlist);
            }
            else
            {
                CopyFile(_FileSource, _FileTarget, ref _Errorlist);
            }
            if (MyCopyEnd != null) MyCopyEnd();
        }
        /// <summary>
        /// 指定のフオルダーにファイルをコピーする関数
        /// </summary>
        /// <param name="source">コピー元</param>
        /// <param name="target">コピー先</param>
        private void CopyFolder(DirectoryInfo p_Source, DirectoryInfo p_Target, ref List<string> errorlist)
        {
            ChildFolderAndFileCopy(p_Source, p_Target, ref errorlist);
        }

        /// <summary>
        /// 指定のファイルをコピーする関数
        /// </summary>
        /// <param name="source">コピー元</param>
        /// <param name="target">コピー先</param>
        private void CopyFile(string p_Source, string p_Target, ref List<string> errorlist)
        {
            try
            {
                if (File.Exists(p_Target))
                {
                    if (FileSystem.IsFileReadOnly(p_Target))
                    {
                        FileSystem.SetFileReadAccess(p_Target, false);
                    }
                }
                File.Copy(p_Source, p_Target, true);
            }
            catch (Exception e)
            {
                errorlist.Add(e.Message);
            }
        }

        /// <summary>
        /// 指定のフオルダーにファイルをコピーする関数
        /// </summary>
        /// <param name="source">コピー元</param>
        /// <param name="target">コピー先</param>
        /// <param name="errorlist">エラーリスト</param>
        private void ChildFolderAndFileCopy(DirectoryInfo p_Source, DirectoryInfo p_Target, ref List<string> errorlist)
        {
            // フオルダーの存在性の判断
            if (!Directory.Exists(p_Target.FullName))
            {
                Directory.CreateDirectory(p_Target.FullName);
            }
            FileSystemInfo[] fsis = p_Source.GetFileSystemInfos();
            if (fsis.Count() > 0)
            {
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi is FileInfo)
                    {
                        if (fsi.Exists)
                        {
                            FileInfo copyFile = fsi as FileInfo;
                            try
                            {
                                string targetFilePath = Path.Combine(p_Target.FullName, copyFile.Name);
                                if (File.Exists(targetFilePath))
                                {
                                    if (FileSystem.IsFileReadOnly(targetFilePath))
                                    {
                                        FileSystem.SetFileReadAccess(targetFilePath, false);
                                    }
                                }
                                copyFile.CopyTo(targetFilePath, true);
                            }
                            catch (Exception e)
                            {
                                errorlist.Add(p_Target.FullName + e.Message);
                                //「処理が続行できなくなる」バグ対応 2014/01/30 王丹
                                continue;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            DirectoryInfo sourceSubDir = (DirectoryInfo)fsi;
                            DirectoryInfo targetSubDir = new DirectoryInfo(Path.Combine(p_Target.FullName, sourceSubDir.Name));
                            ChildFolderAndFileCopy(sourceSubDir, targetSubDir, ref errorlist);
                        }
                        catch (Exception e)
                        {
                            errorlist.Add(p_Target.FullName + e.Message);
                            //「処理が続行できなくなる」バグ対応 2014/01/30 王丹
                            continue;
                        }
                    }
                }
            }
        }
    }
}
