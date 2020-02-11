using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using log4net;
using System.Reflection;

namespace BudFileCheckListen.Common
{
    public class TxtReadClass
    {
        /// <summary>
        /// ファイルパスリスト
        /// </summary>
        private List<string> filelist;
        /// <summary>
        /// 異常の場合のフラグ
        /// </summary>
        private bool ErrorFlg;
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TxtReadClass()
        {
            filelist = new List<string>();
            ErrorFlg = false;
        }

        public List<string> txtRead(string filepath)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    reader.BaseStream.Seek(0L, SeekOrigin.Begin);
                    string strLine = reader.ReadLine();
                    if (!String.IsNullOrEmpty(strLine))
                    {
                        while (!String.IsNullOrEmpty(strLine))
                        {
                            filelist.Add(strLine);
                            strLine = reader.ReadLine();
                        }
                    }
                    reader.Dispose();
                    reader.Close();
                    stream.Dispose();
                    stream.Close();
                }
            }
            catch (System.Security.SecurityException ex)
            {
                logger.Error(ex.Message);
                ErrorFlg = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                ErrorFlg = true;
            }

            return filelist;
        }

        public bool GetErrorFlg() 
        {
            return ErrorFlg;
        }
    }
}
