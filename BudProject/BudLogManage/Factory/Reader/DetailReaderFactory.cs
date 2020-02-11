using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IDAL;
using BudLogManage.DAL;

namespace BudLogManage.Factory
{
   public class DetailReaderFactory:IReaderFactory
    {
       public ILogReader GetRoboCopyLogReader()
       {
           return new RoboCopyLogReader();
       }

       public ILogReader GetSSHLogReader()
       {
           return new SSHLogReader();
       }
    }
}
