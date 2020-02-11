using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.DBInterface;
using System.Data;
using BudCopyListen.Entities;

namespace BudCopyListen.DBService
{
    public class MonitorFileListenService : IMonitorFileListenService
    {
        private BudBackup2Context db = null;

        public MonitorFileListenService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Entities.monitorFileListen GetById(int id)
        {
            Entities.monitorFileListen monitorFileListen = db.monitorFileListens.Find(id);
            return monitorFileListen;
        }

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<monitorFileListen> GetByAllFile(int monitorServerID, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorFileListen  where {0} and {1}", paraMonitorServerID, paraDeleteFlg);

            var result = db.Database.SqlQuery<monitorFileListen>(strsql, new Object[] { }).ToList<monitorFileListen>();

            return result;
        }

        /// <summary>
        /// 監視対象全体
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<string> GetByAllFileName(int monitorServerID, string monitorType, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorType = " monitorType =" + "'" + monitorType + "'";

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select monitorFileLocalPath from dbo.monitorFileListen  where {0} and {1} and {2}", paraMonitorServerID, paraMonitorType, paraDeleteFlg);

            var result = db.Database.SqlQuery<string>(strsql, new Object[] { }).ToList<string>();

            return result;
        }


        /// <summary>
        /// 監視対象のコピー中と更新中のリスト
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<monitorFileListen> GetByCopyDoingFileName(int monitorServerID, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorStatus = " (monitorStatus =" + "'" + "新規コピー中" + "'" + "or monitorStatus =" + "'" + "更新コピー中" + "')";

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorFileListen  where {0} and {1} and {2}", paraMonitorServerID, paraMonitorStatus, paraDeleteFlg);

            var result = db.Database.SqlQuery<monitorFileListen>(strsql, new Object[] { }).ToList<monitorFileListen>();

            return result;
        }

        /// <summary>
        /// 監視対象
        /// </summary> 
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileMD5"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorFileListen> GetByMonitorObject(int monitorServerID, string monitorFileMD5, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorFileMD5 = string.IsNullOrEmpty(monitorFileMD5) ? " 1=1 " : " monitorFileMD5 = " + "'" + monitorFileMD5 + "'";

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorFileListen  where {0} and {1} and {2}",
                paraMonitorServerID, paraMonitorFileMD5, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.monitorFileListen>(strsql, new Object[] { }).ToList<Entities.monitorFileListen>();

            return result;
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Insert(Entities.monitorFileListen monitorFileListen)
        {
            db.monitorFileListens.Add(monitorFileListen);
            db.SaveChanges();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Edit(Entities.monitorFileListen monitorFileListen)
        {
            db.Entry(monitorFileListen).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        public void Del(int id)
        {
            monitorFileListen monitorFileListen = db.monitorFileListens.Find(id);
            db.monitorFileListens.Remove(monitorFileListen);
            db.SaveChanges();
        }
    }
}
