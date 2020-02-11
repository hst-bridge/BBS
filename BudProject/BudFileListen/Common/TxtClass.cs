using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using log4net;
using System.Reflection;

namespace BudFileListen.Common
{
    public class TxtClass
    {
        private List<string> fileInfoList;
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// access flg
        /// </summary>
        private bool accessflg = true;

        public TxtClass()
        {
            fileInfoList = new List<string>();
        }

        public List<string> txtRead(string filepath)
        {
            try
            {
                FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                reader.BaseStream.Seek(0L, SeekOrigin.Begin);
                string strLine = reader.ReadLine();
                if (!String.IsNullOrEmpty(strLine))
                {
                    while (!String.IsNullOrEmpty(strLine))
                    {
                        fileInfoList.Add(strLine);
                        strLine = reader.ReadLine();
                    }
                }
                reader.Dispose();
                reader.Close();
                stream.Dispose();
                stream.Close();
            }
            catch (System.Security.SecurityException ex)
            {
                accessflg = false;
                logger.Error(ex.Message);
            }
            catch (Exception ex)
            {
                accessflg = false;
                logger.Error(ex.Message);
            }

            return fileInfoList;
        }

        public bool GetAccessFlg()
        {
            return accessflg;
        }
    }
}
