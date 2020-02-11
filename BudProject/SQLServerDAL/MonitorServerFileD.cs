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
    public class MonitorServerFileD// : IMonitorServerFile
    {
        public Database db = new Database();
        public int InsertMonitorServerFile(MonitorServerFile MonitorServerFile, SqlConnection conn)
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

        public int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile, SqlConnection conn)
        {
            string sql = @"UPDATE monitorServerFile
                               SET monitorServerID = @monitorServerID
                                  ,monitorFileName = @monitorFileName
                                  ,monitorFilePath = @monitorFilePath
                                  ,monitorFileType = @monitorFileType
                                  ,monitorFileSize = @monitorFileSize
                                  ,monitorTime = @monitorTime
                                  ,transferFlg = @transferFlg
                                  ,updater = @updater
                                  ,updateDate = @updateDate
                             WHERE id=@id";
            try
            {
                SqlParameter[] spvalues = DBTool.GetSqlPm(MonitorServerFile);
                return SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql, spvalues);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateTransferFlg(string MonitorServerFileId, int transferFlg ,SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = @transferFlg,updater = 'exe',"
                          + "updateDate = @updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@transferFlg",transferFlg), 
                new SqlParameter("@id",MonitorServerFileId),                
                new SqlParameter("@updateDate",DateTime.Now)
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
        public int UpdateTransferFlgBatch(string MonitorServerFileId, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET transferFlg = 1,"
                          + "updateDate = @updateDate"
                          + " WHERE id in(@id)";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorServerFileId),                
                new SqlParameter("@updateDate",DateTime.Now)
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

        public int DeleteMonitorServerFileById(int MonitorServerFileId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFile SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorServerFileId),
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

        public IList<MonitorServerFile> GetMonitorServerFile(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<MonitorServerFile> lMonitorServerFile = new List<MonitorServerFile>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFileSize
                              ,monitorFileStatus
                              ,monitorTime
                              ,transferFlg
                          FROM monitorServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql += " order by id asc";
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServerFile = DBTool.GetListFromDatatable<MonitorServerFile>(dt);
            return lMonitorServerFile;
        }

        public int GetMonitorServerFileCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServerFile";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServerFile = DBTool.GetListFromDatatable<MonitorServerFile>(dt);
            return lMonitorServerFile;
        }
    }
}
