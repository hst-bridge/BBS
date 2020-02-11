using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Model;
using Common;
using System.Data.Odbc;

namespace IDAL
{
    public interface IUserInfo
    {
        /// <summary>
        /// 增加ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        //int InsertUserInfo(UserInfo UserInfo, SqlConnection conn);
        /// <summary>
        /// 更新ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        //int UpdateUserInfo(UserInfo UserInfo, SqlConnection conn);
        /// <summary>
        /// 根据ユーザーid删除ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        //int DeleteUserInfoById(int UserInfoId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取ユーザー
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<UserInfo> GetUserInfo(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的ユーザー数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetUserInfoCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的ユーザー
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        /// <summary>
        /// 根据条件获取ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        //UserInfo userExist(UserInfo UserInfo, SqlConnection conn);
        ////////////////////////////////////////////////////ODBC connection/////////////////////////////////////////////////////
        /// <summary>
        /// 增加ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        int InsertUserInfo(UserInfo UserInfo, OdbcConnection conn);
        /// <summary>
        /// 更新ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        int UpdateUserInfo(UserInfo UserInfo, OdbcConnection conn);
        /// <summary>
        /// 根据ユーザーid删除ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        int DeleteUserInfoById(int UserInfoId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取ユーザー
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<UserInfo> GetUserInfo(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的ユーザー数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetUserInfoCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的ユーザー
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        UserInfo userExist(UserInfo UserInfo, OdbcConnection conn);
    }
}
