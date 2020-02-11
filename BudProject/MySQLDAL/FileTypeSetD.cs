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
    public class FileTypeSetD : IFileTypeSet
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileTypeSet"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertFileTypeSet(FileTypeSet FileTypeSet, OdbcConnection conn) 
        {
            try
            {
                return db.insert(FileTypeSet, "FileTypeSet", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileTypeSet"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateFileTypeSet(FileTypeSet FileTypeSet, OdbcConnection conn) 
        {
            int result = -1;
            string sql = "UPDATE fileTypeSet SET monitorServerFolderName = ?,"
                          + " monitorServerID = ?,"
                          + " exceptAttribute1 = ?,"
                          + " exceptAttribute2 = ?,"
                          + " exceptAttribute3 = ?,"
                          + " exceptAttributeFlg1 = ?,"
                          + " exceptAttributeFlg2 = ?,"
                          + " exceptAttributeFlg3 = ?,"
                          + " systemFileFlg = ?,"
                          + " hiddenFileFlg = ?,"
                          //+ " creater = ?,"
                          //+ " createDate = ?,"
                          + " updater = ?,"
                          + " updateDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@monitorServerFolderName",FileTypeSet.monitorServerFolderName),
                new OdbcParameter("@monitorServerID",FileTypeSet.monitorServerID),
                new OdbcParameter("@exceptAttribute1",FileTypeSet.exceptAttribute1),
                new OdbcParameter("@exceptAttribute2",FileTypeSet.exceptAttribute2),
                new OdbcParameter("@exceptAttribute3",FileTypeSet.exceptAttribute3),
                new OdbcParameter("@exceptAttributeFlg1",FileTypeSet.exceptAttributeFlg1),
                new OdbcParameter("@exceptAttributeFlg2",FileTypeSet.exceptAttributeFlg2),
                new OdbcParameter("@exceptAttributeFlg3",FileTypeSet.exceptAttributeFlg3),
                new OdbcParameter("@systemFileFlg",FileTypeSet.systemFileFlg),
                new OdbcParameter("@hiddenFileFlg",FileTypeSet.hiddenFileFlg),
                //new OdbcParameter("@creater",FileTypeSet.creater),
                //new OdbcParameter("@createDate",FileTypeSet.createDate),
                new OdbcParameter("@updater",FileTypeSet.updater),
                new OdbcParameter("@updateDate",FileTypeSet.updateDate),
                new OdbcParameter("@id",FileTypeSet.id)
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
        /// <param name="FileTypeSetId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteFileTypeSetById(int FileTypeSetId, string loginID, OdbcConnection conn) 
        {
            int result = -1;
            string sql = "UPDATE fileTypeSet SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",FileTypeSetId)
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
        public IList<FileTypeSet> GetFileTypeSet(IEnumerable<SearchCondition> conditon, OdbcConnection conn) 
        {
            IList<FileTypeSet> lFileTypeSet = new List<FileTypeSet>();
            string sql = "select id,monitorServerFolderName,monitorServerID,exceptAttribute1,exceptAttribute2,exceptAttribute3,exceptAttributeFlg1,exceptAttributeFlg2,exceptAttributeFlg3,systemFileFlg,hiddenFileFlg from fileTypeSet";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lFileTypeSet = DBTool.GetListFromDatatable<FileTypeSet>(dt);
            return lFileTypeSet;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetFileTypeSetCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn) 
        {
            string sql = @"SELECT count(id) as count FROM fileTypeSet";
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
        public IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn) 
        {
            IList<FileTypeSet> lFileTypeSet = new List<FileTypeSet>();
            string sql = @"SELECT id,monitorServerFolderName,monitorServerID,exceptAttribute1,exceptAttribute2,exceptAttribute3,exceptAttributeFlg1,exceptAttributeFlg2,exceptAttributeFlg3,systemFileFlg,hiddenFileFlg
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM fileTypeSet ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lFileTypeSet = DBTool.GetListFromDatatable<FileTypeSet>(dt);
            return lFileTypeSet;
        }
    }
}
