using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudFileListen.DBInterface;
using BudFileListen.DBService;
using BudFileListen.Entities;

namespace BudFileListen.Models
{
    public class MonitorServerManager
    {
        private IMonitorServerService monitorServerService = null;

        public MonitorServerManager() : this(new MonitorServerService()) { }

        public MonitorServerManager(IMonitorServerService monitorServerService)
        {
            this.monitorServerService = monitorServerService;
        }

        /// <summary>
        /// 監視リスト取得
        /// </summary>
        /// <returns></returns>
        public List<Entities.monitorServer> GetMonitorServerList()
        {
            return this.monitorServerService.GetMonitorServerList();
        }

        /// <summary>
        /// 監視対象取得
        /// </summary>
        /// <returns></returns>
        public Entities.monitorServer GetMonitorServerById(int id)
        {
            return this.monitorServerService.GetMonitorServerById(id);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Edit(monitorServer monitorServer)
        {
            this.monitorServerService.Edit(monitorServer);
        }
    }
}
