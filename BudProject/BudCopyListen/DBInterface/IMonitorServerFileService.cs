using System;
using System.Collections.Generic;
using System.Linq;
using BudCopyListen.Entities;

namespace BudCopyListen.DBInterface
{
    public interface IMonitorServerFileService
    {
        /// <summary>
        /// インサート
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Insert(Entities.monitorServerFile monitorserverfile);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="monitorserverfolder"></param>
        void Edit(Entities.monitorServerFile monitorserverfile);

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="cust_no"></param>
        void Del(int monitorServerID);

    }
}
