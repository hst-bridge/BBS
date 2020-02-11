using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using BudFileCheckListen.Common;

namespace BudFileCheckListen.DBService
{
    /// <summary>
    /// 用于为FileListener提供服务
    /// </summary>
    class FileListenService
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 判断monitorFileListen表中对应monitorServerID是否有数据
        /// </summary>
        /// <returns></returns>
        public bool IsDBCleared(int monitorServerID)
        {
            string sql = "select top 1 id from [monitorFileListen] where monitorServerID="+monitorServerID;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                   object obj = cmd.ExecuteScalar();
                   if (Convert.ToInt32(obj) > 0) return false;
                   else return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    return false;
                }
            }
        }

    }
}
