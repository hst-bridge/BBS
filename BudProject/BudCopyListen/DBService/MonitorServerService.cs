using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.DBInterface;
using System.Data;
using BudCopyListen.Entities;

namespace BudCopyListen.DBService
{
    public class MonitorServerService : IMonitorServerService
    {
        private BudBackup2Context db = null;

        public MonitorServerService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// 監視リスト取得
        /// </summary>
        /// <returns></returns>
        public List<Entities.monitorServer> GetMonitorServerList()
        {
            // 有効の監視対象
            int deleteFlg = 0;
            return db.monitorServers.Where(c => c.deleteFlg == deleteFlg).ToList();
        }

        /// <summary>
        /// 監視リスト取得
        /// </summary>
        /// <returns></returns>
        public Entities.monitorServer GetMonitorServerById(int id)
        {
            Entities.monitorServer monitorServer = db.monitorServers.Find(id);
            return monitorServer;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Edit(Entities.monitorServer monitorServer)
        {
            db.Entry(monitorServer).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
