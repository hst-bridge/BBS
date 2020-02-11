using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BudFileCheckListen.Entities;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using BudFileCheckListen.Common;
using System.Collections;
using BudFileCheckListen.Models;
using System.Text;
using Common;
using BudFileCheckListen.Common.Util;

namespace BudFileCheckListen.DBService
{
    public class MonitorFileListenService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MonitorFileListenService()
        {

        }

        /// <summary>
        /// 監視対象
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<string> GetByMonitorAllInfoForDel(int monitorServerID, int deleteFlg)
        {
            List<string> monitorAllInfoList = new List<string>();
            try
            {
                string sql = @"select monitorLocalPath,monitorFileRelativeFullPath from monitorFileListen where monitorServerID = @monitorServerID and deleteFlg = @deleteFlg ";
                SqlParameter[] commandParameters = new SqlParameter[] {
                    new SqlParameter("@monitorServerID", monitorServerID),
                    new SqlParameter("@deleteFlg", deleteFlg)
                };
                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, commandParameters))
                {
                    while (reader.Read())
                    {
                        monitorAllInfoList.Add(reader["monitorLocalPath"].ToString() + reader["monitorFileRelativeFullPath"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
            return monitorAllInfoList;
        }

        /// <summary>
        /// 監視対象
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public Hashtable GetByMonitorAllInfo(int monitorServerID, int deleteFlg)
        {
            Hashtable monitorAllInfoList = new Hashtable();
            try
            {
                string sql = @"select * from monitorFileListen where monitorServerID = @monitorServerID and deleteFlg = @deleteFlg ";
                SqlParameter[] commandParameters = new SqlParameter[] {
                    new SqlParameter("@monitorServerID", monitorServerID),
                    new SqlParameter("@deleteFlg", deleteFlg)
                };
                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, commandParameters))
                {
                    while (reader.Read())
                    {
                        CompareMonitorInfo monitorInfo  = new CompareMonitorInfo();
                        monitorInfo.IdInfo = int.Parse(reader["id"].ToString());
                        monitorInfo.FileUpdateTimeInfo = DateTime.Parse(reader["monitorFileLastWriteTime"].ToString());
                        monitorAllInfoList.Add(reader["monitorLocalPath"].ToString() + reader["monitorFileRelativeFullPath"].ToString(), monitorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
            return monitorAllInfoList;
        }


        /// <summary>
        /// 監視対象
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileLocalPath"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public monitorFileListen GetByMonitorObject(int id)
        {
            monitorFileListen _monitorFileListen = null;
            try
            {
                string sql = @"select * from monitorFileListen where id = @id";
                SqlParameter paraId = new SqlParameter("@id", id);
                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, paraId))
                {
                    while (reader.Read())
                    {
                        _monitorFileListen = new monitorFileListen();
                        _monitorFileListen.id = int.Parse(reader["id"].ToString());
                        _monitorFileListen.monitorServerID = int.Parse(reader["monitorServerID"].ToString());
                        _monitorFileListen.monitorFileName = reader["monitorFileName"].ToString();
                        _monitorFileListen.monitorType = reader["monitorType"].ToString();
                        _monitorFileListen.monitorServerIP = reader["monitorServerIP"].ToString();
                        _monitorFileListen.sharePoint = reader["sharePoint"].ToString();
                        _monitorFileListen.monitorLocalPath = reader["monitorLocalPath"].ToString();
                        _monitorFileListen.monitorFileRelativeDirectory = reader["monitorFileRelativeDirectory"].ToString();
                        _monitorFileListen.monitorFileRelativeFullPath = reader["monitorFileRelativeFullPath"].ToString();
                        _monitorFileListen.monitorFileLastWriteTime = DateTime.Parse(reader["monitorFileLastWriteTime"].ToString());
                        _monitorFileListen.monitorFileSize = reader["monitorFileSize"].ToString();
                        _monitorFileListen.monitorFileExtension = reader["monitorFileExtension"].ToString();
                        _monitorFileListen.monitorFileCreateTime = DateTime.Parse(reader["monitorFileCreateTime"].ToString());
                        _monitorFileListen.monitorFileLastAccessTime = DateTime.Parse(reader["monitorFileLastAccessTime"].ToString());
                        _monitorFileListen.monitorStatus = reader["monitorStatus"].ToString();
                        _monitorFileListen.monitorFileStartTime = DateTime.Parse(reader["monitorFileStartTime"].ToString());
                        _monitorFileListen.monitorFileEndTime = DateTime.Parse(reader["monitorFileEndTime"].ToString());
                        _monitorFileListen.deleteFlg = short.Parse(reader["deleteFlg"].ToString());
                        _monitorFileListen.deleter = reader["deleter"].ToString();
                        _monitorFileListen.deleteDate = DateTime.Parse(reader["deleteDate"].ToString());
                        _monitorFileListen.creater = reader["creater"].ToString();
                        _monitorFileListen.createDate = DateTime.Parse(reader["createDate"].ToString());
                        _monitorFileListen.updater = reader["updater"].ToString();
                        _monitorFileListen.updateDate = DateTime.Parse(reader["updateDate"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
            }
            return _monitorFileListen;
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public int Insert(Entities.monitorFileListen mFL)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Format(@"INSERT INTO monitorFileListen
                       (monitorServerID
                       ,monitorFileName
                       ,monitorType
                       ,monitorServerIP
                       ,sharePoint
                       ,monitorLocalPath
                       ,monitorFileRelativeDirectory
                       ,monitorFileRelativeFullPath
                       ,monitorFileLastWriteTime
                       ,monitorFileSize
                       ,monitorFileExtension
                       ,monitorFileCreateTime
                       ,monitorFileLastAccessTime
                       ,monitorStatus
                       ,monitorFileStartTime
                       ,monitorFileEndTime
                       ,deleteFlg
                       ,deleter
                       ,deleteDate
                       ,creater
                       ,createDate
                       ,updater
                       ,updateDate
,[synchronismFlg]
                      ) VALUES
                       ('{0}'
                       ,N'{1}'
                       ,'{2}'
                       ,'{3}'
                       ,N'{4}'
                       ,N'{5}'                      
                       ,N'{6}'
                       ,N'{7}'                      
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
                       ,'{19}'
                       ,'{20}'
                       ,'{21}'
                       ,'{22}',0)",
                        mFL.monitorServerID
                        , mFL.monitorFileName
                        , mFL.monitorType
                        , mFL.monitorServerIP
                        , mFL.sharePoint
                        , mFL.monitorLocalPath
                        , mFL.monitorFileRelativeDirectory
                        , mFL.monitorFileRelativeFullPath
                        , CommonUtil.DateTimeToString(mFL.monitorFileLastWriteTime)
                        , mFL.monitorFileSize
                        , mFL.monitorFileExtension
                        , CommonUtil.DateTimeToString(mFL.monitorFileCreateTime)
                        , CommonUtil.DateTimeToString(mFL.monitorFileLastAccessTime)
                        , mFL.monitorStatus
                        , CommonUtil.DateTimeToString(mFL.monitorFileStartTime)
                        , CommonUtil.DateTimeToString(mFL.monitorFileEndTime)
                        , mFL.deleteFlg
                        , mFL.deleter
                        , CommonUtil.DateTimeToString(mFL.deleteDate)
                        , mFL.creater
                        , CommonUtil.DateTimeToString(mFL.createDate)
                        , mFL.updater
                        , CommonUtil.DateTimeToString(mFL.updateDate));

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
                    logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                    trans.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 用于批量插入monitorFileListen xiecongwen
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public void Insert(List<monitorFileListen> mfls, Action<monitorFileListen,StringBuilder> GetSql)
        {
            //遍历logs 拼接sql
            StringBuilder sb = new StringBuilder();
            mfls.ForEach((x) => GetSql(x,sb));

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
                    logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                    Common.LogManager.WriteLog(Common.LogFile.SQL, sb.ToString() + ex.Message);
                }
            }
        }

       
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public int Edit(Entities.monitorFileListen mFL)
        {
            SqlTransaction trans = null;
            using (SqlConnection conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                try
                {
                    string sql = string.Empty;

                    SqlParameter[] paras = null;

                    sql = @"UPDATE monitorFileListen
                                        SET 
                                            monitorServerID =@monitorServerID
                                            ,monitorFileName = @monitorFileName
                                            ,monitorType = @monitorType
                                            ,monitorServerIP = @monitorServerIP
                                            ,sharePoint = @sharePoint
                                            ,monitorLocalPath = @monitorLocalPath
                                            ,monitorFileRelativeDirectory = @monitorFileRelativeDirectory
                                            ,monitorFileRelativeFullPath = @monitorFileRelativeFullPath
                                            ,monitorFileLastWriteTime =@monitorFileLastWriteTime
                                            ,monitorFileSize = @monitorFileSize
                                            ,monitorFileExtension = @monitorFileExtension
                                            ,monitorFileCreateTime = @monitorFileCreateTime
                                            ,monitorFileLastAccessTime = @monitorFileLastAccessTime
                                            ,monitorStatus = @monitorStatus
                                            ,monitorFileStartTime = @monitorFileStartTime
                                            ,monitorFileEndTime = @monitorFileEndTime
                                            ,deleteFlg = @deleteFlg
                                            ,deleter =@deleter
                                            ,deleteDate =@deleteDate
                                            ,creater = @creater
                                            ,createDate = @createDate
                                            ,updater =@updater
                                            ,updateDate =@updateDate
                                        WHERE id = @id";

                    paras = new SqlParameter[] {
                            new SqlParameter("@monitorServerID",mFL.monitorServerID),
                            new SqlParameter("@monitorFileName",mFL.monitorFileName),
                            new SqlParameter("@monitorType",mFL.monitorType),
                            new SqlParameter("@monitorServerIP",mFL.monitorServerIP),
                            new SqlParameter("@sharePoint",mFL.sharePoint),
                            new SqlParameter("@monitorLocalPath",mFL.monitorLocalPath),
                            new SqlParameter("@monitorFileRelativeDirectory",mFL.monitorFileRelativeDirectory),
                            new SqlParameter("@monitorFileRelativeFullPath",mFL.monitorFileRelativeFullPath),
                            new SqlParameter("@monitorFileLastWriteTime",CommonUtil.DateTimeToString(mFL.monitorFileLastWriteTime)),
                            new SqlParameter("@monitorFileSize",mFL.monitorFileSize),
                            new SqlParameter("@monitorFileExtension",mFL.monitorFileExtension),
                            new SqlParameter("@monitorFileCreateTime",CommonUtil.DateTimeToString(mFL.monitorFileCreateTime)),
                            new SqlParameter("@monitorFileLastAccessTime",CommonUtil.DateTimeToString(mFL.monitorFileLastAccessTime)),
                            new SqlParameter("@monitorStatus",mFL.monitorStatus),
                            new SqlParameter("@monitorFileStartTime",CommonUtil.DateTimeToString(mFL.monitorFileStartTime)),
                            new SqlParameter("@monitorFileEndTime",CommonUtil.DateTimeToString(mFL.monitorFileEndTime)),
                            new SqlParameter("@deleteFlg",mFL.deleteFlg),
                            new SqlParameter("@deleter",mFL.deleter),
                            new SqlParameter("@deleteDate",CommonUtil.DateTimeToString(mFL.deleteDate)),
                            new SqlParameter("@creater",mFL.creater),
                            new SqlParameter("@createDate",CommonUtil.DateTimeToString(mFL.createDate)),
                            new SqlParameter("@updater",mFL.updater),
                            new SqlParameter("@updateDate",CommonUtil.DateTimeToString(mFL.updateDate)),
                            new SqlParameter("@id",mFL.id)};

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
                    logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
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
                string sql = @"delete from monitorFileListen
                     where id = @id
                    ";
                SqlParameter para = new SqlParameter("@id", id);

                int iret = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString, CommandType.Text, sql, para);
                return iret;
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex,""));
                return 0;
            }
        }

        /// <summary>
        /// 判断数据库中是否存在此路径记录
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="relativeFullPath"></param>
        /// <returns></returns>
        public bool IsPathExists(int monitorServerID, string relativeFullPath)
        {
            string sql = string.Empty;
            try
            {
                sql = "select 1 from [monitorFileListen] where deleteFlg=0 and monitorServerID=" + monitorServerID + " and monitorFileRelativeFullPath=N'" + relativeFullPath.Replace("'", "''") + "'";

                object obj = SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, CommandType.Text, sql);
                if (obj != null)
                {
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
                Common.LogManager.WriteLog(Common.LogFile.Error, "sql:" + sql);
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                throw;
            }
        }
    }
}
