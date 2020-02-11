using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using budbackup.Models;
using budbackup.CommonWeb;
using System.Collections;
using log4net;
using System.Reflection;
using budbackup.DAL;
using System.Data.SqlClient;
using System.IO;

namespace budbackup.BLL
{
    public class FileDownloadService
    {
        ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ServerShareInfo GetSSI()
        {
            ServerShareInfo ssi = null;

            //get the config
            string configValue = ConfigUtil.AppSetting("backupServerInfo");
            //please guarantee the correctness of the config 
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                string[] valueS = configValue.Split(';');
                if (valueS.Count() == 3)
                {
                    ssi = new ServerShareInfo();
                    foreach (string item in valueS)
                    {
                        string[] kv = item.Split('=');
                        if (kv.Count() == 2)
                        {
                            string key = kv[0];
                            switch (key)
                            {
                                case "UNCBase": ssi.UNCBase = kv[1]; break;
                                case "username": ssi.Username = kv[1]; break;
                                case "passwd": ssi.Passwd = kv[1]; break;
                            }
                        }
                    }
                }
            }

            return ssi;
        }

        public DownloadSearchResult GetMatchList(string searchPattern, string monitorServerID,int maxsize,string username)
        {
            SqlDataReader sdr = null;
            SqlDataReader sdrPathBk = null;
            DownloadSearchResult ds = null;
            //the max size
            try
            {
                #region get the monitor server id
                string msid = string.Empty;
                if (monitorServerID == "-1")
                {
                    msid = monitorServerID;
                }
                else
                {
                    msid = monitorServerID.Split('|')[1];
                }
                #endregion

                FileDownloadDAL fdal = new FileDownloadDAL();

                //get monitor Server
                string dbServerip = ConfigUtil.AppSetting("remoteIP");
                string msidCond = GetMsidCond(username, dbServerip, msid);

                List<MonitorServer> mslist = fdal.GetMSList(dbServerip, msidCond);
                if (mslist != null && mslist.Count > 0)
                {
                    //use this to check whether the folder exists
                    HashSet<string> folderSet = new HashSet<string>();
                    HashSet<string> fileSet = new HashSet<string>();
                    #region  from filelisten
                    #region get reader
                    sdr = fdal.GetSearchReader(searchPattern, msidCond);
                    #endregion

                    #region get the match list
                    if (sdr != null)
                    {
                        

                        ds = new DownloadSearchResult();
                        while (sdr.Read())
                        {
                            string relativePath = Convert.ToString(sdr["monitorFileRelativeFullPath"]);

                            if (fileSet.Add(relativePath))
                            {
                                #region analyse  the rows
                                //get the monitor server
                                string msID = Convert.ToString(sdr["monitorServerID"]);
                                MonitorServer ms = mslist.Find((x) => x.ID.Equals(msID));

                                //get the filename
                                string fileName = Convert.ToString(sdr["monitorFileName"]);
                                string macbasePath = @"\\" + Convert.ToString(sdr["monitorServerIP"]) + @"\" + Convert.ToString(sdr["sharePoint"]);
                                string winbasePath = ms.DBServerIP + ms.MonitorLocalPath.Substring(ms.MonitorLocalPath.IndexOf(':') + 1);
                                string lastWriteTime = Convert.ToString(sdr["updateDate"]);

                                //check filename or subdir contains the search pattern
                                if (fileName.Contains(searchPattern))
                                {
                                    #region file

                                    ds.FileInfoList.Add(new DFileInfo()
                                    {
                                        Name = fileName,
                                        WinPath = winbasePath + relativePath,
                                        MacPath = macbasePath + relativePath,
                                        LastWriteTime = lastWriteTime
                                    });

                                    #endregion
                                }

                                #region check folder

                                //check the count match the needle
                                string[] subdirs = relativePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < subdirs.Count() - 1; i++)
                                {
                                    string subdir = subdirs[i];
                                    if (subdir.Contains(searchPattern))
                                    {
                                        string temp = string.Empty;
                                        for (int j = 0; j <= i; j++) temp += @"\" + subdirs[j];
                                        string folderMacPath = macbasePath + temp;
                                        if (folderSet.Add(folderMacPath))
                                        {

                                            ds.FolderInfoList.Add(new DFolderInfo()
                                            {
                                                Name = subdir,
                                                WinPath = winbasePath + temp,
                                                MacPath = folderMacPath,
                                                LastWriteTime = lastWriteTime
                                            });
                                        }
                                    }
                                }
                                #endregion

                                #endregion
                                //check the count
                                if (ds.Count >= maxsize) break;
                            }
                        }
                    }
                    #endregion
                    #endregion

                    #region from pathBk
                    if (ds.Count <= maxsize)
                    {
                        #region get reader
                        sdrPathBk = fdal.GetSearchReaderPathBk(searchPattern, msidCond);

                        #endregion
                        #region get the match list

                        if (sdrPathBk != null)
                        {
                            
                            while (sdrPathBk.Read())
                            {
                                //get the filename
                                string relativePath = Convert.ToString(sdrPathBk["monitorFileRelativeFullPath"]);
                                if (fileSet.Add(relativePath))
                                {
                                    #region analyse  the rows
                                    //get the monitor server
                                    string msID = Convert.ToString(sdrPathBk["monitorServerID"]);
                                    MonitorServer ms = mslist.Find((x) => x.ID.Equals(msID));


                                    int lastIndex = relativePath.LastIndexOf(Path.DirectorySeparatorChar);
                                    string fileName = relativePath.Substring(lastIndex + 1);
                                    string macbasePath = @"\\" + ms.MonitorServerIP + @"\" + ms.StartFile;
                                    string winbasePath = ms.DBServerIP + ms.MonitorLocalPath.Substring(ms.MonitorLocalPath.IndexOf(':') + 1);
                                    string lastWriteTime = "";

                                    #endregion

                                    //check filename or subdir contains the search pattern
                                    if (fileName.Contains(searchPattern))
                                    {
                                        #region file
                                        ds.FileInfoList.Add(new DFileInfo()
                                        {
                                            Name = fileName,
                                            WinPath = winbasePath + relativePath,
                                            MacPath = macbasePath + relativePath,
                                            LastWriteTime = lastWriteTime
                                        });
                                        #endregion
                                    }

                                    #region check folder

                                    //check the count match the needle
                                    string[] subdirs = relativePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int i = 0; i < subdirs.Count() - 1; i++)
                                    {
                                        string subdir = subdirs[i];
                                        if (subdir.Contains(searchPattern))
                                        {
                                            string temp = string.Empty;
                                            for (int j = 0; j <= i; j++) temp += @"\" + subdirs[j];
                                            string folderMacPath = macbasePath + temp;
                                            if (folderSet.Add(folderMacPath))
                                            {

                                                ds.FolderInfoList.Add(new DFolderInfo()
                                                {
                                                    Name = subdir,
                                                    WinPath = winbasePath + temp,
                                                    MacPath = folderMacPath,
                                                    LastWriteTime = lastWriteTime
                                                });
                                            }
                                        }
                                    }
                                    #endregion

                                    //check the count
                                    if (ds.Count >= maxsize) break;
                                }

                            }
                        }

                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                if (sdr != null)
                {
                    sdr.Close();
                }

                if (sdrPathBk != null)
                {
                    sdr.Close();
                }
            }
            return ds;

        }

        private string GetMsidCond(string username, string dbServerip, string monitorServerID)
        {

            string msidCond = "";
            if (monitorServerID != "-1")
            {
                msidCond = " = " + monitorServerID;
            }
            else
            {
                if (!"admin".Equals(username))
                {
                    List<Models.MonitorServer> mslist = GetRemoteMSList(dbServerip) ?? new List<Models.MonitorServer>();

                    var ms = mslist.FirstOrDefault(x => x.MonitorServerName.Equals("soumu"));
                    if (ms != null)
                    {
                        if ("mac".Equals(username))
                        {
                            msidCond = " != " + ms.ID;
                        }
                        else
                        {
                            msidCond = "  = " + ms.ID;
                        }
                    }
                }
            }

            return msidCond;


        }

       

        /// <summary>
        /// 获取相对应的远端monitorserver list
        /// </summary>
        /// <returns></returns>
        public List<Models.MonitorServer> GetRemoteMSList(string dbServerip)
        {
            FileDownloadDAL fdal = new FileDownloadDAL();
            return fdal.GetRemoteMSList(dbServerip);
        }

    }

}