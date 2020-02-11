using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using Model;
using DBUtility;
using System.Data.SqlClient;
using Common;
using System.Data;
using System.Data.Odbc;

namespace MySQLDAL
{
    public class LogD : ILog
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Log"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertLog(Log Log, OdbcConnection conn)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Log"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateLog(Log Log, OdbcConnection conn)
        {
            string sql = @"UPDATE log
                               SET monitorServerID = ?
                                    ,monitorFileName = ?
                                    ,monitorFileStatus = ?
                                    ,monitorFilePath = ?
                                    ,monitorFileType = ?
                                    ,monitorFileSize = ?
                                    ,monitorTime = ?
                                    ,transferFlg = ?
                                    ,backupServerGroupID = ?
                                    ,backupServerID = ?
                                    ,backupServerFileName = ?
                                    ,backupServerFilePath = ?
                                    ,backupServerFileType = ?
                                    ,backupServerFileSize = ?
                                    ,backupStartTime = ?
                                    ,backupEndTime = ?
                                    ,backupTime = ?
                                    ,backupFlg = ?
                                    ,copyStartTime = ?
                                    ,copyEndTime = ?
                                    ,copyTime = ?
                                    ,copyFlg = ?
                                    ,updater = ?
                                    ,updateDate = ? 
                             WHERE id=@id";
            OdbcParameter[] spvalues = DBTool.GetOdbcPm(Log);
            return OdbcHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql, spvalues);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="LogId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteLogById(int LogId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE log SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",LogId)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<Log> GetLog(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<Log> lLog = new List<Log>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFileStatus
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
                          FROM log";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql += " order by id desc";
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lLog = DBTool.GetListFromDatatable<Log>(dt);
            return lLog;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetLogCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM log";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<Log> GetLogPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
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
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize + " order by a.id desc";
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lLog = DBTool.GetListFromDatatable<Log>(dt);
            return lLog;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<Log> GetConditionLog(string where, OdbcConnection conn)
        {
            IList<Log> lLog = new List<Log>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFileStatus
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
                          FROM log";
            if (where != "")
            {
                sql += " where " + where;
            }
            sql += " order by id desc";
            try
            {
                //conn.ConnectionTimeout = 150;
                DataTable dt = OdbcHelper.Squery(sql, conn);
                lLog = DBTool.GetListFromDatatable<Log>(dt);
            }
            catch (Exception ex) {
            }

            return lLog;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="paras"></param>
        /// <param name="strProcedureName"></param>
        /// <returns></returns>
        public IList<Log> GetLogListByProc(OdbcConnection conn, OdbcParameter[] paras, string strProcedureName)
        {
            IList<Log> lLog = new List<Log>();
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand();
            cmd.Parameters.AddRange(paras);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = strProcedureName;
            cmd.Connection = conn;
            cmd.CommandTimeout = 150;
            try
            {
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
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
