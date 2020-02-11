using BudFileCheckListen.Common;
using BudFileCheckListen.Common.Util;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BudFileCheckListenRemote.DAL
{
    /// <summary>
    /// log table DAL
    /// </summary>
    class LogDAL
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 删除一个月前的log
        /// </summary>
        /// <param name="datetime">格式 2015-01-01 00:00:00.000</param>
        public void ClearOverdueLog(string datetime)
        {
            string sql = "delete FROM [log] where backupStartTime < '" + datetime + "'";
            //执行
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 300;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                }
            }
        }
    }
}
