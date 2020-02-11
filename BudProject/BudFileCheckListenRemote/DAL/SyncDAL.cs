using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BudFileCheckListen.Common;
using log4net;
using System.Reflection;
using BudFileCheckListen.Common.Util;

namespace BudFileCheckListenRemote.DAL
{
    /// <summary>
    /// 用于同步
    /// </summary>
    public class SyncDAL
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 删除昨天的路径备份并初始化
        /// </summary>
        public void TruncatePathBk()
        {
            string sql = "truncate table pathBK ";
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

        /// <summary>
        /// 删除monitorfilelisten 表并初始化
        /// </summary>
        public void TruncateFileListen()
        {
            string sql = "truncate table monitorFileListen ";
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
        public void SavePath(string sql)
        {
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

        /// <summary>
        /// 删除pathBk 表中对应项
        /// </summary>
        /// <param name="msid"></param>
        /// <param name="path"></param>
        public void DeletePath(string msid, string relativePath)
        {
            string sql = "delete from pathBk where msid=" + msid + " and relativePath=N'" + relativePath.Replace("'", "''") + "'";
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
 