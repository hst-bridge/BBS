using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;
using BudSSH.Model.Behind;
using System.Text.RegularExpressions;
using BudSSH.DAL.SQLServer;
using BudSSH.Common.Util;
using Renci.SshNet.Sftp;

namespace BudSSH.BLL
{
    /// <summary>
    /// 用于过滤已经复制过且源端未发生改变的报错路径
    /// </summary>
    class ErrorPathFilter
    {

        private static string[] UnnecessaryList =  { 
           ".DS_Store",".com.apple.timemachine.supported","Icon"
        };
        /// <summary>
        /// 过滤已经复制过且源端未发生改变的报错路径
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static ErrorPathInfo PreFilter(ErrorPathInfo el)
        {
            if (el != null && el.PathList != null && el.PathList.Count > 0)
            {
                //过滤无关紧要的文件
                el.PathList = FilterUnnecessary(el.PathList);
            }
            return el;
        }

        /// <summary>
        /// 判断此路径是否发生变化
        /// 数据库中路径信息 暂定：有效性为一个月 即 每个月全部清空一次 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsUpdate(Model.Config config, SftpFile sf, MonitorServer ms)
        {
            //判断是否更新
            bool isUpdate = false;
            try
            {
                SSHPathInfo spi = new SSHPathInfo()
                {
                    MonitorServerIP = ms.monitorServerIP,
                    MacPath = sf.FullName,
                    LastName = sf.FullName.Substring(sf.FullName.LastIndexOf('/') + 1),
                    depth = GetDepth(sf.FullName),
                    typeflag = sf.IsDirectory ? 0 : 1,
                    updateTime = (Int32)(sf.Attributes.LastWriteTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                };

                SSHPathInfoDAL spidal = new SSHPathInfoDAL();
                isUpdate = spidal.IsUpdate(config,spi);
            }
            catch (System.Exception ex)
            {
                isUpdate = true;
                BudSSH.Common.Util.LogManager.WriteLog(Common.Util.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
            }

            return isUpdate;
        }

        /// <summary>
        /// 获取路径深度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static int GetDepth(string path)
        {
            string ds = "/";
            return Regex.Matches(path, ds).Count;
        }

        /// <summary>
        /// 过滤不必要文件
        /// </summary>
        /// <returns></returns>
        private static List<ErrorEntry> FilterUnnecessary(List<ErrorEntry> list)
        {
            list.RemoveAll((x) => UnnecessaryList.Contains(x.Path.Substring(x.Path.LastIndexOf(@"\") + 1)));
            return list;
        }

    }
}
