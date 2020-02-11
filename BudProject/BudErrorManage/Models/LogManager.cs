using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudErrorManage.DBInterface;
using BudErrorManage.DBService;
using BudErrorManage.Entities;

namespace BudErrorManage.Models
{
    public class LogManager
    {
        private ILogService logService = null;

        public LogManager() : this(new LogService()) { }

        public LogManager(ILogService logService)
        {
            this.logService = logService;
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
            return this.logService.GetLogData(monitorServerID, monitorFileName, monitorFilePath, deleteFlg);
        }

        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="log"></param>
        public void Insert(Entities.log log)
        {
            this.logService.Insert(log);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="log"></param>
        public void Edit(Entities.log log)
        {
            this.logService.Edit(log);
        }
        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        public void Del(int id)
        {
            this.logService.Del(id);
        }
    }
}
