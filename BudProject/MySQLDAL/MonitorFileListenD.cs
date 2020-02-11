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
    public class MonitorFileListenD : IMonitorFileListen
    {
        public OdbcDatabase db = new OdbcDatabase();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServer"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateMonitorFileListen(MonitorServer MonitorServer, MonitorServer MonitorServerForOld, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE monitorFileListen SET monitorServerIP = ?,"
                          + " sharePoint = ?,"
                          + " monitorLocalPath = ?,"
                          + " updater = ?,"
                          + " updateDate = ?,"
                          + " synchronismFlg = 0"
                          + " WHERE monitorServerIP = ? and sharePoint = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@monitorServerIP",MonitorServer.monitorServerIP),
                new OdbcParameter("@sharePoint",MonitorServer.startFile),
                new OdbcParameter("@monitorLocalPath",MonitorServer.monitorLocalPath),
                new OdbcParameter("@updater",MonitorServer.updater),
                new OdbcParameter("@updateDate",MonitorServer.updateDate),
                new OdbcParameter("@monitorServerIP2",MonitorServerForOld.monitorServerIP),
                new OdbcParameter("@sharePoint2",MonitorServerForOld.startFile),
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
        public IList<MonitorFileListen> GetMonitorFileListen(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            string sql = @"select id,monitorServerID,monitorFileName,monitorType,monitorServerIP,sharePoint,monitorLocalPath,monitorFileRelativeDirectory,monitorFileRelativeFullPath,monitorFileLastWriteTime,monitorFileSize,
            monitorFileExtension,monitorFileCreateTime,monitorFileLastAccessTime,monitorStatus,monitorFileStartTime,
            monitorFileEndTime,deleteFlg,deleter,deleteDate,creater,createDate,updater,updateDate 
            from MonitorFileListen";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorFileListen = DBTool.GetListFromDatatable<MonitorFileListen>(dt);
            return lMonitorFileListen;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorFileListenCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM MonitorFileListen";
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
        /// <param name="where"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetMonitorFileListenListCount(string where, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM (select id,ROW_NUMBER() over (PARTITION by monitorFileRelativeDirectory order by monitorFileRelativeDirectory) rowno from MonitorFileListen ";
            if (where != "")
            {
                sql += " where " + where;
            }
            sql += " ) as a where rowno = 1 ";
            int count = (int)OdbcHelper.ExecuteScalar(conn, CommandType.Text, sql);
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
        public IList<MonitorFileListen> GetMonitorFileListenPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            string sql = @"select id,monitorServerID,monitorFileName,monitorType,monitorServerIP,sharePoint,monitorLocalPath,monitorFileRelativeDirectory,monitorFileRelativeFullPath,monitorFileLastWriteTime,monitorFileSize,
            monitorFileExtension,monitorFileCreateTime,monitorFileLastAccessTime,monitorStatus,monitorFileStartTime,
            monitorFileEndTime,deleteFlg,deleter,deleteDate,creater,createDate,updater,updateDate,
            ROW_NUMBER() over(order by createDate) as row
                          FROM MonitorFileListen ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorFileListen = DBTool.GetListFromDatatable<MonitorFileListen>(dt);
            return lMonitorFileListen;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IList<MonitorFileListen> GetMonitorFileListenPage(string sql, int page, int pagesize, OdbcConnection conn)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            DataTable dt = OdbcHelper.Squery(sql, conn);

            lMonitorFileListen = DBTool.GetListFromDatatable<MonitorFileListen>(dt);
            return lMonitorFileListen;
        }

        /// <summary>
        /// 获取匹配项 xiecongwen 20140711
        /// </summary>
        /// <param name="DBServerIP">数据来源</param>
        /// <param name="monitorServerId">监控serverid</param>
        /// <param name="keyName">关键字</param>
        /// <param name="pindex">页码</param>
        /// <param name="psize">页大小</param>
        /// <returns></returns>
        public IList<MonitorFileListen> GetMonitorFileListenByPage(string DBServerIP, string monitorServerId, string keyName, int pindex, int psize)
        {
            #region 拼凑sql语句
            StringBuilder sb = new StringBuilder();
            
            #endregion

            return null;
        }
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        public IList<MonitorFileListen> GetPartMonitorFileListenList(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            string sql = @"select id,monitorServerID,monitorFileName,monitorType,monitorServerIP,sharePoint,monitorLocalPath,monitorFileRelativeDirectory,monitorFileRelativeFullPath,monitorFileLastWriteTime,monitorFileSize,
monitorFileExtension,monitorFileCreateTime,monitorFileLastAccessTime,monitorStatus,monitorFileStartTime,
monitorFileEndTime,deleteFlg,deleter,deleteDate,creater,createDate,updater,updateDate 
from MonitorFileListen";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lMonitorFileListen = DBTool.GetListFromDatatable<MonitorFileListen>(dt);
            return lMonitorFileListen;
        }
        public IList<MonitorFileListen> GetPartMonitorFileListenList(string where, OdbcConnection conn)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            string sql = @"select id,monitorServerID,monitorFileName,monitorType,monitorServerIP,sharePoint,monitorLocalPath,monitorFileRelativeDirectory,monitorFileRelativeFullPath,monitorFileLastWriteTime,monitorFileSize,
monitorFileExtension,monitorFileCreateTime,monitorFileLastAccessTime,monitorStatus,monitorFileStartTime,
monitorFileEndTime,deleteFlg,deleter,deleteDate,creater,createDate,updater,updateDate 
from MonitorFileListen";
            if (where != "")
            {
                sql += " where " + where;
            }
            DataTable dt = OdbcHelper.Squery(sql, conn);
            lMonitorFileListen = DBTool.GetListFromDatatable<MonitorFileListen>(dt);
            return lMonitorFileListen;
        }
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        public IList<MonitorFileListen> GetMonitorFileListenList(OdbcConnection conn, string groupId)
        {
            IList<MonitorFileListen> lMonitorFileListen = new List<MonitorFileListen>();
            return lMonitorFileListen;
        }
    }
}
