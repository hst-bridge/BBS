using System;
using System.Collections.Generic;
using System.Linq;
using BudFileListen.DBInterface;
using System.Data;
using BudFileListen.Entities;

namespace BudFileListen.DBService
{
    public class FileTypeSetService : IFileTypeSetService
    {
        private BudBackup2Context db = null;

        public FileTypeSetService()
        {
            db = new BudBackup2Context();
        }

        /// <summary>
        /// 除外条件の抽出
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public List<Entities.fileTypeSet> GetByFileTypeSet(int monitorServerID, int deleteFlg)
        {
            string paraMonitorServerID = " monitorServerID =" + monitorServerID;

            string paraDeleteFlg = " deleteFlg = " + deleteFlg;

            string strsql = string.Format("select * from dbo.fileTypeSet where {0} and {1}", paraMonitorServerID, paraDeleteFlg);

            var result = db.Database.SqlQuery<Entities.fileTypeSet>(strsql, new Object[] { }).ToList<Entities.fileTypeSet>();

            return result;
        }
    }
}
