using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;
using System.Text.RegularExpressions;
using BudSSH.Common.Util;
using BudSSH.DAL.SQLServer;
using BudSSH.Model.Behind;
using System.IO;
using BudSSH.Common;
using BudSSH.Common.Helper;

namespace BudSSH.BLL
{
    /// <summary>
    /// 用于同步
    /// </summary>
    class LocalSyncManager
    {

        #region dbsync

        /// <summary>
        /// 扫描数据库，判断mac端是否存在
        /// </summary>
        public void LookBackSync(Config config)
        {
            if (TimeCheckHelper.CheckTime("DBSyncTime", config.DBSyncTime))
            {
                //同步数据库 SSH输出 Mac （特殊字符文件）
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.DBSync, "start");

                DBSync(config);

                Common.Util.LogManager.WriteLog(Common.Util.LogFile.DBSync, "end");

                //执行了一个任务
                Signal.CompletedTaskCount++;
            }

        }

        /// <summary>
        /// 扫描数据库，判断是否在mac上存在
        /// </summary>
        /// <param name="config"></param>
        private void DBSync(Config config)
        {
            try
            {
                /*
                 * 分页获取SSHPathInfo,找出mac端已经删除文件对应的id，删除对应的本地文件并刷新数据库
                 * 
                 * 待确定的想法：如果发现mac端文件已经更新，则更新本地文件
                 */

                List<string> delIDList = new List<string>();
                SSHPathInfoDAL spid = new SSHPathInfoDAL();

                #region 主要处理
                try
                {

                    FileSystemUtil fsu = new FileSystemUtil();
                    //获取所有MonitorServer，以便于数据处理
                    MonitorServerDAL msd = new MonitorServerDAL();
                    List<MonitorServer> msList = msd.GetAllMonitorServer(config);

                    foreach (SSHPathInfo spi in spid.GetSSHPathInfo(config))
                    {
                        if (Signal.IsSystemStoping) break;
                        #region 判断Mac端文件是否存在,如果不存在，则删除本地SSH输出目录中对应文件
                        SFTPProxy sftpProxy = null;
                        string localSSHPath = string.Empty;
                        try
                        {
                            //获取对应mac机的信息
                            MonitorServer ms = msList.Find(x => x.monitorServerIP.Trim().Equals(spi.MonitorServerIP.Trim()) && spi.MacPath.ToLower().Trim().Contains(x.monitorMacPath.ToLower().Trim()));
                            if (ms != null)
                            {
                                sftpProxy = new SFTPProxy(ms.monitorServerIP, ms.account, ms.password);
                                if (!sftpProxy.IsExist(spi.MacPath))
                                {
                                    delIDList.Add(spi.ID);
                                    #region 删除SSH输出路径的本地文件
                                    //获取本地路径
                                    localSSHPath = GetLocalSSHPath(ms, spi.MacPath, config.Path.OutputPath);
                                    if (spi.typeflag == 0)
                                    {
                                        fsu.DeleteDir(localSSHPath);
                                    }
                                    else
                                    {
                                        //如果是文件
                                        fsu.DeleteFile(localSSHPath);
                                        //判断目录是否为空：是否有文件
                                        CleanLocalSSHDirectory(fsu, localSSHPath);
                                    }
                                    #endregion
                                }
                            }

                        }
                        catch (System.Exception ex)
                        {
                            Common.Util.LogManager.WriteLog(Common.Util.LogFile.DBSync, localSSHPath + Environment.NewLine + MessageUtil.GetExceptionMsg(ex, ""));
                        }
                        finally
                        {
                            if (sftpProxy != null)
                            {
                                sftpProxy.Close();
                            }
                        }
                        #endregion

                    }
                }
                catch (System.Exception ex)
                {
                    Common.Util.LogManager.WriteLog(Common.Util.LogFile.DBSync, MessageUtil.GetExceptionMsg(ex, ""));
                }
                finally
                {

                    spid.Dispose();
                }
                #endregion

                #region 更新数据库
                spid.DeleteEntrys(config, delIDList);
                #endregion
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.DBSync, MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        /// <summary>
        /// 判断ssh备份路径是否空，如果空了，则删除路径
        /// </summary>
        private void CleanLocalSSHDirectory(FileSystemUtil fsu, string localSSHPath)
        {
            string emptyDirP = fsu.GetTheTopEmptyDirectoryPath(localSSHPath);
            if (!string.IsNullOrWhiteSpace(emptyDirP))
            {
                fsu.DeleteDir(emptyDirP);
            }
        }

        /// <summary>
        /// 获取文件对应的SSH本地文件路径
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="macPath"></param>
        /// <returns></returns>
        private string GetLocalSSHPath(MonitorServer ms, string macPath, string sshOP)
        {
            string localBasePath = ms.monitorLocalPath;

            //获取SSH输出基础路径 暂时不支持网络位置
            string localSSHBasePath = string.Empty;
            if (string.IsNullOrWhiteSpace(sshOP.Trim()))
            {
                localSSHBasePath = localBasePath.Substring(0, localBasePath.IndexOf(':') + 1) + "\\SSH\\" + ms.monitorServerName + "-SSH";
            }
            else
            {
                sshOP.TrimEnd('\\');
                localSSHBasePath = sshOP + "\\" + ms.monitorServerName + "-SSH";
            }
            string localSSHPath = macPath.Replace(ms.monitorMacPath, localSSHBasePath).Replace('/', '\\');
            localSSHPath = PathValidUtil.GetValidPath(localSSHPath);
            return localSSHPath;
        }

        #endregion

        /// <summary>
        /// 将SSH方式copy的文件复制到对应的Robocopy目的路径
        /// </summary>
        public void LocalSync(Config config)
        {
            #region 判断robocopy是否结束（ps:由于RoboCopy有时不能正常结束，暂时通过时间配置
            if (TimeCheckHelper.CheckTime("SSHLocalSyncTime", config.SSHLocalSyncTime))
            {

                Common.Util.LogManager.WriteLog(Common.Util.LogFile.LocalSync, "start");
                _localSync(config);
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.LocalSync, "end");

                //执行了一个任务
                Signal.CompletedTaskCount++;
            }
            #endregion
        }

        private void _localSync(Config config)
        {
            #region 同步到本地目的目录
            try
            {
                //获取所有MonitorServer，以便于数据处理
                MonitorServerDAL msd = new MonitorServerDAL();
                List<MonitorServer> msList = msd.GetAllMonitorServer(config);

                #region 获取SSH输出路径中的第一层子目录 (PS:此情况为：SSH输出路径 必须配置)
                //获取SSH输出基础路径 暂时不支持网络位置
                DirectoryInfo sshDI = new DirectoryInfo(config.Path.OutputPath);
                if (sshDI.Exists)
                {
                    //获取MonitorServer相对的SSH根目录
                    foreach (DirectoryInfo di in sshDI.EnumerateDirectories())
                    {
                        if (Signal.IsSystemStoping) break;

                        //获取对应的MonitorServer
                        int flagIndex = di.Name.LastIndexOf("-SSH");
                        if (flagIndex <= 0) continue;

                        Common.Util.LogManager.WriteLog(Common.Util.LogFile.LocalSync, di.FullName);

                        string msName = di.Name.Substring(0, flagIndex);
                        MonitorServer ms = msList.Find(x => x.monitorServerName.Equals(msName));

                        //相对于本地设定的基础路径
                        DirectoryInfo roboBasePathDI = new DirectoryInfo(ms.monitorLocalPath);
                        if (roboBasePathDI.Exists)
                        {
                            //判断文件在Robocopy目标路径中是否存在
                            foreach (FileInfo fi in di.EnumerateFiles("*", SearchOption.AllDirectories))
                            {
                                if (Signal.IsSystemStoping) break;
                                //获取Robocopy目标路径
                                string destPath = fi.FullName.Replace(di.FullName, ms.monitorLocalPath);
                                FileInfo targetFile = new FileInfo(destPath);
                                //如果已经被删除，则复制
                                if (!targetFile.Exists)
                                {
                                    if (!Directory.Exists(targetFile.DirectoryName))
                                    {
                                        Directory.CreateDirectory(targetFile.DirectoryName);
                                    }

                                    File.Copy(fi.FullName, targetFile.FullName);
                                }
                            }
                        }
                    }
                }
                #endregion

                //探测 Robocopy目的路径中的SSH copy文件是否存在
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.LocalSync, MessageUtil.GetExceptionMsg(ex, ""));

            }
            #endregion
        }
    }
}
