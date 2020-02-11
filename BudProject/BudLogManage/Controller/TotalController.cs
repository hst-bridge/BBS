using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;
using BudLogManage.Factory;
using BudLogManage.DAL.IBLL;
using BudLogManage.BLL;

namespace BudLogManage.Controller
{
    /// <summary>
    /// 合计控制器
    /// </summary>
   public class TotalController
    {
       public Total GetTotal(TimeInterval ti)
       {
           //根据配置，选择工厂
           IAnalyseFactory IAF = AnalyseFactoryFactory.GetAnalyseFactory(ConfigManager.GetCurrentConfig());

           //获取分析器
           ILogAnalyser ila = IAF.GetLogAnalyser();

           return ila.GetTotal(ti);
       }

       public void Export(Total total, string fpath)
       {
           ExportManager em = new ExportManager();
           em.Export(total, fpath);
       }
    }
}
