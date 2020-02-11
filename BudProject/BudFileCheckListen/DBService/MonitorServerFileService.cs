using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BudFileCheckListen.Entities;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using BudFileCheckListen.Common;
using Common;

namespace BudFileCheckListen.DBService
{
    public class MonitorServerFileService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MonitorServerFileService()
        {
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public int Insert(monitorServerFile monitorserverfile)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Format(@"INSERT INTO [dbo].[monitorServerFile] ([monitorServerID]
           ,[monitorFileName]
           ,[monitorFileDirectory]
           ,[monitorFilePath]
           ,[monitorFileType]
           ,[monitorFileSize]
           ,[monitorStartTime]
           ,[monitorEndTime]
           ,[monitorFileStatus]
           ,[transferFlg]
           ,[transferNum]
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
                       ,N'{2}'
                       ,N'{3}'
                       ,'{4}'
                       ,'{5}'                      
                       ,'{6}'
                       ,'{7}'                      
                       ,'{8}'
                       ,'{9}'
                       ,'{10}'
                       ,'{11}'
                       ,'{12}'
                       ,'{13}'                      
                       ,'{14}'
                       ,'{15}'                      
                       ,'{16}'
                       ,'{17}'
                       ,'{18}'
                       ,'{19}',0)",
           monitorserverfile.monitorServerID
           , monitorserverfile.monitorFileName
           , monitorserverfile.monitorFileDirectory
           , monitorserverfile.monitorFilePath
           , monitorserverfile.monitorFileType
           , monitorserverfile.monitorFileSize
           , CommonUtil.DateTimeToString(monitorserverfile.monitorStartTime)
           , CommonUtil.DateTimeToString( monitorserverfile.monitorEndTime)
           , monitorserverfile.monitorFileStatus
           , monitorserverfile.transferFlg
           , monitorserverfile.transferNum
           , monitorserverfile.deleteFlg
           , monitorserverfile.deleter
           , CommonUtil.DateTimeToString(monitorserverfile.deleteDate)
           , monitorserverfile.creater
           , CommonUtil.DateTimeToString(monitorserverfile.createDate)
           , monitorserverfile.updater
           , CommonUtil.DateTimeToString(monitorserverfile.updateDate)
           , monitorserverfile.restorer
           , CommonUtil.DateTimeToString(monitorserverfile.restoreDate));

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
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public int Edit(Entities.monitorServerFile monitorserverfile)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Empty;

                    SqlParameter[] paras = null;

                    sql = @"UPDATE [dbo].[monitorServerFile]
                     SET [monitorServerID] = @monitorServerID
                     ,[monitorFileName] = @monitorFileName
                     ,[monitorFileDirectory] = @monitorFileDirectory
                     ,[monitorFilePath] = @monitorFilePath
                     ,[monitorFileType] = @monitorFileType
                     ,[monitorFileSize] = @monitorFileSize
                     ,[monitorStartTime] = @monitorStartTime
                     ,[monitorEndTime] = @monitorEndTime
                     ,[monitorFileStatus] = @monitorFileStatus
                     ,[transferFlg] = @transferFlg
                     ,[transferNum] = @transferNum
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
                            new SqlParameter("@monitorServerID",monitorserverfile.monitorServerID),
                            new SqlParameter("@monitorFileName",monitorserverfile.monitorFileName),
                            new SqlParameter("@monitorFileDirectory",monitorserverfile.monitorFileDirectory),
                            new SqlParameter("@monitorFilePath",monitorserverfile.monitorFilePath),
                            new SqlParameter("@monitorFileType",monitorserverfile.monitorFileType),
                            new SqlParameter("@monitorFileSize",monitorserverfile.monitorFileSize),
                            new SqlParameter("@monitorStartTime",CommonUtil.DateTimeToString(monitorserverfile.monitorStartTime)),
                            new SqlParameter("@monitorEndTime",CommonUtil.DateTimeToString(monitorserverfile.monitorEndTime)),
                            new SqlParameter("@monitorFileStatus",monitorserverfile.monitorFileStatus),
                            new SqlParameter("@transferFlg",monitorserverfile.transferFlg),
                            new SqlParameter("@transferNum",monitorserverfile.transferNum),
                            new SqlParameter("@deleteFlg",monitorserverfile.deleteFlg),
                            new SqlParameter("@deleter",monitorserverfile.deleter),
                            new SqlParameter("@deleteDate",CommonUtil.DateTimeToString(monitorserverfile.deleteDate)),
                            new SqlParameter("@creater",monitorserverfile.creater),
                            new SqlParameter("@createDate",CommonUtil.DateTimeToString(monitorserverfile.createDate)),
                            new SqlParameter("@updater",monitorserverfile.updater),
                            new SqlParameter("@updateDate",CommonUtil.DateTimeToString(monitorserverfile.updateDate)),
                            new SqlParameter("@restorer",monitorserverfile.updater),
                            new SqlParameter("@restoreDate",CommonUtil.DateTimeToString(monitorserverfile.updateDate)),
                            new SqlParameter("@id",monitorserverfile.id)};


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
        /// <param name="cust_no"></param>
        public int Del(int id)
        {
            try
            {
                string sql = @"delete from monitorServerFile
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
