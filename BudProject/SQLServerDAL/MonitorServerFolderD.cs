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
    public class MonitorServerFolderD// : IMonitorServerFolder
    {
        public Database db = new Database();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder, SqlConnection conn)
        {
            try
            {
                return db.insert(MonitorServerFolder, "monitorServerFolder", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder, SqlConnection conn)
        {
            string sql = @"UPDATE monitorServerFolder
                               SET monitorServerID = @monitorServerID
                                  ,monitorFileName = @monitorFileName
                                  ,monitorFilePath = @monitorFilePath
                                  ,monitorFileType = @monitorFileType
                                  ,monitorFlg = @monitorFlg
                                  ,updater = @updater
                                  ,updateDate = @updateDate
                             WHERE id=@id";
            try
            {
                SqlParameter[] spvalues = DBTool.GetSqlPm(MonitorServerFolder);
                return SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql, spvalues);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFolderId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorServerFolderById(int MonitorServerFolderId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFolder SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorServerFolderId),
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorServerFolder> GetMonitorServerFolder(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<MonitorServerFolder> lMonitorServerFolder = new List<MonitorServerFolder>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFlg 
                          FROM monitorServerFolder";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServerFolder = DBTool.GetListFromDatatable<MonitorServerFolder>(dt);
            return lMonitorServerFolder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorServerFolderCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServerFolder";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
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
        public IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
        {
            IList<MonitorServerFolder> lMonitorServerFolder = new List<MonitorServerFolder>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFlg 
                              ROW_NUMBER() over(order by createDate) as row
                          FROM monitorServerFolder ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lMonitorServerFolder = DBTool.GetListFromDatatable<MonitorServerFolder>(dt);
            return lMonitorServerFolder;
        }
        public int DeleteMonitorServerFolderByServerId(int MonitorServerId, SqlConnection conn)
        {
            int result = -1;
            string sql = "DELETE FROM monitorServerFolder"
                          + " WHERE monitorServerID = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",MonitorServerId)
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
    }
}
