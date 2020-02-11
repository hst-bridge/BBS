using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;
using BudSSH.DAL.SQLServer;
using BudSSH.Model.Behind;
using BudSSH.Common.Util;
using System.IO;
using Microsoft.VisualBasic;
using System.Reflection;
using log4net;
using System.Threading;
using BudSSH.Common;
using Renci.SshNet.Sftp;

namespace BudSSH.BLL
{
    /// <summary>
    /// 用于直接负责复制文件
    /// </summary>
    class SSHCopyManager
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 判断文件是否发生变化，若变化则copy文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="epi"></param>
        public void SSHCopy(Config config,ErrorPathInfo epi)
        {
            //获取mac端信息
            MonitorServerDAL msd = new MonitorServerDAL();
            MonitorServer ms = msd.GetMonitorServer(config, epi);
            if (ms == null)
            {
                Common.Util.LogManager.WriteLog(LogFile.Warning, "there is no MonitorServer that meet the conditions !");
                return;
            }
            SFTPProxy sftpProxy = null;
            var sshlog = new Common.Util.SSHLogManager();
            try
            {
                sftpProxy = new SFTPProxy(ms.monitorServerIP, ms.account, ms.password);
                #region 处理
                foreach (ErrorEntry ee in epi.PathList)
                {
                    string macPath = string.Empty;
                    try
                    {
                        //获取mac路径
                        macPath = MacPathConvert(epi.source, ee.Path, ms.monitorMacPath,ms.startFile);

                        //获取远端文件属性
                        SftpFile sf = sftpProxy.GetFileInfo(macPath);

                        if (!ErrorPathFilter.IsUpdate(config,sf,ms)) continue;
                        if (sf.IsDirectory)
                        {
                            #region 删除job端已经删除的文件

                            #endregion
                            foreach (SftpFile sfile in sftpProxy.Ls(sf))
                            {
                                #region  过滤无关文件
                                if (String.Compare(sfile.Name.Trim('\r'), ".DS_Store", true) == 0
                                                               || String.Compare(sfile.Name.Trim('\r'), ".com.apple.timemachine.supported", true) == 0
                                                               || String.Compare(sfile.Name.Trim('\r'), "Icon", true) == 0)
                                    continue;
                                #endregion
                                if (!ErrorPathFilter.IsUpdate(config, sfile, ms)) continue;

                                SSHCopyFile(sfile, ms,config.Path.OutputPath, sftpProxy);
                            }
                        }
                        else
                        {
                            SSHCopyFile(sf, ms,config.Path.OutputPath, sftpProxy);
                        }

                    }
                    catch (System.Exception ex)
                    {
                        Common.Util.LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                        sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = ee.Path });
                    }
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
            finally
            {
                if (sftpProxy != null)
                {
                    sftpProxy.Close();
                }
            }

        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sf"></param>
        /// <param name="ms"></param>
        /// <param name="sshOP">SSH output path</param>
        /// <param name="sftpProxy"></param>
        private void SSHCopyFile(SftpFile sf, MonitorServer ms, string sshOP, SFTPProxy sftpProxy)
        {
           

            var sshlog = new Common.Util.SSHLogManager();
            //获取本地合法路径
            string localPath = GetValidLocalPath(sf.FullName,ms.monitorMacPath, ms.monitorLocalPath);

            //获取SSH输出路径 暂时不支持网络位置
            if (string.IsNullOrWhiteSpace(sshOP.Trim()))
            {
                localPath = localPath.Substring(0, localPath.IndexOf(':') + 1) + "\\SSH\\" + ms.monitorServerName + "-SSH";
            }
            else
            {
                sshOP.TrimEnd('\\');
                localPath = localPath.Replace(ms.monitorLocalPath.TrimEnd('\\'), sshOP + "\\" + ms.monitorServerName + "-SSH");
            }
            #region 复制
            DateTime dateM = sf.Attributes.LastWriteTime;
            DateTime dateA = sf.Attributes.LastAccessTime;           

            #region
            try
            {
                Alphaleonis.Win32.Filesystem.FileInfo targetFile = new Alphaleonis.Win32.Filesystem.FileInfo(localPath);
                //如果文件没有发生改变，则不进行下载
                if (targetFile.Exists)
                {
                    if (targetFile.LastWriteTime == dateM) return;
                }

                if (!Directory.Exists(targetFile.DirectoryName))
                {
                    try
                    {
                        Directory.CreateDirectory(targetFile.DirectoryName);
                    }
                    catch (System.Exception) { }
                    if (!Directory.Exists(targetFile.DirectoryName))
                    {
                        LongPath.CreateDirectory(targetFile.DirectoryName);
                    }
                }

                sftpProxy.GetFile(sf.FullName, targetFile.FullName, true);

                targetFile.Refresh();
                if (targetFile.Exists)
                {
                    targetFile.CreationTime = dateM;
                    targetFile.LastWriteTime = dateM;
                    targetFile.LastAccessTime = dateA;
                }
                else
                {
                    #region
                    string errorDirToFileNameExtension = "";
                    if (sf.Name.IndexOf(".") > -1)
                    {
                        errorDirToFileNameExtension = sf.Name.Substring(sf.Name.IndexOf(@".") + 1);
                    }
                    Thread.Sleep(2000);
                    bool copyconfirm = true;
                    int confirmCoount = 0;
                    while (copyconfirm)
                    {
                        #region
                        if (confirmCoount < 3)
                        {
                            targetFile.Refresh();
                            if (targetFile.Exists)
                            {
                                DateTime fileFirstTime = targetFile.LastWriteTime;
                                Thread.Sleep(2000);
                                DateTime fileSecondTime = targetFile.LastWriteTime;
                                if (fileFirstTime.Equals(fileSecondTime))
                                {
                                    targetFile.CreationTime = dateM;
                                    targetFile.LastWriteTime = dateM;
                                    targetFile.LastAccessTime = dateA;
                                    copyconfirm = false;
                                }
                            }
                            else
                            {
                                Thread.Sleep(2000);
                            }
                            confirmCoount++;
                        }
                        else
                        {
                            break;
                        }
                        #endregion
                    }

                    #endregion
                }

                sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Success, Message = targetFile.FullName });
            }
            catch (ArgumentException ae)
            {
                logger.Error("localpath:" + localPath + Environment.NewLine + MessageUtil.GetExceptionMsg(ae, ""));
                sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = ms.monitorServerIP + ", " + sf.FullName });

            }
            catch (System.Exception ex)
            {
                logger.Error("localpath:" + localPath +Environment.NewLine+ MessageUtil.GetExceptionMsg(ex, ""));
                sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = ms.monitorServerIP + ", " + sf.FullName });

            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 将出错路径转换成Mac路径
        /// </summary>
        /// <param name="srcRoboCopyPath">Robocopy复制时的基础共享路径</param>
        /// <param name="objectpath">出错目录</param>
        /// <param name="macBasePath">共享目录在mac上的路径</param>
        /// <param name="startFile">第一级共享目录名称</param>
        private string MacPathConvert(string srcRoboCopyPath, string objectpath, string macBasePath,string startFile)
        {
            string macPath = string.Empty;
            try
            {
                string srcBaseSharePath = srcRoboCopyPath.Substring(0,srcRoboCopyPath.IndexOf(startFile)+startFile.Length);
                macPath = objectpath.Replace(srcBaseSharePath, macBasePath).Replace('\\', '/');
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
               throw;
            }
            return macPath;
        }

        /// <summary>
        /// 获取合法的本地路径
        /// </summary>
        /// <param name="destPath">目的路径</param>
        /// <param name="objectpath">报错路径</param>
        /// <returns></returns>
        private string GetValidLocalPath(string macPath,string macBasePath, string destBasePath)
        {
            string localpath = string.Empty;
            try
            {
                //替换为目的路径
                localpath = PathConvertToLocal(macPath,macBasePath, destBasePath);
                //特殊字符替换
                localpath = PathValidUtil.GetValidPath(localpath);

            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                throw;
            }

            return localpath;
        }

        /// <summary>
        /// フォルダーパスの変換
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="macPath"></param>
        /// <param name="targetPath"></param>
        private string PathConvertToLocal(string macPath, string macBasePath, string targetPath)
        {
            string returnFilePath = "";
            try
            {
                string[] mainpath = macBasePath.Split(new string[1] { @"/" }, StringSplitOptions.RemoveEmptyEntries);
                string[] subpath = macPath.Split(new string[1] { @"/" }, StringSplitOptions.RemoveEmptyEntries);
                string dirpath = "";
                bool indexflg = false;
                for (int i = 0; i < subpath.Length; i++)
                {
                    if (subpath[i].Equals(mainpath[mainpath.Length - 1]))
                    {
                        indexflg = true;
                        continue;
                    }

                    if (indexflg)
                    {
                        if (i < subpath.Length - 1)
                        {
                            dirpath += subpath[i] + @"\";
                        }
                    }
                }
                returnFilePath = targetPath + @"\" + dirpath + subpath[subpath.Length - 1];
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
            return returnFilePath;
        }


       

    }
}
