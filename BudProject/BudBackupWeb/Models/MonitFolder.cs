using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.Models
{
    public class MonitFolder
    {
        private string _monitorFileName;
        private string _monitorFilePath;
        private string _monitorFileType;
        public string monitorFileName
        {
            get {
                return this._monitorFileName;
            }
            set {
                this._monitorFileName = value;
            }
        }
        public string monitorFilePath
        {
            get
            {
               return this._monitorFilePath;
            }
            set
            {
                this._monitorFilePath = value;
            }
        }
        public string monitorFileType
        {
            get
            {
                return this._monitorFileType;
            }
            set
            {
                this._monitorFileType = value;
            }
        }
    }
}