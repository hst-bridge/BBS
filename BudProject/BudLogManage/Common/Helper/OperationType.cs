using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Common.Helper
{
    /// <summary>
    /// 操作类型 1.复制到本地 2.传送到远端
    /// </summary>
   public enum OperationType
    {
       ToLocal,//复制到本地
       ToRemote,//传送
       Unknown
    }
}
