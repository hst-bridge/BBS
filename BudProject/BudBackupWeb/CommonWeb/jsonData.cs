using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace budbackup.CommonWeb
{
    /// <summary>
    /// 转义破坏json格式的特殊字符
    /// by:wangjianxu 
    /// 2012年6月17日16:47:51
    /// </summary>

    class jsonData : Hashtable
    {
        public jsonData Add(object key, object value)
        {
            base.Add(key, ConvertData(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string jsonData = "";
            string Elemnet = "\"{0}\":\"{1}\"";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            IDictionaryEnumerator ide = this.GetEnumerator();
            while (ide.MoveNext())
            {
                sb.Append(string.Format(Elemnet, ide.Key, ide.Value));
                sb.Append(",");
            }
            jsonData = sb.ToString();
            jsonData = jsonData.Substring(0, jsonData.Length - 1);
            return jsonData + "}";

        }
        private Object ConvertData(object data)
        {
            if (data == null)
            {
                return "";
            }
            else
            {
                string newData = data.ToString();
                newData = newData.Replace("\"", "\\\"").Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
                return newData;
            }
        }
    }
}
