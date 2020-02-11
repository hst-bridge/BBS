using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Common.Helper;
using BudLogManage.Model;
using System.Text.RegularExpressions;

namespace BudLogManage.Common.Util
{
    /// <summary>
    /// 判断操作类型 1.复制到本地 2.传送到远端
    /// </summary>
    public class OperationTypeUtil
    {
        private static Regex localpathRegex = new Regex(@"^([a-zA-Z]:\\.*)$");
        public static OperationType GetOperationType(Operation operation)
        {
            try
            {
                if (localpathRegex.IsMatch(operation.source)) return OperationType.ToRemote;
                else return OperationType.ToLocal;

            }
            catch (System.Exception)
            {
                return OperationType.Unknown;
            }
            
        }
    }
}
