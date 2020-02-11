using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.Util.Message
{
    /// <summary>
    /// ヒントを得る
    /// </summary>
    class MessageUtil
    {
        /// <summary>
        /// get the message by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMessage(string key)
        {
            return Properties.Resources.ResourceManager.GetString(key);
        }
    }
}
