using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudCopyListen.DBInterface
{
    public interface IFileTypeSetService
    {
        /// <summary>
        /// 除外条件の抽出
        /// </summary>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        List<Entities.fileTypeSet> GetByFileTypeSet(int monitorServerID, int deleteFlg);
    }
}
