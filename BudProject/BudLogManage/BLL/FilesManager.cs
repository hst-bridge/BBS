using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;
using System.IO;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于获取
    /// </summary>
    public class FilesManager
    {
        /// <summary>
        /// 专门用于获取所有日志文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<string> GetFiles(Config config)
        {
            List<string> files = new List<string>();
            if (!string.IsNullOrWhiteSpace(config.LogPath))
            {
                //获取所有文件路径
                string[] paths = config.LogPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var knownKeys = new HashSet<string>();
                foreach (var path in paths)
                {
                  //避免路径重复
                  if(!knownKeys.Add(path))continue;
                  files.AddRange(GetLogFiles(config, path));
                }
            }
            return files;
        }

        /// <summary>
        /// 跟据配置中的文件夹，过滤log
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="path">日志路径</param>
        /// <returns>过滤后的日志列表</returns>
        private static List<string> GetLogFiles(Config config, string path)
        {
            List<string> files = new List<string>();
            string[] fs = Directory.GetFiles(path, "*.log");
            //没设定正确的过滤条件，直接返回文件列表
            if (String.IsNullOrWhiteSpace(config.Folder) || !Directory.Exists(config.Folder.Trim()))
            {
                files.AddRange(fs);
            }
            else
            {
                string folder = config.Folder.Trim();
                foreach (string f in fs)
                {
                    string content = string.Empty;
                    int line = 0;
                    StreamReader Reader = null;
                    try
                    {
                        Reader = new StreamReader(f, BudLogManage.Common.Culture.CultureManager.GetEncoding());
                        //只读前十行
                        while (!Reader.EndOfStream && line < 10)
                        {
                            content = Reader.ReadLine().Trim();
                            line++;
                            if (content.Contains(folder))
                            {
                                files.Add(f);
                                break;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error(ex.Message);
                    }
                    finally
                    {
                        if (Reader != null)
                        {
                            Reader.Close();
                        }
                    }
                }
            }
            return files;
        }
    }
}
