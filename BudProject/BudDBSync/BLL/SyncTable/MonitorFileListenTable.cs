using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    class MonitorFileListenTable : ITable
    {
        public string SourceSql(int ID)
        {
                return "select top " + Properties.Settings.Default.TopNumber + @"
[id]
,[monitorServerID]
           ,[monitorFileName]
           ,[monitorType]
           ,[monitorServerIP]
           ,[sharePoint]
           ,[monitorLocalPath]
           ,[monitorFileRelativeDirectory]
           ,[monitorFileRelativeFullPath]
           ,[monitorFileLastWriteTime]
           ,[monitorFileSize]
           ,[monitorFileExtension]
           ,[monitorFileCreateTime]
           ,[monitorFileLastAccessTime]
           ,[monitorStatus]
           ,[monitorFileStartTime]
           ,[monitorFileEndTime]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate]
,[synchronismFlg]
from [monitorFileListen] " + TableHelper.GetWhereCondCommon(ID);
        }

        private string targetSql
        {
            get {
                return @"INSERT INTO [monitorFileListen]
           (id
,[DBServerIP]
           ,[monitorServerID]
           ,[monitorFileName]
           ,[monitorType]
           ,[monitorServerIP]
           ,[sharePoint]
           ,[monitorLocalPath]
           ,[monitorFileRelativeDirectory]
           ,[monitorFileRelativeFullPath]
           ,[monitorFileLastWriteTime]
           ,[monitorFileSize]
           ,[monitorFileExtension]
           ,[monitorFileCreateTime]
           ,[monitorFileLastAccessTime]
           ,[monitorStatus]
           ,[monitorFileStartTime]
           ,[monitorFileEndTime]
           ,[deleteFlg]
           ,[deleter]
           ,[deleteDate]
           ,[creater]
           ,[createDate]
           ,[updater]
           ,[updateDate])
     VALUES
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}');";
            }
        }
        public string TargetSql(object[] args)
        {
            string sql = string.Empty;
            try
            {
                //判断deleteFlg 是否为1 如果为1 则执行删除
                if (Convert.ToInt32(args[18]) == 1)
                {
                    sql = string.Format(" delete from monitorFileListen where id={0} and DBServerIP='{1}'; ", args[0], args[1]);
                }
                else
                {
                    /*
                     * 不为1 则为创建或更新 判断当前是否存在相应记录，如果存在，则执行更新
                     *       如果不存在 则插入
                     */
                    string where = string.Format(@" where id={0} and DBServerIP='{1}'",
                                                     args[0], args[1]);
                    StringBuilder sb = new StringBuilder(" if exists(select id from [monitorFileListen] " + where + " )");
                    //添加更新
                    string update = string.Format(@"begin 
                              update monitorFileListen set [monitorType]='{0}',monitorFileSize='{1}',updateDate='{2}' ", args[4], args[11], args[24]);
                    sb.Append(update + where + @"                      
                          end 
                          else ");

                    //插入语句
                    sb.Append(string.Format(targetSql, args));

                    sql = sb.ToString();
                }
            }
            catch (Exception)
            { throw; }


            return sql;
        }
        public string UpdateSourceSql
        {
            get
            {
                return "update [monitorFileListen] set synchronismFlg=1 where id in (";
            }
        }
        public override string ToString()
        {
            return "MonitorFileListenTable";
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
