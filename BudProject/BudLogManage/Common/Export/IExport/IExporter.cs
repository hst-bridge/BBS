using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BudLogManage.Common.Export.IExport
{
    /// <summary>
    /// 导出文件接口
    /// </summary>
   public interface IExporter
    {
       /// <summary>
       /// 导出内容为文本解析内容
       /// 各种导出子类自动解析成自适应内容并导出
       /// 
       /// 嗯 ？ 暂时未能抽象出适合所有文件的数据结构
       /// 故为了导出csv的便利 暂时改为 datatable
       /// 待完善
       /// </summary>
       /// <param name="content"></param>
       void Export(DataTable dt,string fpath);
    }
}
