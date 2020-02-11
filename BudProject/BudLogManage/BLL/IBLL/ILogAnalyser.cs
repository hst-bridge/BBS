using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;

namespace BudLogManage.DAL.IBLL
{
    /// <summary>
    /// 日志附加适配器接口
    /// 将日志数据解析 并保存到数据库
    /// </summary>
    public interface ILogAnalyser
    {
        void Analyse(Config config);
        Total GetTotal(TimeInterval ti);
        int FilesTotalCount { get; set; }
        int FilesReadedCount { get; set; }
    }
}
