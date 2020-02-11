using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.Models
{
    public class DownloadSearchResult
    {
        private List<DFolderInfo> _folderInfoList = null;
        public List<DFolderInfo> FolderInfoList {
            get
            {
                if (_folderInfoList == null)
                {
                    _folderInfoList = new List<DFolderInfo>();
                }
                return _folderInfoList;
            }
            set
            {
                _folderInfoList = value;
            }
        }

        private List<DFileInfo> _fileInfoList = null;

        public List<DFileInfo> FileInfoList {
            get
            {
                if (_fileInfoList == null)
                {
                    _fileInfoList = new List<DFileInfo>();
                }
                return _fileInfoList;
            }
            set
            {
                _fileInfoList = value;
            }
        }

        public int Count
        {
            get
            {
                return FileInfoList.Count + FolderInfoList.Count;
            }
        }

        public string SearchPattern { get; set; }

        public string MonitorServerID { get; set; }
    }

    public class DFolderInfo
    {
        public string Name { get; set; }
        public string MacPath { get; set; }
        public string WinPath { get; set; }
        public string LastWriteTime { get; set; }
    }

    public class DFileInfo
    {
        public string Name { get; set; }
        public string MacPath { get; set; }
        public string WinPath { get; set; }
        public string LastWriteTime { get; set; }
    }


}