using System;
using System.Collections.Generic;
using System.Linq;
using BudErrorManage.DBInterface;
using System.Data;
using BudErrorManage.Entities;

namespace BudErrorManage.DBService
{
    public class LogService : ILogService
    {
        private BudBackup2Context db = null;

        public LogService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// Logデータ
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorTime"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.log> GetLogData(int monitorServerID, string monitorFileName, string monitorFilePath, int deleteFlg)
        {
            //
            //if (monitorFileName.Equals("map_神田サロン1003*.ai"))
            //{
            //    Console.Write("OK");
            //}

            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorFileName = " monitorFileName = " + "N'" + monitorFileName + "'";

            string paraMonitorFilePath = string.IsNullOrEmpty(monitorFilePath) ? " 1=1 " : " monitorFilePath = " + "N'" + monitorFilePath + "'";

            //string paraMonitorTime = " convert(nvarchar, monitorTime, 120) = " + "'" + monitorTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from log  where {0} and {1} and {2} and {3}",
                paraMonitorServerID, paraMonitorFileName, paraMonitorFilePath, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.log>(strsql, new Object[] { }).ToList<Entities.log>();

            return result;
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="log"></param>
        public void Insert(Entities.log log)
        {
            db.logs.Add(log);
            db.SaveChanges();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="log"></param>
        public void Edit(Entities.log log)
        {
            db.Entry(log).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        public void Del(int id)
        {
            log log = db.logs.Find(id);
            db.logs.Remove(log);
            db.SaveChanges();
        }
    }
}
