using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BudLogManage.BLL.Cache
{
    /// <summary>
    /// 用于存储此次操作
    /// </summary>
    internal class Session
    {
        private Session() { }
        public static readonly Session Instance = new Session();
        private Hashtable ht = new Hashtable();
        public object this[string key]
        {
            get { return ht[key]; }
            set { ht[key] = value; }
        }
    }
}
