using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Common;

namespace IBLL
{
    public interface IFileTypeSetService
    {

        int InsertFileTypeSet(FileTypeSet FileTypeSet);

        int DeleteFileTypeSet(int FileTypeSetId, string loginID);

        int UpdateFileTypeSet(FileTypeSet FileTypeSet);
        
        FileTypeSet GetFileTypeSetId(int FileTypeSetId);

        IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> condition, int page, int pagesize);

        int GetFileTypeSetCount(IEnumerable<SearchCondition> condition);

        IList<FileTypeSet> GetFileTypeSetList();

        IList<FileTypeSet> GetFileTypeSetByMonitorServerID(string MonitorServerId);

        FileTypeSet GetFileTypeSetByMonitorServerIdAndFolderName(string MonitorServerId,string folderName);
    }
}
