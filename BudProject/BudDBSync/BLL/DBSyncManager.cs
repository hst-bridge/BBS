using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudDBSync.Model;
using System.Threading;
using System.Data.SqlClient;
using BudDBSync.BLL.SyncTable;
using BudDBSync.Util;

namespace BudDBSync.BLL
{
    /// <summary>
    /// 用于数据库同步逻辑
    /// </summary>
    class DBSyncManager
    {
        
       
        private static DBServerManager dbsm = new DBServerManager();
        private SyncThreadHelper sth = null;
        //配置用于同步的数据库表
        private static List<ITable> tables = null;
        public DBSyncManager()
        {
            tables = new List<ITable>() { new BackupServerGroupTable(), new MonitorServerTable(), new LogTable(), new MonitorFileListenTable(), };
        }
        /// <summary>
        /// 全权负责同步逻辑，并自动监听配置
        /// </summary>
        public void BeginSync()
        {
            try
            {
                /*
                 * 重建一个对象 维护IsStoping 这样以前的线程在点击停止按钮后
                 * 会自动停止
                 * */
                sth = new SyncThreadHelper() { IsStoping=false};

                //获取target dbserver
                DBServer targetdb = dbsm.GetTargetInfo();
                //验证target dbserver的正确性
                if (dbsm.LinkTest(targetdb))
                {
                    //获取所有source dbserver
                    List<DBServer> list = dbsm.GetAllSourceInfolist();
                    
                    //使用多线程 此处不要使用foreach 编译器优化后逻辑错误
                    IEnumerator<DBServer> dbs = list.AsEnumerable().GetEnumerator();
                    while(dbs.MoveNext())
                    {
                        DBServer dbserver = dbs.Current;
                        if (dbsm.LinkTest(dbserver))
                        {
                            new Thread(() => sth.DBSync(dbserver)).Start();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
               LogManager.WriteLog(LogFile.Error,ex.Message);
                throw;
            }
                   
        }

        public void EndSync()
        {
            //通知当前运行的线程停止
            if (sth != null)
            {
                sth.IsStoping = true;
                SqlConnection.ClearAllPools();
            }
        }

        /// <summary>
        /// 当点击停止按钮时，通知以前的线程停止
        /// 再次点击开始，则新建一批线程
        /// </summary>
        private class SyncThreadHelper
        {
            
            public bool IsStoping { get; set; }
            
            /// <summary>
            /// 随时监控配置中相应DBServer的编辑、消除
            /// </summary>
            /// <param name="server"></param>
            public void DBSync(DBServer server)
            {

                while (true)
                {
                    
                    try
                    {
                        if (IsStoping) break;

                        //获取target dbserver
                        DBServer targetdb = dbsm.GetTargetInfo();
                        //验证target dbserver的正确性
                        if (dbsm.LinkTest(targetdb))
                        {
                            //判断自身的存在,即是否被删除
                            //获取所有source dbserver
                            List<DBServer> list = dbsm.GetAllSourceInfolist();
                            //获取最新的配置(Ps:比如loginname password 修改了)
                            DBServer dbserver = list.Find(x => x.ServerName.Equals(server.ServerName) && x.DatabaseName.Equals(server.DatabaseName));
                            if (dbserver!=null)
                            {
                                int topNumber = Properties.Settings.Default.TopNumber;
                                int numForSync = 0;
                                //遍历需要同步的数据库
                                foreach (ITable table in tables)
                                {
                                    if (IsStoping) break;
                                    #region 同步table
                                    //new Thread((currenttable) =>
                                    //{
                                        ITable ctable = table;
                                        SqlConnection connTarget = null, connSource = null, connUpdateSource = null;
                                        StringBuilder sqlTarget = null;
                                            StringBuilder sqlUpdatesource = null;
                                            string sourcesql = null;
                                        try
                                        {
                                            /*
                                            * 同步逻辑
                                            * 
                                            * 使用分页 20140705 以前
                                             *****************************
                                            * xiecongwen 20140705 鉴于log表数据巨大，查询性能低下，且不需要维护（即删除，更新操作），特进行如下优化
                                            * 以前是 从id=0扫描原数据log表中 synchronismFlg=0的字段 即 order by id asc
                                            * 改为：order by id desc 即从当前最新往前扫描 条件改为 id between end 来避免全表扫描
                                            * PS：其他表不变 仍然order by id asc，因为其他表，需要维护（删除状态，更新状态） 
                                            */
                                            #region 数据库连接 命令

                                            //source数据库连接
                                            connTarget = new SqlConnection(targetdb.ConnString);
                                            SqlCommand comTarget = new SqlCommand()
                                            {
                                                Connection = connTarget,
                                                CommandType = System.Data.CommandType.Text,
                                                CommandTimeout = 0

                                            };
                                            //target数据库连接
                                            connSource = new SqlConnection(server.ConnString);
                                            SqlCommand comSource = new SqlCommand()
                                            {
                                                Connection = connSource,
                                                CommandType = System.Data.CommandType.Text,
                                                CommandTimeout = 0

                                            };

                                            //update source
                                            connUpdateSource = new SqlConnection(server.ConnString);
                                            SqlCommand comUpdateSource = new SqlCommand()
                                            {
                                                Connection = connUpdateSource,
                                                CommandType = System.Data.CommandType.Text,
                                                CommandTimeout = 0

                                            };
                                            #endregion

                                            #region 删除旧数据——2014-9-19 wjd add

                                            string delSql = ctable.DeleteOldDataSql;
                                            if (!String.IsNullOrWhiteSpace(delSql))
                                            {
                                                //删除目标表的数据
                                                if (connTarget.State != System.Data.ConnectionState.Open)
                                                {
                                                    connTarget.Open();
                                                }
                                                comTarget.CommandText = delSql;
                                                comTarget.ExecuteNonQuery();

                                                //删除源表的数据
                                                if (connSource.State != System.Data.ConnectionState.Open)
                                                {
                                                    connSource.Open();
                                                }
                                                comSource.CommandText = delSql;
                                                comSource.ExecuteNonQuery();
                                            }

                                            #endregion

                                            int ID = -1;//表示开始
                                            int counts = 0;
                                            
                                            do
                                            {
                                                if (IsStoping) break;

                                                if (connSource.State != System.Data.ConnectionState.Open)
                                                {
                                                    connSource.Open();
                                                }
                                                counts = 0;
                                                #region source sql

                                                comSource.CommandText = ctable.SourceSql(ID);
                                                sourcesql = comSource.CommandText;
                                                #endregion
                                                SqlDataReader reader = comSource.ExecuteReader();

                                                sqlTarget = new StringBuilder();
                                                sqlUpdatesource = new StringBuilder(ctable.UpdateSourceSql);
                                                #region 拼接1.insert sql for targetdb 2.update sql for sourcedb
                                                //判断是否有数据
                                                bool isRead = false;
                                                while (reader.Read())
                                                {
                                                    if (IsStoping) break;

                                                    //判断synchronismFlg是否为0
                                                    if (Convert.ToInt32(reader["synchronismFlg"]) != 0)
                                                    {
                                                        break;
                                                    }
                                                    if (!isRead)
                                                    {
                                                        isRead = true;
                                                    }

                                                    counts++;
                                                    #region target sql
                                                    object[] objs = new object[33];
                                                    objs[0] = reader[0];
                                                    objs[1] = server.ServerName;
                                                    for (int i = 2; i < reader.FieldCount; i++)
                                                    {
                                                        object val = reader[i - 1];
                                                        if (val is DateTime)
                                                        {
                                                            objs[i] = Convert.ToDateTime(val).ToString("yyyy/MM/dd HH:mm:ss");
                                                        }
                                                        else
                                                        {
                                                            objs[i] = Convert.ToString(val).Replace("'", "''");
                                                        }
                                                    }

                                                    sqlTarget.Append(ctable.TargetSql(objs));
                                                    #endregion

                                                    //获取最小id
                                                    ID = Convert.ToInt32(reader[0]);//使用主键id,保证数据只获取一次

                                                    #region update sql for source
                                                    sqlUpdatesource.Append(ID.ToString()).Append(",");
                                                    #endregion
                                                }
                                                if (!reader.IsClosed) reader.Close();
                                                //如果没有数据，则跳出
                                                if (!isRead) break;
                                                #endregion

                                                if (IsStoping) break;

                                                #region 往targetdb插入数据
                                                if (sqlTarget.Length > 0)
                                                {
                                                    if (connTarget.State != System.Data.ConnectionState.Open)
                                                    {
                                                        connTarget.Open();
                                                    }
                                                    comTarget.CommandText = sqlTarget.ToString();

                                                    comTarget.ExecuteNonQuery();
                                                }
                                                #endregion

                                                #region update source
                                                if (sqlUpdatesource.Length > 0)
                                                {
                                                    sqlUpdatesource.Remove(sqlUpdatesource.Length - 1, 1).Append(")");
                                                    if (connUpdateSource.State != System.Data.ConnectionState.Open)
                                                    {
                                                        connUpdateSource.Open();
                                                    }
                                                    comUpdateSource.CommandText = sqlUpdatesource.ToString();
                                                    comUpdateSource.ExecuteNonQuery();
                                                }
                                                #endregion

                                                if (IsStoping) break;

                                            } while (counts >= topNumber);//如果小于Properties.Settings.Default.TopNumber，说明没有数据了
                                            LogManager.WriteLog(LogFile.log, server.ServerName+":"+ctable.ToString()+"success");
                                        }
                                        catch (Exception ex)
                                        {
                                            if (string.IsNullOrEmpty(sourcesql))
                                                LogManager.WriteLog(LogFile.SQL,server.ServerName+":"+ctable.ToString() + sourcesql + "\n");

                                            if(sqlTarget!=null)
                                                LogManager.WriteLog(LogFile.SQL,server.ServerName+":"+ctable.ToString()+ sqlTarget.ToString() + "\n");

                                            if(sqlUpdatesource!=null)
                                                LogManager.WriteLog(LogFile.SQL,server.ServerName+":"+ctable.ToString()+ sqlUpdatesource.ToString() + "\n");

                                            LogManager.WriteLog(LogFile.Error,server.ServerName+":"+ctable.ToString()+ex.Message+"\n"+ex.StackTrace);
                                        }
                                        finally
                                        {
                                            if (connTarget != null && connTarget.State != System.Data.ConnectionState.Closed)
                                            {
                                                connTarget.Close();
                                            }
                                            if (connSource != null && connSource.State != System.Data.ConnectionState.Closed)
                                            {
                                                connSource.Close();
                                            }
                                            if (connUpdateSource != null && connUpdateSource.State != System.Data.ConnectionState.Closed)
                                            {
                                                connUpdateSource.Close();
                                            }

                                            Interlocked.Increment(ref numForSync);
                                        }
                                    //}).Start(table);
                                    #endregion
                                }

                                //while (true)
                                //{
                                //    if (numForSync >= tables.Count) break;
                                //    Thread.Sleep(2000);
                                //    if (IsStoping) break;
                                //}
                            }
                            else break;
                        }
                        else break;

                    }
                    catch (Exception ex)
                    {

                        LogManager.WriteLog(LogFile.Error,server.ServerName +":"+ ex.Message);
                    }
                    

                    if (IsStoping) break;

                    Thread.Sleep(Properties.Settings.Default.SleepTime);
                }
            }
        }

    }

    
}
