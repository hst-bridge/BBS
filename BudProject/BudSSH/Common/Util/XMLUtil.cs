using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace BudSSH.Common.Util
{
    /// <summary>
    /// 用于直接操作xml
    /// </summary>
   public class XMLUtil
    {
       /// <summary>
       /// 获取节点内容
       /// </summary>
       /// <param name="xpath"></param>
       /// <param name="filepath"></param>
       /// <returns></returns>
       public static string GetInnerText(string xpath, string filepath)
       {
           string text = string.Empty;
           XmlDocument xdoc = new XmlDocument();
           xdoc.Load(filepath);
           XmlNode node = xdoc.SelectSingleNode(xpath);
           if (node != null)
           {
               text = node.InnerText;
           }

           return text;
       }

       /// <summary>
       /// 设置节点内容
       /// </summary>
       /// <param name="xpath"></param>
       /// <param name="filepath"></param>
       /// <returns></returns>
       public static void SetInnerText(string xpath, string filepath,string path)
       {
           XmlDocument xdoc = new XmlDocument();
           xdoc.Load(filepath);
           XmlNode node = xdoc.SelectSingleNode(xpath);
           if (node != null)
           {
               node.InnerText = path;
               xdoc.Save(filepath);
           }
          

       }
    }
}
