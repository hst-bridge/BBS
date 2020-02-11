using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IDAL;

namespace BudLogManage.Factory
{
    /// <summary>
    ///抽象工厂：用于生成不同日志格式读取适配器
    /// </summary>
   public interface IReaderFactory
    {
        ILogReader GetRoboCopyLogReader();
        ILogReader GetSSHLogReader();
    }
}
