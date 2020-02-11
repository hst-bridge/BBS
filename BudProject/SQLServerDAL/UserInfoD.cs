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
    public class UserInfoD//:IUserInfo
    {
        public Database db = new Database();
        public int InsertUserInfo(UserInfo UserInfo, SqlConnection conn)
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

        public int UpdateUserInfo(UserInfo UserInfo, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE userInfo SET loginID = @loginID,"
                          + " password = @password,"
                          + " name = @name, "
                          + " mail =@mail,"
                          + " mailFlg =@mailFlg,"
                          + " authorityFlg =@authorityFlg,"
                          + " updater =@updater,"
                          + " updateDate =@updateDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@loginID",UserInfo.loginID),
                new SqlParameter("@password",UserInfo.password),
                new SqlParameter("@name",UserInfo.name),
                new SqlParameter("@mail",UserInfo.mail),
                new SqlParameter("@mailFlg",UserInfo.mailFlg),
                new SqlParameter("@authorityFlg",UserInfo.authorityFlg),
                new SqlParameter("@updater",UserInfo.updater),
                new SqlParameter("@updateDate",UserInfo.updateDate),
                new SqlParameter("@id",UserInfo.id)
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

        public int DeleteUserInfoById(int UserInfoId, string loginID, SqlConnection conn)
        {
            int result = -1;
            string sql = "UPDATE userInfo SET deleteFlg = 1,"
                          + "deleter = @deleter,"
                          + "deleteDate = @deleteDate"
                          + " WHERE id = @id";

            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@id",UserInfoId),
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

        public IList<UserInfo> GetUserInfo(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            IList<UserInfo> lUserInfo = new List<UserInfo>();
            string sql = @"select id,loginID,name,password,mail,mailFlg,authorityFlg from userInfo";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lUserInfo = DBTool.GetListFromDatatable<UserInfo>(dt);
            return lUserInfo;
        }

        public int GetUserInfoCount(IEnumerable<SearchCondition> conditon, SqlConnection conn)
        {
            string sql = @"SELECT count(*) as count FROM userInfo";
            if (conditon.Count() > 0)
            {
                string con = DBTool.GetSqlcon(conditon);
                sql += " where " + con;
            }
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, spvalues);
            return count;
        }

        public IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn)
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
            SqlParameter[] spvalues = DBTool.GetSqlParam(conditon);
            DataTable dt = SqlHelper.Squery(sql, conn, spvalues);
            lUserInfo = DBTool.GetListFromDatatable<UserInfo>(dt);
            return lUserInfo;
        }
        /// <summary>
        /// user exist check
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfo userExist(UserInfo userInfo,SqlConnection conn)
        {
            string sql = "SELECT LoginID,password,name,mail,authorityFlg FROM userInfo where LoginID=@LoginID"
                        + " AND password=@password"
                        + " AND deleteFlg=0";
            SqlParameter[] para = new SqlParameter[]{                
                new SqlParameter("@LoginID",userInfo.loginID),
                new SqlParameter("@password",userInfo.password)
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
