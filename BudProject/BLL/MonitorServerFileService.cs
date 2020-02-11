using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBLL;
using Common;
using IDAL;
using System.Data.SqlClient;
using DBUtility;
using Model;
using System.Data.Odbc;

namespace BLL
{
    public class MonitorServerFileService : IMonitorServerFileService
    {
        private IMonitorServerFile MonitorServerFileDal = DALFactory.DataAccess.CreateMonitorServerFile();
        private IMonitorServerFolder MonitorServerFolderDal = DALFactory.DataAccess.CreateMonitorServerFolder();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int GetMonitorServerFileCount(IEnumerable<SearchCondition> condition)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.GetMonitorServerFileCount(condition, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <param name="loginID"></param>
        /// <returns></returns>
        public int DeleteMonitorServerFile(int MonitorServerFileId, string loginID)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.DeleteMonitorServerFileById(MonitorServerFileId,loginID, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int UpdateMonitorServerFile(MonitorServerFile MonitorServerFile)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.UpdateMonitorServerFile(MonitorServerFile, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int UpdateTransferFlg(string MonitorServerFileId, int transferFlg)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.UpdateTransferFlg(MonitorServerFileId,transferFlg,conn);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int UpdateAllTransferFlg(string MonitorFileDirectory, int transferFlg)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.UpdateAllTransferFlg(MonitorFileDirectory, transferFlg, conn);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int UpdateNGTransferFlg(string MonitorServerFileId, int transferFlg, int transferNum)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.UpdateNGTransferFlg(MonitorServerFileId, transferFlg, transferNum, conn);
                conn.Close();
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int UpdateTransferFlgBatch(string MonitorServerFileId)
        {
            OdbcConnection conn;
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                count = MonitorServerFileDal.UpdateTransferFlgBatch(MonitorServerFileId, conn);
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 插入监视到的文件变化
        /// 先判断该文件是否需要处理
        /// 是：插入
        /// 否：不插入
        /// </summary>
        /// <param name="MonitorServerFile"></param>
        /// <returns></returns>
        public int InsertMonitorServerFile(MonitorServerFile MonitorServerFile,string filepath)
        {
            OdbcConnection conn;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "monitorServerID=? ", param = "@monitorServerID", value = MonitorServerFile.monitorServerID.ToString() },
            new  SearchCondition { con = "monitorFileName=?  ", param = "@monitorFileName", value = MonitorServerFile.monitorFileName },
            new  SearchCondition { con = "monitorFilePath=?  ", param = "@monitorFilePath", value = filepath.Replace("\\"+MonitorServerFile.monitorFileName,"") },            
            new  SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" }};
            int count = 0;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                int result = MonitorServerFolderDal.GetMonitorServerFolderCount(condition, conn);
                //文件更新了，且需要监视
                if (MonitorServerFile.monitorFileStatus == 2 && result > 0)
                {
                    count = MonitorServerFileDal.InsertMonitorServerFile(MonitorServerFile, conn);
                }
                //新增文件，重命名后文件，默认为要监视
                else if (MonitorServerFile.monitorFileStatus == 1 || MonitorServerFile.monitorFileStatus == 3 || MonitorServerFile.monitorFileStatus == 4)
                {
                    count = MonitorServerFileDal.InsertMonitorServerFile(MonitorServerFile, conn);
                }
                conn.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitorServerFileId"></param>
        /// <returns></returns>
        public MonitorServerFile GetMonitorServerFileById(int MonitorServerFileId)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "id=?", param = "@id", value = MonitorServerFileId.ToString() } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                if (MonitorServerFile.Count > 0)
                {
                    return MonitorServerFile[0];
                }
                conn.Close();
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFilePage(IEnumerable<SearchCondition> condition, int page, int pagesize)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFilePage(condition, page, pagesize, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFileList()
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg=?", param = "@deleteFlg", value = "0" } };
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }

        /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFileList(string monitorServerID)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg= ?  ", param = "@deleteFlg", value = "0" },
            new SearchCondition { con = "monitorServerID= ?  ", param = "@monitorServerID", value = monitorServerID },
            new SearchCondition { con = "transferFlg=?", param = "@transferFlg", value = "0" },
            new SearchCondition { con = "id IN (SELECT max(id) AS id FROM monitorServerFile GROUP BY monitorFilePath) ", param = "", value = "" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }

        /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFileSSBPutList(string monitorServerID, string topDir)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg= ?  ", param = "@deleteFlg", value = "0" },
            new SearchCondition { con = "monitorServerID= ?  ", param = "@monitorServerID", value = monitorServerID },
            new SearchCondition { con = "transferFlg=?", param = "@transferFlg", value = "0" },
            new SearchCondition { con = "monitorFileStatus!=?", param = "@monitorFileStatus", value = "3" },
            new SearchCondition { con = "monitorFileDirectory!=?", param = "@monitorFileDirectory", value = topDir },
            new SearchCondition { con = "id IN (SELECT max(id) AS id FROM monitorServerFile GROUP BY monitorFilePath) ", param = "", value = "" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }

        /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFileDelList(string monitorServerID)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg= ?  ", param = "@deleteFlg", value = "0" },
            new SearchCondition { con = "monitorServerID= ?  ", param = "@monitorServerID", value = monitorServerID },
            new SearchCondition { con = "transferFlg=?", param = "@transferFlg", value = "0" },
            new SearchCondition { con = "monitorFileStatus=?", param = "@monitorFileStatus", value = "3" },
            new SearchCondition { con = "id IN (SELECT max(id) AS id FROM monitorServerFile GROUP BY monitorFilePath) ", param = "", value = "" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }

        /// <summary>
        /// 転送対象の抽出
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetMonitorServerFileTopDirList(string monitorServerID, string topDir)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg= ?  ", param = "@deleteFlg", value = "0" },
            new SearchCondition { con = "monitorServerID= ?  ", param = "@monitorServerID", value = monitorServerID },
            new SearchCondition { con = "transferFlg=?", param = "@transferFlg", value = "0" },
            new SearchCondition { con = "monitorFileStatus!=?", param = "@monitorFileStatus", value = "3" },
            new SearchCondition { con = "monitorFileDirectory=?", param = "@monitorFileDirectory", value = topDir },
            new SearchCondition { con = "id IN (SELECT max(id) AS id FROM monitorServerFile GROUP BY monitorFilePath) ", param = "", value = "" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }

        /// <summary>
        /// 転送対象の失敗の場合、レコードの抽出
        /// </summary>
        /// <returns></returns>
        public IList<MonitorServerFile> GetTransferNGFileList(string monitorServerID)
        {
            OdbcConnection conn;
            IList<MonitorServerFile> MonitorServerFile;
            SearchCondition[] condition = new SearchCondition[] { new SearchCondition { con = "deleteFlg= ?  ", param = "@deleteFlg", value = "0" },
            new SearchCondition { con = "monitorServerID= ?  ", param = "@monitorServerID", value = monitorServerID },
            new SearchCondition { con = "transferFlg=?", param = "@transferFlg", value = "2" },
            new SearchCondition { con = "id IN (SELECT max(id) AS id FROM monitorServerFile GROUP BY monitorFilePath) ", param = "", value = "" }};
            using (conn = OdbcHelper.CreateConntion())
            {
                conn.Open();
                MonitorServerFile = MonitorServerFileDal.GetMonitorServerFile(condition, conn);
                conn.Close();
                return MonitorServerFile;
            }
        }
    }
}
