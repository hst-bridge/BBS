using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using log4net;
using System.Reflection;
using BudSSH.Common.Util;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System.IO;

namespace BudSSH.BLL
{
    /// <summary>
    /// SFTP proxy xiecongwen 20140716
    /// </summary>
    class SFTPProxy
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private SftpClient sftp = null;
        
        private string ipAddress = string.Empty;
        private string loginUser = string.Empty;
        private string loginPass = string.Empty;

        public SFTPProxy(string ipAddress, string loginUser, string loginPass)
        {
            this.ipAddress = ipAddress;
            this.loginUser = loginUser;
            this.loginPass = loginPass;
        }

        private SftpClient Sftp
        {
            get {
                if (sftp == null)
                {
                    sftp = GetAvailableSFTP();
                }
                return sftp;
            }
        }

        //获取一个可用的SFTPClient
        private SftpClient GetAvailableSFTP()
        {
            //关闭之前的
            //Close();
            try
            {
                var connectionInfo = new KeyboardInteractiveConnectionInfo(ipAddress, 22, loginUser);
                connectionInfo.AuthenticationPrompt += delegate(object sender, AuthenticationPromptEventArgs e)
                {
                    foreach (var prompt in e.Prompts)
                    {
                        if (prompt.Request.Equals("Password:", StringComparison.InvariantCultureIgnoreCase))
                        {
                            prompt.Response = loginPass;
                        }
                    }
                };

                var sftpClient = new SftpClient(connectionInfo);
                sftpClient.Connect();

                return sftpClient;
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }
            return null;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public SftpFile GetFileInfo(string absolutePath)
        {
            SftpFile sf = Sftp.Get(absolutePath);
            
            return sf;
        }

        public bool IsExist(string absolutePath)
        {
            bool IsExist = false;
            try
            {
                SftpFileAttributes fileAttr = Sftp.GetAttributes(absolutePath);
                IsExist = true;
            }
            catch
            {
                IsExist = false;
            }

            return IsExist;
        }

        /// <summary>
        /// List the contents of the given remote directory. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SftpFile[] TopFileLs(String path)
        {
           List<SftpFile> sfList = new List<SftpFile>();
           IEnumerable<SftpFile> sftpFiles = Sftp.ListDirectory(path);
           if (sftpFiles != null && sftpFiles.Count() > 0)
           {
               foreach (SftpFile sf in sftpFiles)
               {
                   if (sf.Name.Equals(".") || sf.Name.Equals("..")) continue;
                   if (sf.IsDirectory) continue;
                   else sfList.Add(sf);
               }

               return sfList.ToArray();
           }

           return null;
        }

        public SftpFile[] TopDirLs(String path)
        {
            List<SftpFile> sfList = new List<SftpFile>();
            IEnumerable<SftpFile> sftpFiles = Sftp.ListDirectory(path);
            if (sftpFiles != null && sftpFiles.Count() > 0)
            {
                foreach (SftpFile sf in sftpFiles)
                {
                    if (sf.Name.Equals(".") || sf.Name.Equals("..")) continue;
                    if (sf.IsDirectory) sfList.Add(sf);
                }

                return sfList.ToArray();
            }

            return null;
        }

        public void GetFile(System.String remote, System.String local, bool resume)
        {
            Sftp.DownloadFile(remote, File.OpenWrite(local));
            
        }

        /// <summary>
        /// 遍历路径 规则 一层一层的获取 先文件后子目录
        /// 
        /// 由于递归不便处理异常，特修改为循环遍历树形结构，以实现递归效果 xiecongwen 20140716
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable Ls(SftpFile file)
        {
            var sshlog = new Common.Util.SSHLogManager();
            NTree<SftpFile> parent = new NTree<SftpFile>(file);
            //后序遍历树
            NTree<SftpFile> child = null;
            #region 自下而上 后序遍历

            while (true)
            {
                string path = parent.Data.FullName;
                #region 判断是否有子节点 ps:将子目录添加到子节点，返回子文件

                SftpFile[] dirs = null;
                SftpFile[] files = null;
                #region 获取
                try
                {
                    #region 获取子目录，并添加到子节点
                    dirs =TopDirLs(path);
                    if (dirs != null && dirs.Count() > 0)
                    {
                        foreach (var dir in dirs) parent.AddChild(dir);
                    }
                    #endregion

                    #region 获取子文件
                    files = TopFileLs(path);
                    #endregion
                }
                catch (System.Exception ex)
                {
                    parent.RemoveAllChildren();
                    //记录报错路径，并忽略其子目录
                    sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = path });
                    logger.Error("FTPPath:"+path+Environment.NewLine+ MessageUtil.GetExceptionMsg(ex, ""));

                    //更新sftp连接对象
                    sftp = GetAvailableSFTP();
                }
                #endregion

                //返回子文件
                if (files != null && files.Count() > 0)
                {
                    foreach (var f in files) yield return f;
                }
                #endregion

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

            #endregion

        }

        public void Close()
        {
            if (sftp != null && sftp.IsConnected)
            {
                sftp.Disconnect();
                sftp.Dispose();
                sftp = null;
            }
        }
    }
}
