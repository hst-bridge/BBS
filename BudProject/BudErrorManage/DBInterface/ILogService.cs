using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudErrorManage.Entities;

namespace BudErrorManage.DBInterface
{
    public interface ILogService
    {
        /// <summary>
        /// Logデータ
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorTime"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.log> GetLogData(int monitorServerID, string monitorFileName, string monitorFilePath, int deleteFlg);

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="log"></param>
        void Insert(Entities.log log);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="log"></param>
        void Edit(Entities.log log);

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        void Del(int id);
    }
}
