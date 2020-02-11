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
    public interface IBackupServerFile
    {
        /// <summary>
        /// 增加バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        //int InsertBackupServerFile(BackupServerFile BackupServerFile, SqlConnection conn);
        /// <summary>
        /// 更新バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        //int UpdateBackupServerFile(BackupServerFile BackupServerFile, SqlConnection conn);
        /// <summary>
        /// 根据バックアップ先対象ファイルid删除バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        //int DeleteBackupServerFileById(int BackupServerFileId, string loginID, SqlConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //IList<BackupServerFile> GetBackupServerFile(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象ファイル数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        //int GetBackupServerFileCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        //IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection////////////////////////////////////////////////////////////////
        /// <summary>
        /// 增加バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        int InsertBackupServerFile(BackupServerFile BackupServerFile, OdbcConnection conn);
        /// <summary>
        /// 更新バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        int UpdateBackupServerFile(BackupServerFile BackupServerFile, OdbcConnection conn);
        /// <summary>
        /// 根据バックアップ先対象ファイルid删除バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        int DeleteBackupServerFileById(int BackupServerFileId, string loginID, OdbcConnection conn);
        /// <summary>
        /// 根据条件获取バックアップ先対象ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        IList<BackupServerFile> GetBackupServerFile(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取满足条件的バックアップ先対象ファイル数量
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        int GetBackupServerFileCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);
        /// <summary>
        /// 获取指定页的バックアップ先対象ファイル
        /// </summary>
        /// <param name="conditon"></param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
