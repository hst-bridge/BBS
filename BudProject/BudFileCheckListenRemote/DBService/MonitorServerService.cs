using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using BudFileCheckListen.Entities;
using System.Data.SqlClient;
using BudFileCheckListen.Common;
using log4net;
using System.Reflection;

namespace BudFileCheckListen.DBService
{
    public class MonitorServerService
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MonitorServerService()
        {
        }

        /// <summary>
        /// 監視リスト取得
        /// </summary>
        /// <returns></returns>
        public List<monitorServer> GetMonitorServerList()
        {
            List<monitorServer> monitorServerList = new List<monitorServer>();
            try
            {
                string sql = @"select * from monitorServer where deleteFlg = 0 order by id asc";
                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, null))
                {
                    while (reader.Read())
                    {
                        monitorServer _monitorServer = new monitorServer();
                        _monitorServer.id = int.Parse(reader["id"].ToString());
                        _monitorServer.monitorServerName = reader["monitorServerName"].ToString();
                        _monitorServer.monitorServerIP = reader["monitorServerIP"].ToString();
                        _monitorServer.monitorSystem = short.Parse(reader["monitorSystem"].ToString());
                        _monitorServer.account = reader["account"].ToString();
                        _monitorServer.password = reader["password"].ToString();
                        _monitorServer.startFile = reader["startFile"].ToString();
                        _monitorServer.monitorDrive = reader["monitorDrive"].ToString();
                        _monitorServer.monitorMacPath = reader["monitorMacPath"].ToString();
                        _monitorServer.monitorLocalPath = reader["monitorLocalPath"].ToString();
                        monitorServerList.Add(_monitorServer);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return monitorServerList;
        }

        /// <summary>
        /// 顺带获取关联信息
        /// </summary>
        /// <returns></returns>
        public List<monitorServer> GetMonitorServerListWithAttached()
        {
            List<monitorServer> monitorServerList = new List<monitorServer>();
            try
            {
                //此处sql语句保证每个monitorserver只有一条信息
                string sql = @"select *
from(
select ms.*,bsg.id as backupServerGroupID,bsgd.backupServerID,'\\'+backupServer.backupServerIP+'\'+backupServer.startFile as remotePath,ROW_NUMBER() over (PARTITION by ms.id order by ms.id) rowno
from 
	monitorServer ms
left join backupServerGroup bsg on bsg.deleteFlg=0 and ms.id=bsg.monitorServerID 
left join backupServerGroupDetail bsgd on bsgd.deleteFlg=0 and bsgd.backupServerGroupID=bsg.id
inner join backupServer on backupServer.id=bsgd.backupServerID

where ms.deleteFlg=0 
) tmp
where tmp.rowno = 1";

                using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.ConnectionString,
                    CommandType.Text, sql, null))
                {
                    while (reader.Read())
                    {
                        monitorServer _monitorServer = new monitorServer();
                        _monitorServer.id = int.Parse(reader["id"].ToString());
                        _monitorServer.monitorServerName = reader["monitorServerName"].ToString();
                        _monitorServer.monitorServerIP = reader["monitorServerIP"].ToString();
                        _monitorServer.monitorSystem = short.Parse(reader["monitorSystem"].ToString());
                        _monitorServer.account = reader["account"].ToString();
                        _monitorServer.password = reader["password"].ToString();
                        _monitorServer.startFile = reader["startFile"].ToString();
                        _monitorServer.monitorDrive = reader["monitorDrive"].ToString();
                        _monitorServer.monitorMacPath = reader["monitorMacPath"].ToString();
                        //此处改为远端服务器的UNC路径 201412162316 xiecongwen
                        _monitorServer.monitorLocalPath = reader["remotePath"].ToString();
                        _monitorServer.backupServerGroupID = Convert.ToInt32(reader["backupServerGroupID"]);
                        _monitorServer.backupServerID = Convert.ToInt32(reader["backupServerID"]);
                        monitorServerList.Add(_monitorServer);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return monitorServerList;
        }
    }
}
