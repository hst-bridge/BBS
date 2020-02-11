using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IBackupServerFileService
    {
        /// <summary>
        /// 增加バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        int InsertBackupServerFile(BackupServerFile BackupServerFile);
        /// <summary>
        /// 删除バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFileId"></param>
        /// <returns></returns>
        int DeleteBackupServerFile(int BackupServerFileId, string loginID);
        /// <summary>
        /// 更新バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFile"></param>
        /// <returns></returns>
        int UpdateBackupServerFile(BackupServerFile BackupServerFile);
        /// <summary>
        /// 根据ID获取バックアップ先対象ファイル
        /// </summary>
        /// <param name="BackupServerFileId"></param>
        /// <returns></returns>
        BackupServerFile GetBackupServerFileById(int BackupServerFileId);
        /// <summary>
        /// 根据条件获取バックアップ先対象ファイル
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<BackupServerFile> GetBackupServerFilePage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetBackupServerFileCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ先対象ファイルリスト
        /// </summary>
        /// <returns></returns>
        IList<BackupServerFile> GetBackupServerFileList();
    }
}
