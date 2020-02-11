using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DBUtility;
using System.Data.SqlClient;
using Common;
using System.Data;

namespace SQLServerDAL
{
    public class LogD// : ILog
    {
        public Database db = new Database();
        public int InsertLog(Log Log, SqlConnection conn)
        {
            try
            {
                return db.insert(Log, "log", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateLog(Log Log, SqlConnection conn)
        {
            string sql = @"UPDATE log
                               SET monitorServerID = @monitorserverid
                                    ,monitorFileName = @monitorfilename
                                    ,monitorFilePath = @monitorfilepath
                                    ,monitorFileType = @monitorfiletype
                                    ,monitorFileSize = @monitorfilesize
                                    ,monitorTime = @monitortime
                                    ,transferFlg = @transferflg
                                    ,backupServerGroupID = @backupservergroupid
                                    ,backupServerID = @backupserverid
                                    ,backupServerFileName = @backupserverfilename
                                    ,backupServerFilePath = @backupserverfilepath
                                    ,backupServerFileType = @backupserverfiletype
                                    ,backupServerFileSize = @backupserverfilesize
                                    ,backupStartTime = @backupstarttime
                                    ,backupEndTime = @backupendtime
                                    ,backupTime = @backuptime
                                    ,backupFlg = @backupflg
                                    ,copyStartTime = @copystarttime
                                    ,copyEndTime = @copyendtime
                                    ,copyTime = @copytime
                                    ,copyFlg = @copyflg
                                    ,updater = @updater
                                    ,updateDate = @updatedate
                             WHERE id=@id";
            SqlParameter[] spvalues = DBTool.GetSqlPm(Log);
            return SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql, spvalues);
        }

        public int DeleteLogById(int LogId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE log SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",LogId),
                new SqlParameter("@deleter",loginID),
                new SqlParameter("@deleteDate",DateTime.Now)
            };
            try
            {
                result = db.Udaquery(sql, conn, para);
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public IList<Log> GetLog(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<Log> lLog = new List<Log>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorTime
                              ,transferFlg
                              ,backupServerGroupID
                              ,backupServerID
                              ,backupServerFileName
                              ,backupServerFilePath
                              ,backupServerFileType
                              ,backupServerFileSize
                              ,backupStartTime
                              ,backupEndTime
                              ,backupTime
                              ,backupFlg
                              ,copyStartTime
                              ,copyEndTime
                              ,copyTime
                              ,copyFlg
                              ,creater
                              ,createDate
                          FROM [log]";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lLog = DBTool.GetListFromDatatable<Log>(dt);
            return lLog;
        }

        public int GetLogCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM log";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<Log> GetLogPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
        {
            IList<Log> lLog = new List<Log>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorTime
                              ,transferFlg
                              ,backupServerGroupID
                              ,backupServerID
                              ,backupServerFileName
                              ,backupServerFilePath
                              ,backupServerFileType
                              ,backupServerFileSize
                              ,backupStartTime
                              ,backupEndTime
                              ,backupTime
                              ,backupFlg
                              ,copyStartTime
                              ,copyEndTime
                              ,copyTime
                              ,copyFlg
                              ,creater
                              ,createDate
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM log ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lLog = DBTool.GetListFromDatatable<Log>(dt);
            return lLog;
        }
        public IList<Log> GetConditionLog(string where, SqlConnection conn)
        {
            IList<Log> lLog = new List<Log>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorTime
                              ,transferFlg
                              ,backupServerGroupID
                              ,backupServerID
                              ,backupServerFileName
                              ,backupServerFilePath
                              ,backupServerFileType
                              ,backupServerFileSize
                              ,backupStartTime
                              ,backupEndTime
                              ,backupTime
                              ,backupFlg
                              ,copyStartTime
                              ,copyEndTime
                              ,copyTime
                              ,copyFlg
                              ,creater
                              ,createDate
                          FROM [log]";
            if (where != "")
            {
                sql += " where " + where;
            }
            DataTable dt = SqlHelper.Squery(sql, conn);
            lLog = DBTool.GetListFromDatatable<Log>(dt);
            return lLog;
        }
        public IList<Log> GetLogListByProc(SqlConnection conn, SqlParameter[] paras, string strProcedureName)
        {
            IList<Log> lLog = new List<Log>();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddRange(paras);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = strProcedureName;
            cmd.Connection = conn;
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    lLog = DBTool.GetListFromDatatable<Log>(ds.Tables[0]);  
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return lLog;
        }
    }
}
