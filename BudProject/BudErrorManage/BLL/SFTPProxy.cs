using System;
using System.Collections.Generic;
using System.Linq;
using Maverick.SFTP;
using Maverick.SSH;
using System.Collections;
using log4net;
using System.Reflection;

namespace BudErrorManage.BLL
{
    /// <summary>
    /// SFTP proxy xiecongwen 20140716
    /// </summary>
    class SFTPProxy
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private SFTPClient sftp = null;
        private SSHClient ssh = null;
        private string ipAddress = string.Empty;
        private string loginUser = string.Empty;
        private string loginPass = string.Empty;

        public SFTPProxy(string ipAddress, string loginUser, string loginPass)
        {
            this.ipAddress = ipAddress;
            this.loginUser = loginUser;
            this.loginPass = loginPass;
        }

        private SFTPClient Sftp
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
        private SFTPClient GetAvailableSFTP()
        {
            //关闭之前的
            Close();

            SSHConnector con = SSHConnector.Create();
            ssh = con.Connect(new TcpClientTransport(ipAddress, 22), loginUser, true);

            PasswordAuthentication pwd = new PasswordAuthentication();
            pwd.Password = loginPass;

            // Authenticate the user
            if (ssh.Authenticate(pwd) == AuthenticationResult.COMPLETE)
            {
                // Open up a session and do something..
                sftp = new SFTPClient(ssh);
            }
            return sftp;
        }

        /// <summary>
        /// List the contents of the given remote directory. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Maverick.SFTP.SFTPStatusException"/>
        /// <exception cref="Maverick.SSH.SSHException"/>
        public SFTPFile[] TopFileLs(String path)
        {
           return Sftp.TopFileLs(path);
        }

        public SFTPFile[] TopDirLs(String path)
        {
            return Sftp.TopDirLs(path);
        }

        public SFTPFileAttributes GetFile(System.String remote, System.String local, bool resume)
        {
            return Sftp.GetFile(remote, local, resume);
        }

        /// <summary>
        /// 遍历路径 规则 一层一层的获取 先文件后子目录
        /// 
        /// 由于递归不便处理异常，特修改为循环遍历树形结构，以实现递归效果 xiecongwen 20140716
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable Ls(SFTPFile file)
        {
            var sshlog = new Common.Util.SSHLogManager();
            NTree<SFTPFile> parent = new NTree<SFTPFile>(file);
            //后序遍历树
            NTree<SFTPFile> child = null;
            #region 自下而上 后序遍历

            while (true)
            {
                string path = parent.Data.AbsolutePath;
                #region 判断是否有子节点 ps:将子目录添加到子节点，返回子文件

                SFTPFile[] dirs = null;
                SFTPFile[] files = null;
                #region 获取
                try
                {
                    #region 获取子目录，并添加到子节点
                    dirs = sftp.TopDirLs(path);
                    if (dirs != null && dirs.Count() > 0)
                    {
                        foreach (var dir in dirs) parent.AddChild(dir);
                    }
                    #endregion

                    #region 获取子文件
                    files = sftp.TopFileLs(path);
                    #endregion
                }
                catch (Exception ex)
                {
                    parent.RemoveAllChildren();
                    //记录报错路径，并忽略其子目录
                    sshlog.WriteLog(new Common.Util.SSHLog() { DateTime = DateTime.Now, LogType = Common.Util.SSHLogType.Failure, Message = path });
                    logger.Error(ex.Message);

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
            if (sftp != null && !sftp.IsClosed)
            {
                sftp.Quit();
            }
            if (ssh != null && ssh.IsConnected)
            {
                ssh.Disconnect();
            }
        }
    }
}
