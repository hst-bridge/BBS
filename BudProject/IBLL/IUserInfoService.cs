using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IUserInfoService
    {
        /// <summary>
        /// 增加ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        int InsertUserInfo(UserInfo UserInfo);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="UserInfoId"></param>
        /// <returns></returns>
        int DeleteUserInfo(int UserInfoId, string loginID);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        int UpdateUserInfo(UserInfo UserInfo);
        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="UserInfoId"></param>
        /// <returns></returns>
        UserInfo GetUserInfoById(int UserInfoId);
        /// <summary>
        /// 根据条件获取
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<UserInfo> GetUserInfoPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetUserInfoCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// 根据条件获取ユーザー
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        UserInfo userExist(UserInfo UserInfo);
        /// <summary>
        /// ユーザーリスト
        /// </summary>
        /// <returns></returns>
        IList<UserInfo> GetUserInfoList();
    }
}
