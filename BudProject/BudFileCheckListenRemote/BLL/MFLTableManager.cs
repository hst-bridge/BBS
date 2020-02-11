using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.IO;
using BudFileCheckListen.Entities;
using BudFileCheckListen.DBService;
using BudFileCheckListen.Common.Constant;
using BudFileCheckListen.Common.FileSystem;
using BudFileCheckListen.Common.Util;
using BudFileCheckListen.Models;

namespace BudFileCheckListen.BLL
{
    /// <summary>
    /// 用于维护monitorFileListen表
    /// </summary>
    class MFLTableManager
    {

        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ReactiveQueue<monitorFileListen> rqueue = new ReactiveQueue<monitorFileListen>();
        private MFLTableManager()
        {
            rqueue.DequeueHandler += new DequeueEventHandler<monitorFileListen>(rqueue_DequeueHandler);
        }

        public static readonly MFLTableManager Instance = new MFLTableManager();
        /// <summary>
        /// 用于维护文件系统
        /// </summary>
        /// <param name="item"></param>
        void rqueue_DequeueHandler(List<monitorFileListen> items)
        {
            try
            {
                MonitorFileListenService mflManager = new MonitorFileListenService();
                mflManager.Insert(items,GetSql);
            }
            catch (Exception ex)
            {
                logger.Error("MFLTableManager" + Environment.NewLine + MessageUtil.GetExceptionMsg(ex, ""));
            }
        }
        /// <summary>
        /// 维护到数据库
        /// </summary>
        public void Handle(monitorServer ms,FileSystemEventArgs item)
        {
            //忽略更新事件
            if (item.ChangeType != WatcherChangeTypes.Changed) { 
                //封装MFL对象 并压入队列
                InsertMFL(ms, item);
            }
        }

         /// <summary>
        /// 封装MFL对象 并压入队列
        /// </summary>
        /// <param name="file"></param>
        private void InsertMFL(monitorServer ms, FileSystemEventArgs item)
        {
            try
            {
                //根据不同事件类型,封装信息多少有所不同
                switch(item.ChangeType)
                {
                    case WatcherChangeTypes.Deleted: { var mfl = GetWhenDeletedType(ms, item); if(mfl!=null) rqueue.Enqueue(mfl); break; }
                    case WatcherChangeTypes.Created: { var mfl = GetWhenCreatedType(ms, item); if (mfl != null) rqueue.Enqueue(mfl); break; }
                    case WatcherChangeTypes.Changed: { var mfl = GetWhenChangedType(ms, item); if (mfl != null) rqueue.Enqueue(mfl); break; } 
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("MFLTableManager" + Environment.NewLine + item.FullPath + Environment.NewLine + MessageUtil.GetExceptionMsg(ex, ""));
            }
        }

        #region 用于封装对象和获取对应sql

        #region 用于封装对象
        /// <summary>
        /// 创建事件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private monitorFileListen GetWhenCreatedType(monitorServer ms, FileSystemEventArgs item)
        {
            monitorFileListen mflinfo = new monitorFileListen();
            FileInfoStruct fi = FSUtil.FindSpecialFileInfo(item.FullPath);

            mflinfo.monitorServerID = ms.id;
            mflinfo.monitorFileName = fi.Name;
            mflinfo.monitorType = "新規";
            mflinfo.monitorServerIP = ms.monitorServerIP;
            mflinfo.sharePoint = ms.startFile;
            mflinfo.monitorLocalPath = ms.monitorLocalPath;
            if (item.Name.Contains('\\'))
            {
                mflinfo.monitorFileRelativeDirectory = "\\" + item.Name.Substring(0, item.Name.LastIndexOf('\\'));
            }
            else
            {
                mflinfo.monitorFileRelativeDirectory = "";
            }

            mflinfo.monitorFileRelativeFullPath = "\\" + item.Name;

            mflinfo.monitorFileLastWriteTime = fi.LastWriteTime;
            mflinfo.monitorFileSize = fi.Length.ToString();
            mflinfo.monitorFileExtension = fi.Extension;

            mflinfo.monitorFileCreateTime = fi.CreationTime;
            mflinfo.monitorFileLastAccessTime = fi.LastAccessTime;
            mflinfo.monitorStatus = "転送済";
            mflinfo.monitorFileStartTime = DateTime.Now;
            mflinfo.monitorFileEndTime = DateTime.Now;
            mflinfo.deleteFlg = DefaultValue.DEFAULTINT_VALUE;
            mflinfo.deleter = DefaultValue.DEFAULTCHAR_VALUE;
            mflinfo.deleteDate = DefaultValue.DEFAULTDATETIME_VALUE;
            mflinfo.creater = "admin";
            mflinfo.createDate = DateTime.Now;
            mflinfo.updater = "admin";
            mflinfo.updateDate = DateTime.Now;

            return mflinfo;
        }
        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private monitorFileListen GetWhenDeletedType(monitorServer ms, FileSystemEventArgs item)
        {
            monitorFileListen mflinfo = new monitorFileListen();
            
            mflinfo.monitorServerID = ms.id;
            mflinfo.monitorType = "削除";
            mflinfo.monitorStatus = "削除済";
            mflinfo.monitorFileRelativeFullPath = "\\"+ item.Name;
            mflinfo.deleteDate = DateTime.Now;
            mflinfo.deleteFlg = 1;
            return mflinfo;
        }
        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private monitorFileListen GetWhenChangedType(monitorServer ms, FileSystemEventArgs item)
        {
            monitorFileListen mflinfo = new monitorFileListen();
            FileInfoStruct fi = FSUtil.FindSpecialFileInfo(item.FullPath);

            mflinfo.monitorServerID = ms.id;
            mflinfo.monitorType = "更新";
            mflinfo.monitorFileRelativeFullPath = "\\" + item.Name;
            mflinfo.monitorFileSize = fi.Length.ToString();
            mflinfo.updateDate = DateTime.Now;

            return mflinfo;
        }
        #endregion

        #region 用于获取对应sql
        /// <summary>
        /// 将monitorFileListen对象转换成sql字符串 xiecongwen 20140705
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private void GetSql(monitorFileListen mFL,StringBuilder sb)
        {
            try
            {
                string sql = string.Empty;
                if ("新規".Equals(mFL.monitorType))
                {
                    #region 新規
                    //判断是否已经存在
                    //string check = string.Format(@" if exists(select id from [monitorFileListen] where deleteFlg=0 and monitorServerID={0} and monitorFileRelativeFullPath='{1}')",
                    //                              mFL.monitorServerID,mFL.monitorFileRelativeFullPath.Replace("'","''"));
                    sql = string.Format(@" INSERT INTO monitorFileListen
                       (monitorServerID
                       ,monitorFileName
                       ,monitorType
                       ,monitorServerIP
                       ,sharePoint
                       ,monitorLocalPath
                       ,monitorFileRelativeDirectory
                       ,monitorFileRelativeFullPath
                       ,monitorFileLastWriteTime
                       ,monitorFileSize
                       ,monitorFileExtension
                       ,monitorFileCreateTime
                       ,monitorFileLastAccessTime
                       ,monitorStatus
                       ,monitorFileStartTime
                       ,monitorFileEndTime
                       ,deleteFlg
                       ,deleter
                       ,deleteDate
                       ,creater
                       ,createDate
                       ,updater
                       ,updateDate
,[synchronismFlg]
                      ) VALUES
                       ('{0}'
                       ,N'{1}'
                       ,'{2}'
                       ,'{3}'
                       ,N'{4}'
                       ,N'{5}'                      
                       ,N'{6}'
                       ,N'{7}'                      
                       ,'{8}'
                       ,'{9}'
                       ,'{10}'
                       ,'{11}'
                       ,'{12}'
                       ,'{13}'                      
                       ,'{14}'
                       ,'{15}'                      
                       ,'{16}'
                       ,'{17}'
                       ,'{18}'
                       ,'{19}'
                       ,'{20}'
                       ,'{21}'
                       ,'{22}',0);",
                            mFL.monitorServerID
                            , mFL.monitorFileName.Replace("'", "''")
                            , mFL.monitorType
                            , mFL.monitorServerIP
                            , mFL.sharePoint.Replace("'", "''")
                            , mFL.monitorLocalPath
                            , mFL.monitorFileRelativeDirectory.Replace("'", "''")
                            , mFL.monitorFileRelativeFullPath.Replace("'", "''")
                            , mFL.monitorFileLastWriteTime
                            , mFL.monitorFileSize
                            , mFL.monitorFileExtension
                            , mFL.monitorFileCreateTime
                            , mFL.monitorFileLastAccessTime
                            , mFL.monitorStatus
                            , mFL.monitorFileStartTime
                            , mFL.monitorFileEndTime
                            , mFL.deleteFlg
                            , mFL.deleter
                            , mFL.deleteDate
                            , mFL.creater
                            , mFL.createDate
                            , mFL.updater
                            , mFL.updateDate);
                    #endregion
                    goto append;
                }

                if ("削除".Equals(mFL.monitorType))
                {
                    #region 削除
                    sql = string.Format(@"update [monitorFileListen] set [monitorType]='{0}',monitorStatus='{1}',deleteDate='{2}',deleteFlg={3},synchronismFlg=0
                                            where deleteFlg=0 and monitorServerID={4} and monitorFileRelativeFullPath='{5}';
                                                ", mFL.monitorType,mFL.monitorStatus,mFL.deleteDate,mFL.deleteFlg,mFL.monitorServerID,mFL.monitorFileRelativeFullPath.Replace("'","''"));
                    #endregion
                    goto append;
                }

                if ("更新".Equals(mFL.monitorType))
                {
                    #region 更新
                    sql = string.Format(@"update [monitorFileListen] set [monitorType]='{0}',monitorFileSize='{1}',updateDate='{2}',synchronismFlg=0
                                           where deleteFlg=0 and monitorServerID={3} and monitorFileRelativeFullPath='{4}';
                                                ", mFL.monitorType, mFL.monitorFileSize, mFL.updateDate, mFL.monitorServerID, mFL.monitorFileRelativeFullPath.Replace("'", "''"));
                    #endregion
                }
            append:    sb.Append(sql);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }
        #endregion

        #endregion
        ~MFLTableManager()
        {
            //清理
            rqueue.Close();
        }
    }
}
