using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Model;
using Common;
using System.Data.Odbc;

namespace IDAL
{
    public interface IFileTypeSet
    {

        //int InsertFileTypeSet(FileTypeSet FileTypeSet, SqlConnection conn);

        //int UpdateFileTypeSet(FileTypeSet FileTypeSet, SqlConnection conn);

        //int DeleteFileTypeSetById(int FileTypeSetId, string loginID, SqlConnection conn);

        //IList<FileTypeSet> GetFileTypeSet(IEnumerable<SearchCondition> conditon, SqlConnection conn);

        //int GetFileTypeSetCount(IEnumerable<SearchCondition> conditon, SqlConnection conn);

        //IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, SqlConnection conn);
        ////////////////////////////////////////////////////////////ODBC connection//////////////////////////////////////////////////////////////////
        int InsertFileTypeSet(FileTypeSet FileTypeSet, OdbcConnection conn);

        int UpdateFileTypeSet(FileTypeSet FileTypeSet, OdbcConnection conn);

        int DeleteFileTypeSetById(int FileTypeSetId, string loginID, OdbcConnection conn);

        IList<FileTypeSet> GetFileTypeSet(IEnumerable<SearchCondition> conditon, OdbcConnection conn);

        int GetFileTypeSetCount(IEnumerable<SearchCondition> conditon, OdbcConnection conn);

        IList<FileTypeSet> GetFileTypeSetPage(IEnumerable<SearchCondition> conditon, int page, int pagesize, OdbcConnection conn);
    }
}
