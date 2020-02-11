using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Model;
using Common;
using System.Data;
using System.Data.Odbc;

namespace IDAL
{
    public interface ILog
    {
        /// <summary>
        /// 增加ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        //int InsertLog(Log Log, SqlConnection conn);
        /// <summary>
        /// 更新ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        //int UpdateLog(Log Log, SqlConnection conn);
        /// <summary>
        /// 根据ログid删除ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        //int DeleteLogById(int LogId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取ログ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<Log> GetLog(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的ログ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetLogCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的ログ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<Log> GetLogPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的ログ
        /// </summary>
        /// <param name="where"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<Log> GetConditionLog(string where, SqlConnection conn);
        /// <summary>
        /// ログリスト
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="paras">パラメータ</param>
        /// <param name="strProcedureName">ストアド名前</param>
        /// <returns></returns>
        //IList<Log> GetLogListByProc(SqlConnection conn, SqlParameter[] paras, string strProcedureName);
        /////////////////////////////////////ODBC connection/////////////////////////////////////////////////
        /// <summary>
        /// 增加ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        int InsertLog(Log Log, OdbcConnection conn);
        /// <summary>
        /// 更新ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        int UpdateLog(Log Log, OdbcConnection conn);
        /// <summary>
        /// 根据ログid删除ログ
        /// </summary>
        /// <param name="Log"></param>
        /// <returns></returns>
        int DeleteLogById(int LogId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取ログ
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<Log> GetLog(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的ログ数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetLogCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的ログ
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<Log> GetLogPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的ログ
        /// </summary>
        /// <param name="where"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<Log> GetConditionLog(string where, OdbcConnection conn);
        /// <summary>
        /// ログリスト
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="paras">パラメータ</param>
        /// <param name="strProcedureName">ストアド名前</param>
        /// <returns></returns>
        IList<Log> GetLogListByProc(OdbcConnection conn, OdbcParameter[] paras, string strProcedureName);
    }
}
