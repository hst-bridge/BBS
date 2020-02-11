using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BudFileCheckListen.Entities;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using BudFileCheckListen.Common;
using System.Text;
using Common;
using BudFileCheckListen.Common.Util;

namespace BudFileCheckListen.DBService
{
    public class LogService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LogService()
        {
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="log"></param>
        public int Insert(log log)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Format(@"INSERT INTO [dbo].[log]
           ([monitorServerID]
           ,[monitorFileName]
           ,[monitorFileStatus]
           ,[monitorFilePath]
           ,[monitorFileType]
           ,[monitorFileSize]
           ,[monitorTime]
           ,[transferFlg]
           ,[backupServerGroupID]
           ,[backupServerID]
           ,[backupServerFileName]
           ,[backupServerFilePath]
           ,[backupServerFileType]
           ,[backupServerFileSize]
           ,[backupStartTime]
           ,[backupEndTime]
           ,[backupTime]
           ,[backupFlg]
           ,[copyStartTime]
           ,[copyEndTime]
           ,[copyTime]
           ,[copyFlg]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate]
           ,[restorer]
           ,[restoreDate]
,[synchronismFlg])
           VALUES
                       ('{0}'
                       ,N'{1}'
                       ,'{2}'
                       ,N'{3}'
                       ,'{4}'
                       ,'{5}'                      
                       ,'{6}'
                       ,'{7}'                      
                       ,'{8}'
                       ,'{9}'
                       ,N'{10}'
                       ,N'{11}'
                       ,'{12}'
                       ,'{13}'                      
                       ,'{14}'
                       ,'{15}'                      
                       ,'{16}'
                       ,'{17}'
                       ,'{18}'
                       ,'{19}'
                       ,'{20}'
                       ,'{21}'
                       ,'{22}'
                       ,'{23}'
                       ,'{24}'
                       ,'{25}'
                       ,'{26}'                      
                       ,'{27}'
                       ,'{28}'                      
                       ,'{29}'
                       ,'{30}',0)",
           log.monitorServerID
           , log.monitorFileName
           , log.monitorFileStatus
           , log.monitorFilePath
           , log.monitorFileType
           , log.monitorFileSize
           , CommonUtil.DateTimeToString(log.monitorTime)
           , log.transferFlg
           , log.backupServerGroupID
           , log.backupServerID
           , log.backupServerFileName
           , log.backupServerFilePath
           , log.backupServerFileType
           , log.backupServerFileSize
           , CommonUtil.DateTimeToString(log.backupStartTime)
           , CommonUtil.DateTimeToString(log.backupEndTime)
           , log.backupTime
           , log.backupFlg
           , CommonUtil.DateTimeToString(log.copyStartTime)
           , CommonUtil.DateTimeToString(log.copyEndTime)
           , log.copyTime
           , log.copyFlg
           , log.deleteFlg
           , log.deleter
           , CommonUtil.DateTimeToString(log.deleteDate)
           , log.creater
           , CommonUtil.DateTimeToString(log.createDate)
           , log.updater
           , CommonUtil.DateTimeToString(log.updateDate)
           , log.restorer
           , CommonUtil.DateTimeToString(log.restoreDate));

                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    int iret = cmd.ExecuteNonQuery();

                    if (iret > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    trans.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 用于批量插入Log xiecongwen
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public void Insert(List<log> logs)
        {
            //遍历logs 拼接sql
            StringBuilder sb = new StringBuilder();
            logs.ForEach((x) => sb.Append(GetSql(x)));

            //执行
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                cmd.CommandTimeout = 300;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    logger.Error(MessageUtil.GetExceptionMsg(ex,""));
                }
            }
        }

        /// <summary>
        /// 将log对象转换成sql字符串 xiecongwen 20140705
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private string GetSql(log log)
        {
            try
            {

             return string.Format(@"INSERT INTO [dbo].[log]
           ([monitorServerID]
           ,[monitorFileName]
           ,[monitorFileStatus]
           ,[monitorFilePath]
           ,[monitorFileType]
           ,[monitorFileSize]
           ,[monitorTime]
           ,[transferFlg]
           ,[backupServerGroupID]
           ,[backupServerID]
           ,[backupServerFileName]
           ,[backupServerFilePath]
           ,[backupServerFileType]
           ,[backupServerFileSize]
           ,[backupStartTime]
           ,[backupEndTime]
           ,[backupTime]
           ,[backupFlg]
           ,[copyStartTime]
           ,[copyEndTime]
           ,[copyTime]
           ,[copyFlg]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate]
           ,[restorer]
           ,[restoreDate]
,[synchronismFlg])
           VALUES
                       ('{0}'
                       ,N'{1}'
                       ,'{2}'
                       ,N'{3}'
                       ,'{4}'
                       ,'{5}'                      
                       ,'{6}'
                       ,'{7}'                      
                       ,'{8}'
                       ,'{9}'
                       ,N'{10}'
                       ,N'{11}'
                       ,'{12}'
                       ,'{13}'                      
                       ,'{14}'
                       ,'{15}'                      
                       ,'{16}'
                       ,'{17}'
                       ,'{18}'
                       ,'{19}'
                       ,'{20}'
                       ,'{21}'
                       ,'{22}'
                       ,'{23}'
                       ,'{24}'
                       ,'{25}'
                       ,'{26}'                      
                       ,'{27}'
                       ,'{28}'                      
                       ,'{29}'
                       ,'{30}',0);",
            log.monitorServerID
            , log.monitorFileName.Replace("'","''")
            , log.monitorFileStatus
            , log.monitorFilePath.Replace("'", "''")
            , log.monitorFileType
            , log.monitorFileSize
            , CommonUtil.DateTimeToString(log.monitorTime)
            , log.transferFlg
            , log.backupServerGroupID
            , log.backupServerID
            , log.backupServerFileName.Replace("'","''")
            , log.backupServerFilePath
            , log.backupServerFileType
            , log.backupServerFileSize
            , CommonUtil.DateTimeToString(log.backupStartTime)
            , CommonUtil.DateTimeToString(log.backupEndTime)
            , log.backupTime
            , log.backupFlg
            , CommonUtil.DateTimeToString(log.copyStartTime)
            , CommonUtil.DateTimeToString(log.copyEndTime)
            , log.copyTime
            , log.copyFlg
            , log.deleteFlg
            , log.deleter
            , CommonUtil.DateTimeToString(log.deleteDate)
            , log.creater
            , CommonUtil.DateTimeToString(log.createDate)
            , log.updater
            , CommonUtil.DateTimeToString(log.updateDate)
            , log.restorer
            , CommonUtil.DateTimeToString(log.restoreDate));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="log"></param>
        public int Edit(log log)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Empty;

                    SqlParameter[] paras = null;

                    sql = @"UPDATE [dbo].[log]
       SET [monitorServerID] = @monitorServerID
      ,[monitorFileName] = @monitorFileName
      ,[monitorFileStatus] = @monitorFileStatus
      ,[monitorFilePath] = @monitorFilePath
      ,[monitorFileType] = @monitorFileType
      ,[monitorFileSize] = @monitorFileSize
      ,[monitorTime] = @monitorTime
      ,[transferFlg] = @transferFlg
      ,[backupServerGroupID] = @backupServerGroupID
      ,[backupServerID] = @backupServerID
      ,[backupServerFileName] = @backupServerFileName
      ,[backupServerFilePath] = @backupServerFilePath
      ,[backupServerFileType] = @backupServerFileType
      ,[backupServerFileSize] = @backupServerFileSize
      ,[backupStartTime] = @backupStartTime
      ,[backupEndTime] = @backupEndTime
      ,[backupTime] = @backupTime
      ,[backupFlg] = @backupFlg
      ,[copyStartTime] = @copyStartTime
      ,[copyEndTime] = @copyEndTime
      ,[copyTime] = @copyTime
      ,[copyFlg] = @copyFlg
      ,[deleteFlg] = @deleteFlg
      ,[deleter] = @deleter
      ,[deleteDate] = @deleteDate
      ,[creater] = @creater
      ,[createDate] = @createDate
      ,[updater] = @updater
      ,[updateDate] = @updateDate
      ,[restorer] = @restorer
      ,[restoreDate] = @restoreDate
      WHERE id = @id";

                    paras = new SqlParameter[] {
                            new SqlParameter("@monitorServerID",log.monitorServerID),
                            new SqlParameter("@monitorFileName",log.monitorFileName),
                            new SqlParameter("@monitorFileStatus",log.monitorFileStatus),
                            new SqlParameter("@monitorFilePath",log.monitorFilePath),
                            new SqlParameter("@monitorFileType",log.monitorFileType),
                            new SqlParameter("@monitorFileSize",log.monitorFileSize),
                            new SqlParameter("@monitorTime",CommonUtil.DateTimeToString(log.monitorTime)),
                            new SqlParameter("@transferFlg",log.transferFlg),
                            new SqlParameter("@backupServerGroupID",log.backupServerGroupID),
                            new SqlParameter("@backupServerID",log.backupServerID),
                            new SqlParameter("@backupServerFileName",log.backupServerFileName),
                            new SqlParameter("@backupServerFilePath",log.backupServerFilePath),
                            new SqlParameter("@backupServerFileType",log.backupServerFileType),
                            new SqlParameter("@backupServerFileSize",log.backupServerFileSize),
                            new SqlParameter("@backupStartTime",CommonUtil.DateTimeToString(log.backupStartTime)),
                            new SqlParameter("@backupEndTime",CommonUtil.DateTimeToString(log.backupEndTime)),
                            new SqlParameter("@backupTime",log.backupTime),
                            new SqlParameter("@backupFlg",log.backupFlg),
                            new SqlParameter("@copyStartTime",CommonUtil.DateTimeToString(log.copyStartTime)),
                            new SqlParameter("@copyEndTime",CommonUtil.DateTimeToString(log.copyEndTime)),
                            new SqlParameter("@copyTime",log.copyTime),
                            new SqlParameter("@copyFlg",log.copyFlg),
                            new SqlParameter("@deleteFlg",log.deleteFlg),
                            new SqlParameter("@deleter",log.deleter),
                            new SqlParameter("@deleteDate",CommonUtil.DateTimeToString(log.deleteDate)),
                            new SqlParameter("@creater",log.creater),
                            new SqlParameter("@createDate",CommonUtil.DateTimeToString(log.createDate)),
                            new SqlParameter("@updater",log.updater),
                            new SqlParameter("@updateDate",CommonUtil.DateTimeToString(log.updateDate)),
                            new SqlParameter("@restorer",log.updater),
                            new SqlParameter("@restoreDate",CommonUtil.DateTimeToString(log.updateDate)),
                            new SqlParameter("@id",log.id)};


                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddRange(paras);
                    int iret = cmd.ExecuteNonQuery();

                    if (iret > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    trans.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        public int Del(int id)
        {
            try
            {
                string sql = @"delete from log
                     where id = @id
                    ";
                SqlParameter para = new SqlParameter("@id", id);

                int iret = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString, CommandType.Text, sql, para);
                return iret;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return 0;
            }
        }
    }
}
