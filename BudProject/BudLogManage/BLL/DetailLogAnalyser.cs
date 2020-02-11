using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IBLL;
using BudLogManage.DAL.IDAL;
using BudLogManage.Factory;
using System.IO;
using System.Reflection;
using log4net;
using BudLogManage.Model;
using BudLogManage.Common.Util;
using System.Text.RegularExpressions;
using BudLogManage.Common.Helper;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于解析SSH
    /// </summary>
    public class DetailLogAnalyser : ILogAnalyser
    {
        private IReaderFactory readerFactory = null;
        public DetailLogAnalyser(IReaderFactory readerFactory)
        {
            this.readerFactory = readerFactory;
        }

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region status
        private int _filesTotalCount = 0;
        public int FilesTotalCount {
            get{
                if (_filesTotalCount == 0)
                {
                    _filesTotalCount = FilesManager.GetFiles(ConfigManager.GetCurrentConfig()).Count;
                }
                return _filesTotalCount;
            }
            set
            {
                _filesTotalCount = value;
            }
        }
        private int _filesReadedCount = 0;
        public int FilesReadedCount
        {
            get { return _filesReadedCount; }
            set { _filesReadedCount = value; }
        }
        #endregion
        /// <summary>
        /// 处理文件
        /// </summary>
        /// <param name="fpath"></param>
        public void Analyse(Config config)
        {
            //清空cache
            Cache.Cache.OpersList.Clear();

            //读日志、预处理、解析、存数据库
            //获取所有日志文件，然后遍历解析并汇总
            //robocopy log  //ssh log
            List<string> files = FilesManager.GetFiles(config);
            if (files != null && files.Count > 0)
            {
                _filesTotalCount = files.Count;
                _filesReadedCount = 0;
                foreach (var file in files)
                {
                    if (_filesTotalCount == 0) break;
                    ReadToCache(file);
                    _filesReadedCount++;
                }
            }
            
        }

        /// <summary>
        /// 获取合计
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public Total GetTotal(TimeInterval ti)
        {
            Total total = new Total();
            
            try
            {
                var operlist = Cache.Cache.OpersList.Where(x => DateTimeUtil.GetDateTime(x.start) >= ti.Start && DateTimeUtil.GetDateTime(x.end) <= ti.End);
                if (operlist != null && operlist.Count() > 0)
                {
                    //解析
                    foreach (var operation in operlist)
                    {
                        try
                        {
                            //判断是传送的 还是 copy的
                            OperationType ot = OperationTypeUtil.GetOperationType(operation);
                            if (OperationType.ToLocal == ot)
                            {
                                total.FilesCopied += Convert.ToInt32(operation.files_copied);
                                total.FilesDeleted += Convert.ToInt32(operation.files_extras);
                                total.BytesCopied += new Common.Helper.Size(operation.bytes_copied);
                                total.BytesDeleted += new Common.Helper.Size(operation.bytes_extras);
                            }
                            else if (OperationType.ToRemote == ot)
                            {
                                total.FileTransfered += Convert.ToInt32(operation.files_copied);
                                total.BytesTransfered += new Size(operation.bytes_copied);
                                total.FileTransDeleted += Convert.ToInt32(operation.files_extras);
                                total.BytesTransDeleted += new Common.Helper.Size(operation.bytes_extras);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            logger.Error(ex.Message);
                            throw;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }

            return total;
        }

        /// <summary>
        /// 将数据读取到缓存
        /// </summary>
        /// <param name="fpath"></param>
        private void ReadToCache(string fpath)
        {
            ILogReader ilr = GetLogReader(fpath);
            if (ilr == null) return;
            try
            {
                while (!ilr.EndOfStream)
                {
                    Operation oper = ilr.ReadBlock();
                    if (!string.IsNullOrWhiteSpace(oper.dirs_total))
                    {
                        Cache.Cache.OpersList.Add(oper);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                if (ilr != null) ilr.Close();
            }

        }

        /// <summary>
        /// 读取数据到数据库
        /// </summary>
        private void PreProcess(string fpath)
        {
            ILogReader ilr = GetLogReader(fpath);
            if (ilr == null) return;

            ilr.ReadBlock();
        }

        /// <summary>
        /// 判断文件是否存在并确定日志类型,然后使用相对应的适配器读取
        /// </summary>
        /// <param name="fpath"></param>
        /// <returns></returns>
        private ILogReader GetLogReader(string fpath)
        {
            ILogReader ilr = null;
           
            //判断文件是否存在并确定日志类型,然后使用相对应的适配器读取
            if (File.Exists(fpath))
            {
                 //默认
                Provider provider = Provider.Unknown;

                //读取前4行 判断此4行中有否关键字（SSH,ROBOCOPY)
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(fpath);
                    #region 判断日志类型
                    int i = 4;
                    while (i-- > 0)
                    {
                        string content = sr.ReadLine();
                        if (content.Contains(Provider.ROBOCOPY.ToString()))
                        {
                            provider = Provider.ROBOCOPY; break;
                        }
                        if (content.Contains(Provider.SSH.ToString()))
                        {
                            provider = Provider.SSH; break;
                        }
                    }
                    #endregion
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                    throw;
                }
                finally
                {
                    if (sr != null) sr.Close();
                }
                #region 生成reader
                if (provider == Provider.ROBOCOPY)
                {
                    ilr = readerFactory.GetRoboCopyLogReader();
                    ilr.Path = fpath;
                }
                else if(provider == Provider.SSH){ 
                    ilr = readerFactory.GetSSHLogReader();
                    ilr.Path = fpath;

                }
                #endregion

            }

            return ilr;
        }
    }
}
