using System;
using System.Collections.Generic;
using System.Linq;
using BudFileListen.DBInterface;
using System.Data;
using BudFileListen.Entities;

namespace BudFileListen.DBService
{
    public class MonitorServerFileService : IMonitorServerFileService
    {
        private BudBackup2Context db = null;

        public MonitorServerFileService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Insert(Entities.monitorServerFile monitorserverfile)
        {
            db.monitorServerFiles.Add(monitorserverfile);
            db.SaveChanges();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Edit(Entities.monitorServerFile monitorserverfile)
        {
            db.Entry(monitorserverfile).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="cust_no"></param>
        public void Del(int monitorServerID)
        {
            monitorServerFile monitorserverfile = db.monitorServerFiles.Find(monitorServerID);
            db.monitorServerFiles.Remove(monitorserverfile);
            db.SaveChanges();
        }
    }
}
