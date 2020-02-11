using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudErrorManage.SSH
{
    public class MacFileInfo
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// ディレクトリパス
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }
    }
}
