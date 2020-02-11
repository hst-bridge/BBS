using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IBLL;
using BudLogManage.Common.Config;
using BudLogManage.Model;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于解析Robocopy命令产生的日志
    /// </summary>
    public class FastLogAnalyser : ILogAnalyser
    {
       //private string logPath = ConfigLoader.GetRoboCopyLogPath();
        public void Analyse(Config config)
       {
           //读日志

           //预处理
           //解析
           //存数据库
       }

        public Total GetTotal(TimeInterval ti)
        {
            return null;
        }

        #region status
        private int _filesTotalCount = 0;
        public int FilesTotalCount
        {
            get
            {

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
            set
            {
                _filesReadedCount = value;
            }
        }
        #endregion
    }
}
