using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BudFileCheckListen.Entities;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using BudFileCheckListen.Common;


namespace BudFileCheckListen.DBService
{
    public class BackupServerGroupService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BackupServerGroupService()
        {
        }

        /// <summary>
        /// グループ情報取得
        /// </summary>
        /// <returns></returns>
        public backupServerGroup GetBackupServerGroup(int monitorServerID)
        {
            backupServerGroup _backupServerGroup = null;
            try
            {
                string sql = @"select * from backupServerGroup where monitorServerID = @monitorServerID and deleteFlg = 0";
                SqlParameter para = new SqlParameter("@monitorServerID", monitorServerID);

                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                   CommandType.Text, sql, para))
                {
                    if (reader.Read())
                    {
                        _backupServerGroup = new backupServerGroup();
                        _backupServerGroup.id = int.Parse(reader["id"].ToString());
                        _backupServerGroup.backupServerGroupName = reader["backupServerGroupName"].ToString();
                        _backupServerGroup.monitorServerID = int.Parse(reader["monitorServerID"].ToString());
                        _backupServerGroup.deleteFlg = short.Parse(reader["deleteFlg"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return _backupServerGroup;
        }
    }
}
