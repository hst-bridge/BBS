using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IBackupServerService
    {
        /// <summary>
        /// 增加バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int InsertBackupServer(BackupServer BackupServer);
        /// <summary>
        /// 删除バックアップ先対象
        /// </summary>
        /// <param name="BackupServerId"></param>
        /// <returns></returns>
        int DeleteBackupServer(int BackupServerId, string loginID);
        /// <summary>
        /// 更新バックアップ先対象
        /// </summary>
        /// <param name="BackupServer"></param>
        /// <returns></returns>
        int UpdateBackupServer(BackupServer BackupServer);
        /// <summary>
        /// 根据ID获取バックアップ先対象
        /// </summary>
        /// <param name="BackupServerId"></param>
        /// <returns></returns>
        BackupServer GetBackupServerById(int BackupServerId);
        /// <summary>
        /// 根据条件获取バックアップ先対象
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerPage(IEnumerable<SearchCondition> condition, int page, int pagesize);
        /// <summary>
        /// 获取满足条件的记录数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int GetBackupServerCount(IEnumerable<SearchCondition> condition);
        /// <summary>
        /// バックアップ先対象リスト
        /// </summary>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerList();
        /// <summary>
        /// get part backup server list
        /// </summary>
        /// <returns></returns>
        IList<BackupServer> GetPartBackupServerList(string groupId);
        /// <summary>
        /// get the server group detail list 
        /// </summary>
        /// <returns></returns>
        IList<BackupServer> GetGroupBackupServerList(string groupId);

        /// <summary>
        /// バックアップ先名によって、対象リストを取得する。
        /// </summary>
        /// <param name="bkName"></param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerListByName(string bkName);

        /// <summary>
        /// このIDに加えて、バックアップ先名によって、対象リストを取得する。
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkName">バックアップ先名</param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerListByNameButId(int id, string bkName);

        /// <summary>
        /// このIDに加えて、IPとstartFolderによって、対象リストを取得する。
        /// </summary>
        /// 2014-06-24 wjd add
        /// <param name="id"></param>
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        IList<BackupServer> GetBackupServerListButId(int id, string bkIP, string startFolder);

        /// <summary>
        /// Get Deleted BackupServer List By IP And StartFolder
        /// </summary>
        /// 2014-06-30 wjd add
        /// <param name="bkIP"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        IList<BackupServer> GetDeletedBackupServerList(string bkIP, string startFolder);
    }
}
