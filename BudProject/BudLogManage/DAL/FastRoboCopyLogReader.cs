using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IDAL;
using BudLogManage.Model;

namespace BudLogManage.DAL
{
    /// <summary>
    /// RoboCopy日志快速读取适配器
    /// </summary>
    public class FastRoboCopyLogReader : ILogReader
    {
        public Operation ReadBlock()
        {
            return null;
        }
        public bool EndOfStream
        {
            get
            {
                return true;
            }
        }
        public string Path { get; set; }

        public void Close()
        {
        }
    }
}
