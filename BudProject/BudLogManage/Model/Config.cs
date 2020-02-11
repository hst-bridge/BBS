using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Model
{
    /// <summary>
    /// 用于承载配置,避免不断访问文件
    /// </summary>
   public class Config
    {
       /// <summary>
       /// 所有日志路径
       /// 具体是什么日志 程序自己探测
       /// </summary>
       public string LogPath { get; set; }

       /// <summary>
       /// 只查看一个拷贝文件夹的日志，或所有的
       /// </summary>
       public string Folder { get; set; }
    }
}
