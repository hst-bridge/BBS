using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Model
{
    /// <summary>
    /// 用于描述处理状态
    /// </summary>
   public class Status
    {
       public int FilesTotalCount{get;set;}
       public int FilesReadedCount { get; set; }
    }
}
