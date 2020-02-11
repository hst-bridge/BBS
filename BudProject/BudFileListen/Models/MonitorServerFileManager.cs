using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudFileListen.DBInterface;
using BudFileListen.DBService;
using BudFileListen.Entities;

namespace BudFileListen.Models
{
    public class MonitorServerFileManager
    {
        private IMonitorServerFileService monitorServerFileService = null;

        public MonitorServerFileManager() : this(new MonitorServerFileService()) { }

        public MonitorServerFileManager(IMonitorServerFileService monitorServerFileService)
        {
            this.monitorServerFileService = monitorServerFileService;
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Insert(Entities.monitorServerFile monitorServerFile)
        {
            this.monitorServerFileService.Insert(monitorServerFile);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorServerFolder"></param>
        public void Edit(Entities.monitorServerFile monitorServerFile)
        {
            this.monitorServerFileService.Edit(monitorServerFile);
        }
        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="monitorServerID"></param>
        public void Del(int monitorServerID)
        {
            this.monitorServerFileService.Del(monitorServerID);
        }
    }
}
