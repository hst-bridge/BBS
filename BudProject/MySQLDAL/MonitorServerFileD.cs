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
    public class MonitorServerFileD : IMonitorServerFile
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertMonitorServerFile(MonitorServerFile MonitorServerFile, OdbcConnection conn)
        {
            try
            {
                return db.insert(MonitorServerFile, "monitorServerFile", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile, OdbcConnection conn)
        {
            string sql = @"UPDATE monitorServerFile
                               SET monitorServerID = ?
                                  ,monitorFileName = ?
                                  ,monitorFilePath = ?
                                  ,monitorFileType = ?
                                  ,monitorFileSize = ?
                                  ,monitorTime = ?
                                  ,transferFlg = ?
                                  ,updater = ?
                                  ,updateDate = ? 
                             WHERE id=?";
            try
            {
                OdbcParameter[] spvalues = DBTool.GetOdbcPm(MonitorServerFile);
                return OdbcHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql, spvalues);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="transferFlg"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateTransferFlg(string MonitorServerFileId, int transferFlg ,OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = ?,updater = 'exe',"
                          + "updateDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@transferFlg",transferFlg), 
                new OdbcParameter("@updateDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerFileId)                
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
        /// <param name="MonitorFileDirectory"></param>
        /// <param name="transferFlg"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateAllTransferFlg(string MonitorFileDirectory, int transferFlg, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = ?,updater = 'exe',"
                          + "updateDate = ?"
                          + " WHERE monitorFileDirectory= ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@transferFlg",transferFlg), 
                new OdbcParameter("@updateDate",DateTime.Now),
                new OdbcParameter("@monitorFileDirectory",MonitorFileDirectory)                
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
        /// <param name="MonitorServerFileId"></param>
        /// <param name="transferFlg"></param>
        /// <param name="transferNum"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateNGTransferFlg(string MonitorServerFileId, int transferFlg, int transferNum, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = ?,transferNum = ?,updater = 'exe',"
                          + "updateDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@transferFlg",transferFlg),
                new OdbcParameter("@transferNum",transferNum),
                new OdbcParameter("@updateDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerFileId)                
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
        /// <param name="MonitorServerFileId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateTransferFlgBatch(string MonitorServerFileId, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = 1,"
                          + "updateDate = ?"
                          + " WHERE id in(?)";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@updateDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerFileId)
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
        /// <param name="MonitorServerFileId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorServerFileById(int MonitorServerFileId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerFileId)
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
        public IList<MonitorServerFile> GetMonitorServerFile(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorServerFile> lMonitorServerFile = new List<MonitorServerFile>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFileDirectory
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorFileStatus
                              ,monitorStartTime
                              ,transferFlg
                              ,transferNum
                          FROM monitorServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql += " order by id desc";
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServerFile = DBTool.GetListFromDatatable<MonitorServerFile>(dt);
            return lMonitorServerFile;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorServerFileCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServerFile";
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
        public IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorServerFile> lMonitorServerFile = new List<MonitorServerFile>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorTime
                              ,transferFlg
                              ROW_NUMBER() over(order by createDate) as row
                          FROM monitorServerFile ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServerFile = DBTool.GetListFromDatatable<MonitorServerFile>(dt);
            return lMonitorServerFile;
        }
    }
}
