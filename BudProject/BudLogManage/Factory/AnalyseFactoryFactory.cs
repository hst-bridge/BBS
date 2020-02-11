using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;

namespace BudLogManage.Factory
{
    /// <summary>
    /// 生成解析组件工厂的工厂
    /// </summary>
    class AnalyseFactoryFactory
    {
        /// <summary>
        /// 根据配置获取对应的解析组件工厂
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IAnalyseFactory GetAnalyseFactory(Config config)
        {
            //return new FastAnalyseFactory();
            return new DetailAnalyseFactory();
        }
    }
}
