using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model.Behind;
using System.Data.SqlClient;
using System.Data;
using BudSSH.Common.Helper;
using System.Collections;
using log4net;
using System.Reflection;
using BudSSH.Common.Util;

namespace BudSSH.DAL.SQLServer
{
    /// <summary>
    /// 用于访问SSHPathInfo
    /// </summary>
    class SSHPathInfoDAL
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SqlDataReader sr = null;
        /// <summary>
        /// 判断某个mac上的文件或目录是否更新
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsUpdate(Model.Config config,SSHPathInfo spi)
        {
            string connstr = config.DB.ConnString;
            SqlParameter[] spin = new SqlParameter[]{ 
                new SqlParameter("@monitorServerIP",SqlDbType.VarChar){Value=spi.MonitorServerIP},
                new SqlParameter("@macPath",SqlDbType.NVarChar){Value=spi.MacPath},
                new SqlParameter("@lastName",SqlDbType.NVarChar){Value=spi.LastName},
                new SqlParameter("@depth",SqlDbType.Int){Value=spi.depth},
                new SqlParameter("@typeflag",SqlDbType.Bit){Value=spi.typeflag},
                new SqlParameter("@updateTime",SqlDbType.VarChar){Value=spi.updateTime},

            };

            SqlParameter spout = new SqlParameter("@IsUpdate", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            return Convert.ToBoolean(DBHelper.GetOutValue(connstr, "CheckSSHPath", spout, spin));
        }

        public IEnumerable GetSSHPathInfo(Model.Config config)
        {
            #region  获取SqlDataReader
            try
            {

                string sql = "select ID,MonitorServerIP,MacPath,typeflag from SSHPathInfo";
                sr = DBHelper.ExecuteReader(config.DB.ConnString, CommandType.Text, sql);
               
            }
            catch (System.Exception ex)
            {
                Common.Util.LogManager.WriteLog(Common.Util.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                if (sr != null) { sr.Close(); sr = null; }
                yield break;
            }
            #endregion
            //获取数据
            while (sr!=null && sr.Read())
            {
                yield return new SSHPathInfo() {
                     ID = Convert.ToString(sr[0]),
                     MonitorServerIP = Convert.ToString(sr[1]),
                      MacPath = Convert.ToString(sr[2]),
                      typeflag = Convert.ToInt32(sr[3])
                };
            }

            //退出
            if (sr != null)
            {
                sr.Close();
                sr = null;
            }

            yield break;
        }

        /// <summary>
        /// 删除数据库中相应记录
        /// </summary>
        /// <param name="delIDList"></param>
        public void DeleteEntrys(Model.Config config,List<string> delIDList)
        {
            string sql = string.Empty;
            try
            {
                if (delIDList != null && delIDList.Count > 0)
                {
                    //获取sql
                    StringBuilder sqlSB = new StringBuilder("delete from [SSHPathInfo] where id in (");
                    foreach (string id in delIDList)
                    {
                        sqlSB.Append(id).Append(',');
                    }
                    sql = sqlSB.ToString().TrimEnd(',') + ")";

                    DBHelper.ExecuteNonQuery(config.DB.ConnString, CommandType.Text, sql);
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(MessageUtil.GetExceptionMsg(ex, "") + " sql:" + sql);
            }
        }
        /// <summary>
        /// 用于清理资源
        /// </summary>
        public void Dispose()
        {
            if (sr != null)
            {
                sr.Close();
                sr = null;
            }
        }
    }
}
