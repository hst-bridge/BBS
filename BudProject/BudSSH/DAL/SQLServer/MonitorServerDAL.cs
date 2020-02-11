using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model.Behind;
using System.Data.SqlClient;
using BudSSH.Model;
using BudSSH.Common.Helper;
using System.Data;

namespace BudSSH.DAL.SQLServer
{
    /// <summary>
    /// 用于获取MonitorServer 相关信息
    /// </summary>
    class MonitorServerDAL
    {
        /// <summary>
        /// 获取符合配置的MonitorServer
        /// </summary>
        /// <returns></returns>
        public MonitorServer GetMonitorServer(Model.Config config,ErrorPathInfo epi)
        {
            MonitorServer ms = null;
            string connstr = config.DB.ConnString;
            string sql = "select top 1 * from [monitorServer] where [deleteFlg]=0 and charindex([monitorDrive],@monitorDrive)>0";

            SqlParameter sp = new SqlParameter("@monitorDrive",epi.source);
            
            DataSet ds = DBHelper.ExecuteDataset(connstr, CommandType.Text, sql, sp);
            DataTable dt = null;
            if (ds != null && ds.Tables.Count == 1) dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ms = new MonitorServer()
                {
                    //monitorServerName unique
                    monitorServerName = Convert.ToString(dr["monitorServerName"]),
                    monitorServerIP = Convert.ToString(dr["monitorServerIP"]),
                    account = Convert.ToString(dr["account"]),
                    password = Convert.ToString(dr["password"]),
                    startFile = Convert.ToString(dr["startFile"]),
                    monitorDrive = Convert.ToString(dr["monitorDrive"]),
                    monitorMacPath = Convert.ToString(dr["monitorMacPath"]),
                    monitorLocalPath = Convert.ToString(dr["monitorLocalPath"]),
                };
            }
            return ms;
        }

        /// <summary>
        /// 获取所有MonitorServer
        /// </summary>
        /// <returns></returns>
        public List<MonitorServer> GetAllMonitorServer(Model.Config config)
        {
            List<MonitorServer> list = new List<MonitorServer>();

            string sql = "select * from [monitorServer] where [deleteFlg]=0 ";
            DataSet ds = DBHelper.ExecuteDataset(config.DB.ConnString, CommandType.Text, sql);
            DataTable dt = null;
            if (ds != null && ds.Tables.Count == 1) dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                //获取数据
                foreach (DataRow dr in dt.Rows)
                {
                    MonitorServer ms = new MonitorServer()
                    {
                        //monitorServerName unique
                        monitorServerName = Convert.ToString(dr["monitorServerName"]),
                        monitorServerIP = Convert.ToString(dr["monitorServerIP"]),
                        account = Convert.ToString(dr["account"]),
                        password = Convert.ToString(dr["password"]),
                        startFile = Convert.ToString(dr["startFile"]),
                        monitorDrive = Convert.ToString(dr["monitorDrive"]),
                        monitorMacPath = Convert.ToString(dr["monitorMacPath"]),
                        monitorLocalPath = Convert.ToString(dr["monitorLocalPath"]),
                    };

                    list.Add(ms);
                }
            }

            return list;
        }
    }
}
