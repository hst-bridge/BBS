using System;
using System.Data;
using BudDBSync.Model;
using BudDBSync.Util;

namespace BudDBSync.DAL.XML
{
    class DBXMLDAO
    {
        static string xmlPath = string.Empty;
        public static string XMLPath
        {
            get
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(xmlPath)){
                      xmlPath = System.Configuration.ConfigurationManager.AppSettings["dbserverfile"];
                    }
                   
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogFile.Error,ex.Message);
                    xmlPath = "./config/DB.xml";
                }

                return xmlPath;
            }
        }
        /// <summary>
        /// データベース元
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllSourceInfo()
        {
            try
            {
                DataTable dt = default(DataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(XMLPath);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    dt.Rows.RemoveAt(0);
                }
                
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogFile.Error,ex.Message);
                throw;
            }
        }

        /// <summary>
        /// DBサーバーの追加
        /// </summary>
        /// <param name="dbserver"></param>
        public bool AddDBServer(DBServer dbserver)
        {
            bool status = false;
            try
            {
                DataTable dt = default(DataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(XMLPath);

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }

                //判断是否已经存在
                DataColumn[] key = new DataColumn[]{dt.Columns["ServerName"],dt.Columns["DatabaseName"]};
                dt.PrimaryKey = key;
                //dt.Rows.Find(

                dt.Rows.Add(new object[] { dbserver.ServerName,dbserver.LoginName,dbserver.Password,dbserver.DatabaseName });
                ds.WriteXml(XMLPath);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }

            return status;
        }

        public void DeleteSourceDBServerAt(int index)
        {
            try
            {
                DataTable dt = default(DataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(XMLPath);

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                   
                }
                dt.Rows.RemoveAt(index+1);
                ds.WriteXml(XMLPath);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }

        }

        public bool UpdateSourceDBServer(int index,DBServer dbserver)
        {
            bool status = false;
            try
            {
                DataTable dt = default(DataTable);
                DataSet ds = new DataSet();
                ds.ReadXml(XMLPath);

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    
                }
                DataRow editRow =  dt.Rows[index+1];
                editRow["ServerName"] = dbserver.ServerName;
                editRow["LoginName"] = dbserver.LoginName;
                editRow["Password"] = dbserver.Password;
                editRow["DatabaseName"] = dbserver.DatabaseName;
                dt.AcceptChanges();
                ds.WriteXml(XMLPath);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                LogManager.WriteLog(LogFile.Error,ex.Message);
            }

            return status;
        }
        
    }
}
