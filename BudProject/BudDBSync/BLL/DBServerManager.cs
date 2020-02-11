using System;
using System.Collections.Generic;
using System.Data;
using BudDBSync.Model;
using BudDBSync.DAL.XML;
using BudDBSync.DAL.SQLSERVER;
using System.Configuration;
using BudDBSync.Util;

namespace BudDBSync.BLL
{
    /// <summary>
    /// manage the configuration of dbserver . xiecongwen
    /// </summary>
    class DBServerManager
    {
        
        private DBXMLDAO dal = new DBXMLDAO();
        #region データベース元
        /// <summary>
        /// データベース元
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllSourceInfo()
        {
           
            return dal.GetAllSourceInfo();
        }

        public List<DBServer> GetAllSourceInfolist()
        {
            List<DBServer> list = new List<DBServer>();
            DataTable dt = GetAllSourceInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DBServer()
                    {
                        ServerName = row["ServerName"].ToString(),
                        LoginName = row["LoginName"].ToString(),
                        Password = row["Password"].ToString(),
                        DatabaseName = row["DatabaseName"].ToString(),
                    });
                }
            }

            return list;
        }

        public DBServer GetSourceInfoByIndex(int index)
        {
            DBServer dbserver = default(DBServer);
            DataTable dt = GetAllSourceInfo();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[index];
                dbserver = new DBServer() {
                    ServerName = row["ServerName"].ToString(),
                    LoginName = row["LoginName"].ToString(),
                    Password = row["Password"].ToString(),
                    DatabaseName = row["DatabaseName"].ToString(),
                };
            }
            return dbserver;
        }

        public bool AddSourceDBServer(DBServer dbserver)
        {
            
            return dal.AddDBServer(dbserver);
           
        }

        public void DeleteSourceDBServerAt(int index)
        {
            dal.DeleteSourceDBServerAt(index);
        }

        public bool UpdateSourceDBServer(int index,DBServer dbserver)
        {
            return dal.UpdateSourceDBServer(index, dbserver);
        }
        #endregion

        #region データベース先
        /// <summary>
        /// データベース先
        /// </summary>
        /// <returns></returns>
        public DBServer GetTargetInfo()
        {
            DBServer dbserver = new DBServer();
            try
            {
                string connstr = string.Empty;
                ConnectionStringSettings connss = System.Configuration.ConfigurationManager.ConnectionStrings["targetdb"];
                if (connss != null)
                {
                    connstr = connss.ConnectionString;
                    //待完善：应使用正则表达式校验格式,防止用户收到配置错误
                    string[] paras = connstr.Split(';');
                    dbserver.ServerName = paras[0].Split('=')[1];
                    dbserver.LoginName = paras[1].Split('=')[1];
                    dbserver.Password = paras[2].Split('=')[1];
                    dbserver.DatabaseName = paras[3].Split('=')[1];
                }

            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }
            return dbserver;
        }

        public bool SetTargetInfo(DBServer dbserver)
        {
            bool status = true;
            try
            {
                string connstr = string.Format("server={0};uid={1};pwd={2};database={3};", dbserver.ServerName, dbserver.LoginName, dbserver.Password, dbserver.DatabaseName);
                ConnectionStringSettings connss = ConfigurationManager.ConnectionStrings["targetdb"];

                // 打开可执行的配置文件*.exe.config 
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringSettings mySettings = new ConnectionStringSettings("targetdb", connstr, "System.Data.SqlClient");
                if (connss != null)
                {
                    config.ConnectionStrings.ConnectionStrings.Remove("targetdb"); 
                }
                // 将新的连接串添加到配置文件中. 
                config.ConnectionStrings.ConnectionStrings.Add(mySettings);
                // 保存对配置文件所作的更改 
                config.Save(ConfigurationSaveMode.Modified); 
                System.Configuration.ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                status = false;
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }
            return status;
        }
        #endregion

        #region link test
        /// <summary>
        /// test the server 
        /// </summary>
        /// <param name="dbserver"></param>
        /// <returns></returns>
        public bool LinkTest(DBServer dbserver)
        {
            DBHelper dbhelper = new DBHelper();
            return dbhelper.LinkTest(dbserver);
            
        }
        #endregion
    }
}
