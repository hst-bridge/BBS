using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IDAL;
using BudLogManage.DAL;

namespace BudLogManage.Factory
{
    public class FastReaderFactory : IReaderFactory
    {
        public ILogReader GetRoboCopyLogReader()
        {
            return new FastRoboCopyLogReader();
        }
        public ILogReader GetSSHLogReader()
        {
            return new SSHLogReader();
        }
    }
}
