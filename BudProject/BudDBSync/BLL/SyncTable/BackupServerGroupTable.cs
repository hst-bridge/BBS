using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    class BackupServerGroupTable : ITable
    {
        public string SourceSql(int ID)
        {
            
                return "select top " + Properties.Settings.Default.TopNumber + @"
 [id]
,[backupServerGroupName]
,[monitorServerID]
,[memo]
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
from [backupServerGroup] " + TableHelper.GetWhereCondCommon(ID);
            
        }

        private string targetSql
        {
            get {
                return @"INSERT INTO [backupServerGroup]
           (id
            ,[DBServerIP]
           ,[backupServerGroupName]
           ,[monitorServerID]
           ,[memo]
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
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}');";
            }
        }

        public string TargetSql(object[] args)
        {
//            string where = " where [DBServerIP]='" + args[1] + "' and id=" + args[0];
//            StringBuilder sb = new StringBuilder("if exists (select id from [backupServerGroup] "+where);
//            sb.Append( @")
//begin 
//   update [backupServerGroup] set deleteFlg="+args[5]+where+@"  
//   end 
//   else
//  ").Append(string.Format(targetSql, args));
//            return sb.ToString();

            //copied from MonitorServerTable.cs line 73 by wjd 2014-12-02
            string sql = string.Empty;
            try
            {

                sql = string.Format(" delete from backupServerGroup where id={0} and DBServerIP='{1}'; ", args[0], args[1]);

                //插入语句
                sql += string.Format(targetSql, args);

            }
            catch (Exception)
            { throw; }


            return sql;
        }

        public string UpdateSourceSql
        {
            get
            {
                return "update [backupServerGroup] set synchronismFlg=1 where id in (";
            }
        }

        public override string ToString()
        {
            return "BackupServerGroupTable";
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
