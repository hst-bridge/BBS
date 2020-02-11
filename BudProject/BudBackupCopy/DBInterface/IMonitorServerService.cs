using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudBackupCopy.DBInterface
{
    public interface IMonitorServerService
    {
        /// <summary>
        /// 監視リスト取得
        /// </summary>
        /// <returns></returns>
        List<Entities.monitorServer> GetMonitorServerList();

        /// <summary>
        /// 監視対象取得
        /// </summary>
        /// <returns></returns>
        Entities.monitorServer GetMonitorServerById(int id);

                /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Edit(Entities.monitorServer monitorServer);
    }
}
