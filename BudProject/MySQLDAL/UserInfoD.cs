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
    public class UserInfoD:IUserInfo
    {
        public OdbcDatabase db = new OdbcDatabase();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int InsertUserInfo(UserInfo UserInfo, OdbcConnection conn)
        {
            try
            {
                return db.insert(UserInfo, "userInfo", conn);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int UpdateUserInfo(UserInfo UserInfo, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE userInfo SET loginID = ?,"
                          + " password = ?,"
                          + " name = ?, "
                          + " mail = ?,"
                          + " mailFlg = ?,"
                          + " authorityFlg = ?,"
                          + " updater = ?,"
                          + " updateDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@loginID",UserInfo.loginID),
                new OdbcParameter("@password",UserInfo.password),
                new OdbcParameter("@name",UserInfo.name),
                new OdbcParameter("@mail",UserInfo.mail),
                new OdbcParameter("@mailFlg",UserInfo.mailFlg),
                new OdbcParameter("@authorityFlg",UserInfo.authorityFlg),
                new OdbcParameter("@updater",UserInfo.updater),
                new OdbcParameter("@updateDate",UserInfo.updateDate),
                new OdbcParameter("@id",UserInfo.id)
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
        /// <param name="UserInfoId"></param>
        /// <param name="loginID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int DeleteUserInfoById(int UserInfoId, string loginID, OdbcConnection conn)
        {
            int result = -1;
            string sql = "UPDATE userInfo SET deleteFlg = 1,"
                          + "deleter = ?,"
                          + "deleteDate = ?"
                          + " WHERE id = ?";

            OdbcParameter[] para = new OdbcParameter[]{
                new OdbcParameter("@deleter",loginID),
                new OdbcParameter("@deleteDate",DateTime.Now),
                new OdbcParameter("@id",UserInfoId)
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
        public IList<UserInfo> GetUserInfo(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            IList<UserInfo> lUserInfo = new List<UserInfo>();
            string sql = @"select id,loginID,name,password,mail,mailFlg,authorityFlg from userInfo";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lUserInfo = DBTool.GetListFromDatatable<UserInfo>(dt);
            return lUserInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int GetUserInfoCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn)
        {
            string sql = @"SELECT count(id) as count FROM userInfo";
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
        public IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn)
        {
            IList<UserInfo> lUserInfo = new List<UserInfo>();
            string sql = @"SELECT id,loginID,name,password,mail,mailFlg,authorityFlg
                              ,ROW_NUMBER() over(order by createDate) as row
                          FROM userInfo ";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            sql = "select * from (" + sql + ") as a where row>" + (page - 1) * pagesize + " and row<=" + page * pagesize;
            OdbcParameter[] spvalues = DBTool.GetOdbcParam(conditon);
            DataTable dt = OdbcHelper.Squery(sql, conn, spvalues);
            lUserInfo = DBTool.GetListFromDatatable<UserInfo>(dt);
            return lUserInfo;
        }
        /// <summary>
        /// user exist check
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfo userExist(UserInfo userInfo,OdbcConnection conn)
        {
            string sql = "SELECT LoginID,password,name,mail,authorityFlg FROM userInfo where LoginID=?"
                        + " AND password=?"
                        + " AND deleteFlg=0";
            OdbcParameter[] para = new OdbcParameter[]{                
                new OdbcParameter("@LoginID",userInfo.loginID),
                new OdbcParameter("@password",userInfo.password)
            };
            try
            {
                DataTable dt = db.Squery(sql, conn, para);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        userInfo.authorityFlg = Int32.Parse(row["authorityFlg"].ToString());
                    }
                    return userInfo;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }
    }
}
