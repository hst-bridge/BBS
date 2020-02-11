using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IBLL;
using BudLogManage.DAL.IDAL;

namespace BudLogManage.Factory
{
    /// <summary>
    /// 抽象工厂
    /// </summary>
    interface IAnalyseFactory
    {
        /// <summary>
        /// 获取日志附加适配器
        /// </summary>
        /// <returns></returns>
        ILogAnalyser GetLogAnalyser();
        /// <summary>
        /// 获取日志直接读取适配器工厂
        /// </summary>
        /// <returns></returns>
        IReaderFactory GetLogReaderFactory();
    }
}
