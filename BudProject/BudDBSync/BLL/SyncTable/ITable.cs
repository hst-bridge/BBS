using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    interface ITable
    {
        string SourceSql(int ID);
        string TargetSql(object[] args);

        string UpdateSourceSql { get; }

        /// <summary>
        /// 用于删除旧数据的sql语句
        /// </summary>
        string DeleteOldDataSql { get; }
    }
}
