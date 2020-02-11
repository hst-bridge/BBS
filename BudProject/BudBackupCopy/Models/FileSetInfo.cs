using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BudBackupCopy.Models
{
    public class FileSetInfo
    {
        /// <summary>
        /// ディレクトリパス
        /// </summary>
        public List<string> DirectoryPathList = new List<string>();
        /// <summary>
        /// ファイル拡張子リスト
        /// </summary>
        public Hashtable DirectoryFileExtensionList = new Hashtable();
    }
}
