using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.DAL.IBLL;
using BudLogManage.DAL.IDAL;
using BudLogManage.BLL;
using BudLogManage.DAL;

namespace BudLogManage.Factory
{
    /// <summary>
    /// 默认解析组件工厂
    /// </summary>
    class FastAnalyseFactory : IAnalyseFactory
    {
        /// <summary>
        /// 获取日志附加适配器
        /// </summary>
        /// <returns></returns>
        public ILogAnalyser GetLogAnalyser()
        {
            return new FastLogAnalyser();
        }
        /// <summary>
        /// 获取日志直接读取适配器工厂
        /// </summary>
        /// <returns></returns>
        public IReaderFactory GetLogReaderFactory()
        {
            return new FastReaderFactory();
        }
    }
}
