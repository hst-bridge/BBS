using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    class LogTable : ITable
    {
        public string SourceSql(int ID)
        {
            
                return "select top " + Properties.Settings.Default.TopNumber + @" 
[id]
,[monitorServerID]
,[monitorFileName]
,[monitorFileStatus]
,[monitorFilePath]
,[monitorFileType]
,[monitorFileSize]
,[monitorTime]
,[transferFlg]
,[backupServerGroupID]
,[backupServerID]
,[backupServerFileName]
,[backupServerFilePath]
,[backupServerFileType]
,[backupServerFileSize]
,[backupStartTime]
,[backupEndTime]
,[backupTime]
,[backupFlg]
,[copyStartTime]
,[copyEndTime]
,[copyTime]
,[copyFlg]
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
from [log] " + TableHelper.GetWhereCond(ID);
            
        }

        private string targetSql
        {
            get
            {
                return @"INSERT INTO [log]
 
          (
id
,[DBServerIP]
,[monitorServerID]
,[monitorFileName]
,[monitorFileStatus]
,[monitorFilePath]
,[monitorFileType]
,[monitorFileSize]
,[monitorTime]
,[transferFlg]
,[backupServerGroupID]
,[backupServerID]
,[backupServerFileName]
,[backupServerFilePath]
,[backupServerFileType]
,[backupServerFileSize]
,[backupStartTime]
,[backupEndTime]
,[backupTime]
,[backupFlg]
,[copyStartTime]
,[copyEndTime]
,[copyTime]
,[copyFlg]
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
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}');";
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
                return "update [log] set synchronismFlg=1 where id in (";
            }
        }

        public int ID { get; set; }

        public override string ToString()
        {
            return "LogTable";
        }

        /// <summary>
        /// Log表：删除一个月以前的
        /// </summary>
        public string DeleteOldDataSql
        {
            get
            {
                return "delete from [log] where [backupStartTime] < '" + DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd") + "';";
            }
        }
    }
}
