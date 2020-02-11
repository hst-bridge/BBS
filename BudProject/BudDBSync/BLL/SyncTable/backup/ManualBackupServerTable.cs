using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable.backup
{
    class ManualBackupServerTable : ITable
    {
        public string SourceSql(int ID)
        {
            
                return "select top " + Properties.Settings.Default.TopNumber + @"
[id]
           ,[serverIP]
           ,[account]
           ,[password]
           ,[drive]
           ,[startFile]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate]
           ,[restorer]
           ,[restoreDate]
,[synchronismFlg]
from [manualBackupServer] where synchronismFlg=0 and id>{0} order by id";
            
        }

        private string targetSql
        {
            get {
                return @"INSERT INTO [manualBackupServer]
           (id
,[DBServerIP]
           ,[serverIP]
           ,[account]
           ,[password]
           ,[drive]
           ,[startFile]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate]
           ,[restorer]
           ,[restoreDate])
     VALUES
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');";
            }
        }
        public string TargetSql(object[] args)
        {
            return string.Format(targetSql, args);
        }

        public string UpdateSourceSql
        {
            get
            {
                return "update [manualBackupServer] set synchronismFlg=1 where id in (";
            }
        }

        public string DeleteOldDataSql
        {
            get
            {
                return "";
            }
        }
    }
}
