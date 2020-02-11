using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using log4net;
using System.Reflection;

namespace BudCopyListen.Common
{
    public class TxtClass
    {
        /// <summary>
        /// pathList
        /// </summary>
        private List<string> pathList;
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TxtClass()
        {
            pathList = new List<string>();
        }

        public List<string> TxtRead(string filepath)
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
                        pathList.Add(strLine);
                        strLine = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return pathList;
        }
    }
}
