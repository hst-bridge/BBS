using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    class MonitorServerTable : ITable
    {
        public string SourceSql(int ID)
        {
                return "select top " + Properties.Settings.Default.TopNumber + @"
[id]
 ,[monitorServerName]
 ,[monitorServerIP]
 ,[monitorSystem]
 ,[memo]
 ,[account]
 ,[password]
 ,[startFile]
 ,[monitorDrive]
 ,[monitorDriveP]
 ,[monitorMacPath]
 ,[monitorLocalPath]
 ,[copyInit]
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
from [monitorServer] " + TableHelper.GetWhereCondCommon(ID);
        }

        private string targetSql
        {
            get {
                return @"INSERT INTO [monitorServer]
           (
id
,[DBServerIP]
           ,[monitorServerName]
           ,[monitorServerIP]
           ,[monitorSystem]
           ,[memo]
           ,[account]
           ,[password]
           ,[startFile]
           ,[monitorDrive]
           ,[monitorDriveP]
           ,[monitorMacPath]
           ,[monitorLocalPath]
           ,[copyInit]
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
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}');";
            }
        }
        public string TargetSql(object[] args)
        {
            string sql = string.Empty;
            try
            {

                sql = string.Format(" delete from monitorServer where id={0} and DBServerIP='{1}'; ", args[0], args[1]);
                
                    //插入语句
                    sql+=string.Format(targetSql, args);

            }
            catch (Exception)
            { throw; }


            return sql;
        }
        public string UpdateSourceSql
        {
            get
            {
                return "update [monitorServer] set synchronismFlg=1 where id in (";
            }
        }

        public override string ToString()
        {
            return "MonitorServerTable";
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
