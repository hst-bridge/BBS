using System;
using System.Collections.Generic;
using System.Linq;
using BudBackupCopy.DBInterface;
using System.Data;
using BudBackupCopy.Entities;

namespace BudBackupCopy.DBService
{
    public class MonitorServerFolderService : IMonitorServerFolderService
    {
        private BudBackup2Context db = null;

        public MonitorServerFolderService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// バックアップ設定のキー検索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Entities.monitorServerFolder GetById(int id)
        {
            Entities.monitorServerFolder monitorServerFolder = db.monitorServerFolders.Find(id);
            return monitorServerFolder;
        }

        /// <summary>
        /// 監視対象の設定全体取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> GetByAllMonitorObject(int monitorServerID, int initFlg, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraInitFlg = " initFlg = " + initFlg;

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorServerFolder  where {0} and {1} and {2}", paraMonitorServerID, paraInitFlg, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.monitorServerFolder>(strsql, new Object[] { }).ToList<Entities.monitorServerFolder>();

            return result;
        }

        /// <summary>
        /// 監視対象取得
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> GetByMonitorObject(int monitorServerID, string monitorFileType, int initFlg, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorFileType = string.IsNullOrEmpty(monitorFileType) ? " 1=1 " : " monitorFileType = " + "'" + monitorFileType + "'";

            string paraInitFlg = " initFlg = " + initFlg;

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorServerFolder  where {0} and {1} and {2} and {3}", paraMonitorServerID, paraMonitorFileType, paraInitFlg, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.monitorServerFolder>(strsql, new Object[] { }).ToList<Entities.monitorServerFolder>();

            return result;
        }

        /// <summary>
        /// バックアップ設定の検索
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="initFlg"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> GetByMonitorTopFolder(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, string initFlg, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorFileName = " monitorFileName = " + "'" + monitorFileName + "'";

            string paraMonitorFilePath = string.IsNullOrEmpty(monitorFilePath) ? " 1=1 " : " monitorFilePath = " + "N'" + monitorFilePath + "'";

            string paraMonitorFileType = string.IsNullOrEmpty(monitorFileType) ? " 1=1 " : " monitorFileType = " + "'" + monitorFileType + "'";

            string paraInitFlg = " initFlg = " + initFlg;

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorServerFolder  where {0} and {1} and {2} and {3} and {4} and {5}", paraMonitorServerID, paraMonitorFileName, paraMonitorFilePath, paraMonitorFileType, paraInitFlg, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.monitorServerFolder>(strsql, new Object[] { }).ToList<Entities.monitorServerFolder>();

            return result;
        }

        /// <summary>
        /// バックアップ設定の検索
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorFilePath"></param>
        /// <param name="monitorFileType"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.monitorServerFolder> Search(int monitorServerID, string monitorFileName, string monitorFilePath, string monitorFileType, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraMonitorFileName = string.IsNullOrEmpty(monitorFileName) ? " 1=1 " : " monitorFileName = " + "N'" + monitorFileName + "'";

            string paraMonitorFilePath = string.IsNullOrEmpty(monitorFilePath) ? " 1=1 " : " monitorFilePath = " + "N'" + monitorFilePath + "'";

            string paraMonitorFileType = " monitorFileType = " + "'" + monitorFileType + "'";

            string paraDeleteFlg = " deleteFlg =" + deleteFlg;

            string strsql = string.Format("select * from dbo.monitorServerFolder  where {0} and {1} and {2} and {3} and {4}", paraMonitorServerID, paraMonitorFileName, paraMonitorFilePath, paraMonitorFileType, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.monitorServerFolder>(strsql, new Object[] { }).ToList<Entities.monitorServerFolder>();

            return result;
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Insert(Entities.monitorServerFolder monitorserverfolder)
        {
            db.monitorServerFolders.Add(monitorserverfolder);
            db.SaveChanges();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        public void Edit(Entities.monitorServerFolder monitorserverfolder)
        {
            db.Entry(monitorserverfolder).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        public void Del(int id)
        {
            monitorServerFolder monitorserverfolder = db.monitorServerFolders.Find(id);
            db.monitorServerFolders.Remove(monitorserverfolder);
            db.SaveChanges();
        }
    }
}
