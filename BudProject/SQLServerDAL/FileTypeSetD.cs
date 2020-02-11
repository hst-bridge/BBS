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
    public class FileTypeSetD// : IFileTypeSet
    {
        public Database db = new Database();
        public int InsertFileTypeSet(FileTypeSet FileTypeSet, SqlConnection conn) 
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

        public int UpdateFileTypeSet(FileTypeSet FileTypeSet, SqlConnection conn) 
        {
            int result = -1;
            string sql = "UPDATE fileTypeSet SET monitorServerFolderName = @monitorServerFolderName,"
                          + " monitorServerID = @monitorServerID,"
                          + " exceptAttribute1 = @exceptAttribute1,"
                          + " exceptAttribute2 = @exceptAttribute2,"
                          + " exceptAttribute3 = @exceptAttribute3,"
                          + " systemFileFlg = @systemFileFlg,"
                          + " hiddenFileFlg = @hiddenFileFlg,"
                          //+ " creater = @creater,"
                          //+ " createDate = @createDate,"
                          + " updater = @updater,"
                          + " updateDate = @updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@monitorServerFolderName",FileTypeSet.monitorServerFolderName),
                new SqlParameter("@monitorServerID",FileTypeSet.monitorServerID),
                new SqlParameter("@exceptAttribute1",FileTypeSet.exceptAttribute1),
                new SqlParameter("@exceptAttribute2",FileTypeSet.exceptAttribute2),
                new SqlParameter("@exceptAttribute3",FileTypeSet.exceptAttribute3),
                new SqlParameter("@systemFileFlg",FileTypeSet.systemFileFlg),
                new SqlParameter("@hiddenFileFlg",FileTypeSet.hiddenFileFlg),
                //new SqlParameter("@creater",FileTypeSet.creater),
                //new SqlParameter("@createDate",FileTypeSet.createDate),
                new SqlParameter("@updater",FileTypeSet.updater),
                new SqlParameter("@updateDate",FileTypeSet.updateDate),
                new SqlParameter("@id",FileTypeSet.id)
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

        public int DeleteFileTypeSetById(int FileTypeSetId, string loginID, SqlConnection conn) 
        {
            int result = -1;
            string sql = "UPDATE fileTypeSet SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",FileTypeSetId),
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

        public IList<FileTypeSet> GetFileTypeSet(IEnumerable<SearchCondition> conditon, SqlConnection conn) 
        {
            IList<FileTypeSet> lFileTypeSet = new List<FileTypeSet>();
            string sql = "select id,monitorServerFolderName,monitorServerID,exceptAttribute1,exceptAttribute2,exceptAttribute3,systemFileFlg,hiddenFileFlg from fileTypeSet";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lFileTypeSet = DBTool.GetListFromDatatable<FileTypeSet>(dt);
            return lFileTypeSet;
            
        }

        public int GetFileTypeSetCount(IEnumerable<SearchCondition> conditon, SqlConnection conn) 
        {
            string sql = @"SELECT count(*) as count FROM fileTypeSet";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn) 
        {
            IList<FileTypeSet> lFileTypeSet = new List<FileTypeSet>();
            string sql = @"SELECT id,monitorServerFolderName,monitorServerID,exceptAttribute1,exceptAttribute2,exceptAttribute3,systemFileFlg,hiddenFileFlg
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM fileTypeSet ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lFileTypeSet = DBTool.GetListFromDatatable<FileTypeSet>(dt);
            return lFileTypeSet;
        }
    }
}
