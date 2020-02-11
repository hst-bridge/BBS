using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using budbackup.CommonWeb;
using budbackup.CommonWeb.Helper;
using System.Data;
using System.Reflection;
using budbackup.Models;
using budbackup.CommonWeb.Util;
using DBUtility;

namespace budbackup.DAL
{
    public class FileDownloadDAL
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// get the search reader
        /// </summary>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorServerID"></param>
        /// <returns></returns>
        public SqlDataReader GetSearchReader(string monitorFileName, string msidCond)
        {
            SqlDataReader sdr = null;
            try
            {
                string connstr = ConfigUtil.AppSetting("ConnStringFordownload");
                if (string.IsNullOrWhiteSpace(connstr)) return null;

                string sql = "select [monitorServerID],[monitorServerIP],[sharePoint],[monitorLocalPath],[monitorFileName],[monitorFileRelativeFullPath],[updateDate] from [dbo].[monitorFileListen] where deleteFlg=0 ";
                if (!string.IsNullOrWhiteSpace(msidCond))
                    sql += " and [monitorServerID]  " + msidCond;
                
                sql += " and monitorFileRelativeFullPath like N'%" + monitorFileName + "%'";
                sdr = DBHelper.ExecuteReader(connstr, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));

            }
            return sdr;
        }

        /// <summary>
        /// get the search reader
        /// </summary>
        /// <param name="monitorFileName"></param>
        /// <param name="monitorServerID"></param>
        /// <returns></returns>
        public SqlDataReader GetSearchReaderPathBk(string monitorFileName, string msidCond)
        {
            SqlDataReader sdr = null;
            try
            {
                string connstr = ConfigUtil.AppSetting("ConnStringFordownload");
                if (string.IsNullOrWhiteSpace(connstr)) return null;

                string sql = "select msid as [monitorServerID],[relativePath] as [monitorFileRelativeFullPath] from [dbo].[pathBk] where 1=1  ";
                if (!string.IsNullOrWhiteSpace(msidCond))
                    sql += " and [msid]  " + msidCond;
                
                sql += " and relativePath like N'%" + monitorFileName + "%'";
                sdr = DBHelper.ExecuteReader(connstr, CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));

            }
            return sdr;
        }
        /// <summary>
        /// get monitor server list
        /// </summary>
        /// <param name="dbServerip"></param>
        /// <param name="monitorServerID"></param>
        /// <returns></returns>
        public List<MonitorServer> GetMSList(string dbServerip,string msidCond)
        {
            List<MonitorServer> list = new List<MonitorServer>();
            try
            {
                string connstr = ConfigUtil.AppSetting("ConnStringFordownload");
                if (string.IsNullOrWhiteSpace(connstr)) return null;

                string sql = "select [id],'" + dbServerip + "' as [DBServerIP],[monitorServerIP],[startFile],[monitorLocalPath] from [dbo].[monitorServer] where [deleteFlg] = 0  ";
                if(!string.IsNullOrWhiteSpace(msidCond))
                    sql += " and id "+ msidCond;

                DataSet ds = DBHelper.ExecuteDataset(connstr, CommandType.Text, sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new MonitorServer()
                            {
                                ID = Convert.ToString(row["id"]),
                                DBServerIP = Convert.ToString(row["DBServerIP"]),
                                MonitorLocalPath = Convert.ToString(row["monitorLocalPath"]),
                                MonitorServerIP = Convert.ToString(row["monitorServerIP"]),
                                StartFile = Convert.ToString(row["startFile"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                throw;
            }

            return list;
        }

       

        
        /// <summary>
        /// 获取相对应的远端monitorserver list
        /// </summary>
        /// <returns></returns>
        public List<Models.MonitorServer> GetRemoteMSList(string dbServerip)
        {
            List<Models.MonitorServer> list = new List<Models.MonitorServer>();
            try
            {
                string connstr = ConfigUtil.AppSetting("ConnStringFordownload");
                if (string.IsNullOrWhiteSpace(connstr)) return null;

                string sql = "select [id],'" + dbServerip + "' as [DBServerIP],[monitorLocalPath],[monitorServerName] from [dbo].[monitorServer] where [deleteFlg] = 0  ";

                DataSet ds = DBHelper.ExecuteDataset(connstr, CommandType.Text, sql);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new MonitorServer() {
                                 ID = Convert.ToString(row["id"]),
                                 DBServerIP = Convert.ToString(row["DBServerIP"]),
                                 MonitorLocalPath = Convert.ToString(row["monitorLocalPath"]),
                                 MonitorServerName = Convert.ToString(row["monitorServerName"]),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
                throw;
            }

            return list;
        }
    }
}