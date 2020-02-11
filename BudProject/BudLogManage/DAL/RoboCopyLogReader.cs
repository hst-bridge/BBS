using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IDAL;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using log4net;
using BudLogManage.Model;
using BudLogManage.Common.Helper;
using BudLogManage.Common;
using BudLogManage.Common.Culture;

namespace BudLogManage.DAL
{
    public class RoboCopyLogReader : ILogReader
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  
        /// 匹配------------------------------------
        /// 1.@"^(\s*-{20,}[(\r\n)|(\n)])?$" readline 不读取换行符
        /// </summary>
        private Regex endRegex = new Regex(@"^(\s*-{20,})$");
        /*
         * 使用正则表达式进行匹配
         * */
        //private Regex blockRegex = new Regex("");
        private StreamReader streamReader = null;
        private StreamReader Reader
        {
            get
            {
                if (streamReader == null)
                {
                    try
                    {
                        streamReader = new StreamReader(Path,CultureManager.GetEncoding());
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(ex.Message);
                        throw;
                    }
                }

                return streamReader;
            }
        }
        public RoboCopyLogReader()
        {

        }

        public RoboCopyLogReader(string path)
        {
            this.Path = path;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 读取一次操作
        /// </summary>
        public Operation ReadBlock()
        {
            Operation otn = new Operation() { provider= Provider.ROBOCOPY,logEntrys = new List<LogEntry>() };

            GetTop(otn);
          //  GetBody(otn);
            GetTail(otn);

            return otn;
        }

        /// <summary>
        /// 跳过标题信息
        ///1 
        ///2-------------------------------------------------------------------------------
        ///3   ROBOCOPY     ::     Windows の堅牢性の高いファイル コピー                              
        ///4-------------------------------------------------------------------------------
        /// </summary>
        private void SkipTitle()
        {
            int i = 4;
            while (i-- > 0) Reader.ReadLine();

        }
        /// <summary>
        /// 填充头信息 要找到-----------------然后停止
        /// </summary>
        /// <param name="operation"></param>
        private void GetTop(Operation operation)
        {
            if (operation == null) return;

            //扫描寻找关键字 【開始】
            string key = KeyUtil.TopKey;
            string content = string.Empty;

            //判断是否寻找到了关键字
            bool Searched = false;

            while (!Reader.EndOfStream)
            {
                
                try
                {
                    content = Reader.ReadLine().Trim();
                    //找到关键字后 解析Top
                    if (!Searched && content.Contains(key))
                    {
                        Searched = true;
                        #region 解析Top
                        //此处假设日志格式正常
                        operation.start = content.Substring(content.IndexOf(':')+1).Trim();
                        
                        //获取コピー元
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.source = content.Substring(content.IndexOf(':') + 1).Trim();
                                break;
                            }

                        }

                        //获取コピー先
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.destination = content.Substring(content.IndexOf(':') + 1).Trim();
                                break;
                            }

                        }
                        //获取ファイル
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.file = content.Substring(content.IndexOf(':') + 1).Trim();
                                break;
                            }

                        }
                        //获取オプション
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.option = content.Substring(content.IndexOf(':') + 1).Trim();
                                break;
                            }

                        }
                        #endregion
                        continue;
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                }

                if (Searched && IsEndLine(content)) break;
            }
        }

        /// <summary>
        /// 填充记录  要找到-----------------然后停止
        /// </summary>
        /// <param name="operation"></param>
        private void GetBody(Operation operation)
        {
            if (operation == null) return;
            //不用扫描寻找关键字 
            string content = string.Empty;
            if(operation.logEntrys==null)operation.logEntrys = new List<LogEntry>();
            
            while (!Reader.EndOfStream)
            {
                
                try
                {
                    content = Reader.ReadLine().Replace("100%", "").Replace("0%", "").Trim();;
                    if(string.IsNullOrWhiteSpace(content))continue;
                    if (IsEndLine(content)) break;

                    //判断是否出错
                    if (content.Contains(KeyUtil.ErrorKey))
                    {
                        #region 解析错误记录
                        LogEntry le = new LogEntry(){ LType= LogType.ERROR};
                        //出错信息解析
                        le.Time = content.Substring(0, content.IndexOf(KeyUtil.ErrorKey)).Trim();
                        #region 获取path
                        int index= content.IndexOf('\\');
                        
                        if (content[index - 1] == ':')
                        {
                            le.Path = content.Substring(index - 2).Trim();
                        }
                        else
                        {
                            le.Path = content.Substring(index - 1).Trim();
                        }

                        #endregion
                        le.Cause = Reader.ReadLine().Trim();

                        operation.logEntrys.Add(le);
                        continue;
                        #endregion
                    }
                    else if (content.Contains(KeyUtil.RetryErrorKey)) continue;
                    else
                    {
                        #region 解析正常记录
                        if (content[0] != '*')
                        {
                            string[] paras = content.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            LogEntry le = new LogEntry()
                                {
                                    LType = CultureManager.GetLogType(paras[0]),
                                    Size = paras[1],
                                    Path = paras[2]
                                };
                            operation.logEntrys.Add(le);
                        }
                        else
                        {
                            LogEntry le = new LogEntry();
                            le.LType = CultureManager.GetLogType(content.Substring(0, 11));
                            string[] paras = content.Substring(11).Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            le.Size = paras[0];
                            le.Path = paras[1];

                            operation.logEntrys.Add(le);
                        }
                        #endregion
                    }
                  
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// 填充尾信息 要找到-----------------然后停止
        /// </summary>
        /// <param name="operation"></param>
        private void GetTail(Operation operation)
        {
            if (operation == null) return;
            //扫描寻找关键字 【合計】
            string key = KeyUtil.TailKey;
            string content = string.Empty;

            //判断是否寻找到了关键字
            bool Searched = false;

            while (!Reader.EndOfStream)
            {

                try
                {
                    content = Reader.ReadLine().Trim();
                    //找到关键字后 解析tail
                    if (!Searched && content.Contains(key))
                    {
                        Searched = true;
                        #region 解析tail
                        #region ディレクトリ
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                string[] dirsParas = content.Substring(content.IndexOf(':') + 1).Trim().Split(new char[]{'\t',' '},StringSplitOptions.RemoveEmptyEntries);
                                operation.dirs_total = dirsParas[0];
                                operation.dirs_copied = dirsParas[1];
                                operation.dirs_skipped = dirsParas[2];
                                operation.dirs_mismatch = dirsParas[3];
                                operation.dirs_failed = dirsParas[4];
                                operation.dirs_extras = dirsParas[5];
                                break;
                            }

                        }
                        #endregion
                        #region ファイル
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                string[] paras = content.Substring(content.IndexOf(':') + 1).Trim().Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                operation.files_total = paras[0];
                                operation.files_copied = paras[1];
                                operation.files_skipped = paras[2];
                                operation.files_mismatch = paras[3];
                                operation.files_failed = paras[4];
                                operation.files_extras = paras[5];
                                break;
                            }

                        }
                        #endregion
                        #region バイト
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                string[] paras = content.Substring(content.IndexOf(':') + 1).Trim().Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                int i = 0;
                                operation.bytes_total = paras[i++];
                                if (paras[i].Contains('g') || paras[i].Contains('m') || paras[i].Contains('k'))
                                {
                                    operation.bytes_total += paras[i++];
                                }
                                operation.bytes_copied = paras[i++];
                                if (paras[i].Contains('g') || paras[i].Contains('m') || paras[i].Contains('k'))
                                {
                                    operation.bytes_copied += paras[i++];
                                }
                                operation.bytes_skipped = paras[i++];
                                if (paras[i].Contains('g') || paras[i].Contains('m') || paras[i].Contains('k'))
                                {
                                    operation.bytes_skipped += paras[i++];
                                }
                                operation.bytes_mismatch = paras[i++];
                                if (paras[i].Contains('g') || paras[i].Contains('m') || paras[i].Contains('k'))
                                {
                                    operation.bytes_mismatch += paras[i++];
                                }
                                operation.bytes_failed = paras[i++];
                                if (paras[i].Contains('g') || paras[i].Contains('m') || paras[i].Contains('k'))
                                {
                                    operation.bytes_failed += paras[i++];
                                }
                                operation.bytes_extras = paras[i++];
                                if (i < paras.Length)
                                {
                                    operation.bytes_extras += paras[i];
                                }
                                break;
                            }

                        }
                        #endregion
                        #region 終了
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (content.Contains(KeyUtil.TailTimeKey))
                            {
                                operation.end = content.Substring(content.IndexOf(':') + 1).Trim();
                                break;
                            }

                        }
                        #endregion
                        #endregion
                        continue;
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                }

                if (Searched && IsEndLine(content)) break;
            }
        }

        /// <summary>
        /// 判断是不是段落结束线
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool IsEndLine(string content)
        {
            if (endRegex.IsMatch(content)) return true;
            else return false;
        }

        /// <summary>
        /// 判断是否结束
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return Reader.EndOfStream;
            }
        }

        public void Close()
        {
            if (this.streamReader != null)
            {
                this.streamReader.Close();
                this.streamReader = null;
            }
        }
    }
}
