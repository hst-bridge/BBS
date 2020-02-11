using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;

namespace BudSSH.DAL.IBLL
{
    /// <summary>
    /// 日志附加适配器接口
    /// </summary>
    public interface ILogAnalyser
    {
        void Analyse(Model.Config config);
        
    }
}
