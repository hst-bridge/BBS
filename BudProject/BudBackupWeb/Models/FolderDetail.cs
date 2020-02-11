using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.Models
{
    public class FolderDetail
    {
        private string _fileName;
        private string _fileSize;
        private string _fileExtensionType;
        private string _fileLastUpdateTime;
        private bool _checkState;
        private string _filePath;
        public string fileName {
            get {
                return this._fileName;
            }
            set {
                this._fileName = value;
            }
        }

        public string fileSize
        {
            get {
                return this._fileSize;
            }
            set {
                this._fileSize = value;
            }
        }
        public string fileExtensionType
        {
            get {
                return this._fileExtensionType;
            }
            set {
                this._fileExtensionType = value;
            }
        }
        public string fileLastUpdateTime
        {
            get {
                return this._fileLastUpdateTime;
            }
            set {
                this._fileLastUpdateTime = value;
            }
        }
        public bool checkState
        {
            get {
                return this._checkState;
            }
            set {
                this._checkState = value;
            }
        }
        public string filePath
        {
            get {
                return this._filePath;

            }
            set {
                this._filePath = value;
            }
        }
    }
}