using System;
using BudDBSync.Model;
using System.Data.SqlClient;
using BudDBSync.Util;

namespace BudDBSync.DAL.SQLSERVER
{
    /// <summary>
    /// help
    /// </summary>
    class DBHelper
    {

        /// <summary>
        /// test the server is linked?
        /// </summary>
        /// <param name="dbserver"></param>
        /// <returns></returns>
        public bool LinkTest(DBServer dbserver)
        {
            bool status = false;
            string connstr = string.Format("server={0};uid={1};pwd={2};database={3};",dbserver.ServerName,dbserver.LoginName,dbserver.Password,dbserver.DatabaseName);
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                try
                {
                    connection.Open();
                    status = true;
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogFile.Error,ex.Message);
                    status = false;
                }
            }
            return status;
        }
    }
}
