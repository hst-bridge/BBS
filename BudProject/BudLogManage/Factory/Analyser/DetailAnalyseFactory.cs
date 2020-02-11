using BudLogManage.DAL.IBLL;
using BudLogManage.BLL;

namespace BudLogManage.Factory
{
    /// <summary>
    /// 默认解析组件工厂
    /// </summary>
    class DetailAnalyseFactory : IAnalyseFactory
    {

        /// <summary>
        /// 获取日志附加适配器
        /// </summary>
        /// <returns></returns>
        public ILogAnalyser GetLogAnalyser()
        {
            return new DetailLogAnalyser(GetLogReaderFactory());
        }
        /// <summary>
        /// 获取日志直接读取适配器
        /// </summary>
        /// <returns></returns>
      public IReaderFactory GetLogReaderFactory()
        {
            return new DetailReaderFactory();
        }
    }
}
