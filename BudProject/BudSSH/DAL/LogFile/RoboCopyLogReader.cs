using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using log4net;
using BudSSH.Model;
using BudSSH.Common.Helper.Culture;
using BudSSH.Common.Util;

namespace BudSSH.DAL.LogFile
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
                        logger.Error(MessageUtil.GetExceptionMsg(ex, ""));
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
        /// 获取此文件中所有的Error记录
        /// </summary>
        public ErrorPathInfo GetErrorList()
        {
            return Scan();
        }

        /// <summary>
        /// 填充记录  要找到-----------------然后停止
        /// </summary>
        /// <param name="operation"></param>
        private ErrorPathInfo Scan()
        {
            ErrorPathInfo epi = new ErrorPathInfo();
            GetTop(epi);
            List<ErrorEntry> list = new List<ErrorEntry>();
            //用于判断是否已经存在
            HashSet<string> keyset = new HashSet<string>();
            //扫描寻找关键字 
            string keyword = KeyUtil.ErrorKey;
            string retrykeyword = KeyUtil.RetryErrorKey;
            #region scan
            string errorContent = string.Empty;
            string content = string.Empty;
            while (!Reader.EndOfStream)
            {
                
                try
                {
                    content = Reader.ReadLine().Replace("100%", "").Replace("0%", "").Trim();
                    if(string.IsNullOrWhiteSpace(content))continue;

                    //判断是否出错
                    if (content.Contains(keyword) && !content.Contains(retrykeyword))
                    {
                        errorContent = content;
                        #region 解析错误记录
                        ErrorEntry entry = new ErrorEntry();
                        //出错信息解析
                        entry.Time = content.Substring(0, content.IndexOf(keyword)).Trim();
                        #region 获取path
                        /*
                         * 当数据格式发生变化时,如下
                         * 
                         * 1.例子
                         * 2014/06/10 01:34:00 エラー 0 (0x00000000) 
                             \\10.0.5.105\Job5A-web\021阪急-その他\推進協力会\00_推進協力会運用中\BK\?_dj\
                            この操作を正しく終了しました。
                         * 
                         * 2.例子
                         * 2014/11/15 01:41:08 エラー 123 (0x0000007B) コピー先ディレクトリを作成しています 
                            2014/11/15 01:41:08 エラー 123 (0x0000007B) ファイルをコピーしています 
                            S:\Job1A-1ka\01-提案中\Theお宿リニューアル\カセットフォーマット\-素材\-Bあとすて\別紙C*\\\10.0.5.101\Job1A\01-提案中\阪急カレンダー2015\-オレの\2014年カレンダー案\2014-提案-A\Icon
                            2014/11/15 01:41:08 エラー 123 (0x0000007B) コピー先ディレクトリを作成しています 2014/11/15 01:41:08 エラー 123 (0x0000007B) コピー先ディレクトリを作成しています S:\Job1A-1ka\01-提案中\Theお宿リニューアル\カセットフォーマット\-素材\-Bあとすて\別紙E*\S:\Job1A-1ka\01-提案中\Theお宿リニューアル\カセットフォーマット\-素材\-Bあとすて\別紙D*\
                            ファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。


                            ファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。


                            ファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。


                            ファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。


                            エラー: 再試行が制限回数を超えました。
                         * 
                         */

                        //get index 此处content可能发生变化，直到找到路径信息
                        int index = content.IndexOf('\\');
                        while(index < 0)
                        {
                            //error cause
                            entry.Cause = content;

                            content = Reader.ReadLine().Replace("100%", "").Replace("0%", "").Trim().TrimEnd('\\');
                            index = content.IndexOf('\\');
                        }

                        if (index == 0) {
                            entry.Path = content;
                        }
                        else
                        {
                            

                            if (content[index - 1] == ':')
                            {
                                entry.Path = content.Substring(index - 2).Trim().TrimEnd('\\');
                                /*
                                 * 特殊情况
                                 * 
                                    2014/11/15 01:41:08 エラー 123 (0x0000007B) ファイルをコピーしています 
                                    S:\Job1A-1ka\01-提案中\Theお宿リニューアル\カセットフォーマット\-素材\-Bあとすて\別紙C*\\\10.0.5.101\Job1A\01-提案中\阪急カレンダー2015\-オレの\2014年カレンダー案\2014-提案-A\Icon

                                 * 
                                 */
                                if (entry.Path.Contains("\\\\"))
                                {
                                    entry.Path = entry.Path.Substring(0, entry.Path.IndexOf("\\\\"));
                                }
                                #region 转换路径
                                /*
                                 * コピー先路径转成コピー元路径
                                 * 
                                 * 2014/06/10 01:33:55 エラー 123 (0x0000007B) コピー先ディレクトリを作成しています S:\Job5A-web\021阪急-その他\推進協力会\00_推進協力会運用中\BK\?_dj\
                                    ファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。
                                 * 
                                 * 将S:\Job5A-web\021阪急-その他\ 替换为 类似\\10.0.5.105\Job5A-web\021阪急-その他\的路径
                                 */
                                entry.Path = entry.Path.Replace(epi.destination, epi.source).TrimEnd('\\');
                                #endregion
                            }
                            else
                            {
                                entry.Path = content.Substring(index - 1).Trim().TrimEnd('\\');
                            }
                        }
                        #region 检验日志格式是否发生混乱
                        /*
                         * 检验日志格式是否发生混乱
                         * 即:2014/06/10 01:31:46 エラー 123 (0x0000007B) ファイルをコピーしています \\10.0.5.105\Job5A-web\021阪急-その他\Iconファイル名、ディレクトリ名、またはボリューム ラベルの構文が間違っています。
                         * 
                         * 如上 异常原因与路径混到一块
                         */

                        int lastIndex = entry.Path.LastIndexOf('\\');
                        if (entry.Path.Substring(lastIndex).Contains(KeyUtil.CauseKey))
                        {
                            entry.Path = entry.Path.Substring(0, entry.Path.LastIndexOf(KeyUtil.CauseKey)).TrimEnd('\\');
                        }

                        if (entry.Path.Contains("0x0000007B")) continue;
                        #endregion
                        #endregion
                        //获取错误原因 只向下读两行，如果没有符合条件的，则跳过
                        int i = 2;
                        while (string.IsNullOrWhiteSpace(entry.Cause) && i-->0)
                        {
                            entry.Cause = Reader.ReadLine().Trim();
                        }

                        
                        if (keyset.Add(entry.Path))list.Add(entry);

                        continue;
                        #endregion
                    }
                   
                  
                }
                catch (System.Exception ex)
                {
                    logger.Error(errorContent +" "+ MessageUtil.GetExceptionMsg(ex," "));
                }
            }
            #endregion
            epi.PathList = list;
            //即时释放资源
            this.Dispose();

            return epi;
        }

        /// <summary>
        /// 填充头信息 要找到-----------------然后停止
        /// </summary>
        /// <param name="ErrorPathInfo"></param>
        private void GetTop(ErrorPathInfo operation)
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
                        operation.start = content.Substring(content.IndexOf(':') + 1).Trim();

                        //获取コピー元
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.source = content.Substring(content.IndexOf(':') + 1).Trim().TrimEnd('\\');
                                break;
                            }

                        }

                        //获取コピー先
                        while (!Reader.EndOfStream)
                        {
                            content = Reader.ReadLine().Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                operation.destination = content.Substring(content.IndexOf(':') + 1).Trim().TrimEnd('\\');
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
                    logger.Error(MessageUtil.GetExceptionMsg(ex,""));
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

        public void Dispose()
        {
            if (this.streamReader != null)
            {
                this.streamReader.Close();
                this.streamReader = null;
            }
        }
       
    }
}
