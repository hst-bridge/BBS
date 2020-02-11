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
    public class MonitorServerFolderD : IMonitorServerFolder
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFolder"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertMonitorServerFolder(MonitorServerFolder MonitorServerFolder, OdbcConnection conn)
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
        public int UpdateMonitorServerFolder(MonitorServerFolder MonitorServerFolder, OdbcConnection conn)
        {
            string sql = @"UPDATE monitorServerFolder
                               SET monitorServerID = ?
                                  ,monitorFileName = ?
                                  ,monitorFilePath = ?
                                  ,monitorFileType = ?
                                  ,monitorFlg = ?
                                  ,updater = ?
                                  ,updateDate = ? 
                             WHERE id=?";
            try
            {
                OdbcParameter[] spvalues = DBTool.GetOdbcPm(MonitorServerFolder);
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
        /// <param name="MonitorServerFolderId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorServerFolderById(int MonitorServerFolderId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorServerFolder SET deleteFlg = 1,"
                          + "deleter = ？,"
                          + "deleteDate = ？"
                          + " WHERE id = ？";

            OdbcParameter[] para = new OdbcParameter[]{                                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",MonitorServerFolderId)
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
        public IList<MonitorServerFolder> GetMonitorServerFolder(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorServerFolder> lMonitorServerFolder = new List<MonitorServerFolder>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFlg 
                              ,initFlg
                          FROM monitorServerFolder";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServerFolder = DBTool.GetListFromDatatable<MonitorServerFolder>(dt);
            return lMonitorServerFolder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorServerFolderCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM monitorServerFolder";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            int count = Int32.Parse(OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues).ToString());
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
        public IList<MonitorServerFolder> GetMonitorServerFolderPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorServerFolder> lMonitorServerFolder = new List<MonitorServerFolder>();
            string sql = @"SELECT id
                              ,monitorServerID
                              ,monitorFileName
                              ,monitorFilePath
                              ,monitorFileType
                              ,monitorFlg 
                              ,initFlg
                              ROW_NUMBER() over(order by createDate) as row
                          FROM monitorServerFolder ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorServerFolder = DBTool.GetListFromDatatable<MonitorServerFolder>(dt);
            return lMonitorServerFolder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteMonitorServerFolderByServerId(int MonitorServerId, OdbcConnection conn)
        {
            int result = -1;
            string sql = "DELETE FROM monitorServerFolder"
                          + " WHERE monitorServerID = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@id",MonitorServerId)
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
