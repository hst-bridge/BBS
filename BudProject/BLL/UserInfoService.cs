using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBLL;
using Common;
using IDAL;
using System.Data.SqlClient;
using DBUtility;
using Model;
using System.Data.Odbc;
using System.Windows.Forms;
using System.Reflection;
using log4net;

namespace BLL
{
    public class UserInfoService :IUserInfoService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IUserInfo UserInfoDal = DALFactory.DataAccess.CreateUserInfo();
        public int GetUserInfoCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count=0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count=UserInfoDal.GetUserInfoCount(condition,conn);
                conn.Close();
            }
            return count;
        }
        public int DeleteUserInfo(int UserInfoId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = UserInfoDal.DeleteUserInfoById(UserInfoId,loginID,conn);
                conn.Close();
            }
            return count;
        }

        public int UpdateUserInfo(UserInfo UserInfo)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = UserInfoDal.UpdateUserInfo(UserInfo,conn);
                conn.Close();
            }
            return count;
        }

        public int InsertUserInfo(UserInfo UserInfo)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = UserInfoDal.InsertUserInfo(UserInfo, conn);
                conn.Close();
            }
            return count;
        }

        public UserInfo GetUserInfoById(int UserInfoId)
        {
            OdbcConnection conn;
            IList<UserInfo> UserInfo;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition{con="id=?",param="@id",value=UserInfoId.ToString()}};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                UserInfo = UserInfoDal.GetUserInfo(condition, conn);
                if(UserInfo.Count>0)
                {
                    return UserInfo[0];
                }
                conn.Close();
                return null;
            }
        }

        public IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<UserInfo> UserInfo;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                UserInfo = UserInfoDal.GetUserInfoPage(condition, page, pagesize, conn);
                conn.Close();
                return UserInfo;
            }
        }
        public UserInfo userExist(UserInfo userInfo)
        {
            OdbcConnection conn;
            UserInfo UserInfo;
            using (conn = OdbcHelper.CreateConntion())
            {
                try {
                    // 処理する
                    conn.Open();
                    UserInfo = UserInfoDal.userExist(userInfo,conn);
                    return UserInfo;

                }
               catch (Exception ex)
                {
                    throw(ex);
                }
            }
        }

        public IList<UserInfo> GetUserInfoList()
        {
            OdbcConnection conn;
            IList<UserInfo> UserInfo;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                UserInfo = UserInfoDal.GetUserInfo(condition, conn);
                conn.Close();
                return UserInfo;
            }
        }
    }
}
