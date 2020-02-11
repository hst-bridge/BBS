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
    public class BackupServerGroupDetailService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BackupServerGroupDetailService()
        {
        }

        /// <summary>
        /// サーバリスト情報取得
        /// </summary>
        /// <returns></returns>
        public List<backupServerGroupDetail> GetBackupServerGroupDetailList(int backupServerGroupID)
        {
            List<backupServerGroupDetail> backupServerGroupDetailList = new List<backupServerGroupDetail>();
            try
            {
                string sql = @"select * from backupServerGroupDetail where backupServerGroupID = @backupServerGroupID and deleteFlg = 0";

                SqlParameter para = new SqlParameter("@backupServerGroupID", backupServerGroupID);

                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, para))
                {
                    while (reader.Read())
                    {
                        backupServerGroupDetail _backupServerGroupDetail = new backupServerGroupDetail();
                        _backupServerGroupDetail.id = int.Parse(reader["id"].ToString());
                        _backupServerGroupDetail.backupServerGroupID = int.Parse(reader["backupServerGroupID"].ToString());
                        _backupServerGroupDetail.backupServerID = int.Parse(reader["backupServerID"].ToString());
                        _backupServerGroupDetail.deleteFlg = short.Parse(reader["deleteFlg"].ToString());
                        backupServerGroupDetailList.Add(_backupServerGroupDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return backupServerGroupDetailList;
        }
    }
}
