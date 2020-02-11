using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class MonitorServerFile
    {
        /// <summary>
        /// 文字列のディフォルト値
        /// </summary>
        const string DEFAULTCHAR_VALUE = "";
        /// <summary>
        /// 日付時間フィールドのディフォルト値
        /// </summary>
        const string DEFAULTDATETIME_VALUE = "1900-01-01 00:00:00.000";
        /// <summary>
        /// 数字フィールドのディフォルト値
        /// </summary>
        const int DEFAULTINT_VALUE = 0; 
        
        private string _id;
        private int _monitorServerID = DEFAULTINT_VALUE;
        private string _monitorFileName = DEFAULTCHAR_VALUE;
        private string _monitorFileMD5 = DEFAULTCHAR_VALUE;
        private string _monitorFileDirectory = DEFAULTCHAR_VALUE;
        private string _monitorFilePath = DEFAULTCHAR_VALUE;
        private string _monitorFileType = DEFAULTCHAR_VALUE;
        private string _monitorFileSize = DEFAULTCHAR_VALUE;
        private string _monitorStartTime = DEFAULTDATETIME_VALUE;
        private string _monitorEndTime = DEFAULTDATETIME_VALUE;
        private int _monitorFileStatus = DEFAULTINT_VALUE;
        private int _transferFlg = DEFAULTINT_VALUE;
        private int _transferNum = DEFAULTINT_VALUE;
        private int _deleteFlg = DEFAULTINT_VALUE;
        private string _deleter = DEFAULTCHAR_VALUE;
        private string _deleteDate = DEFAULTDATETIME_VALUE;
        private string _creater = DEFAULTCHAR_VALUE;
        private string _createDate = DEFAULTDATETIME_VALUE;
        private string _updater = DEFAULTCHAR_VALUE;
        private string _updateDate = DEFAULTDATETIME_VALUE;
        private string _restorer = DEFAULTCHAR_VALUE;
        private string _restoreDate;

        public string id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public int monitorServerID
        {
            get
            {
                return this._monitorServerID;
            }
            set
            {
                this._monitorServerID = value;
            }
        }

        public string monitorFileName
        {
            get
            {
                return this._monitorFileName;
            }
            set
            {
                this._monitorFileName = value;
            }
        }

        public string monitorFileMD5
        {
            get
            {
                return this._monitorFileMD5;
            }
            set
            {
                this._monitorFileMD5 = value;
            }
        }

        public string monitorFileDirectory
        {
            get
            {
                return this._monitorFileDirectory;
            }
            set
            {
                this._monitorFileDirectory = value;
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

        public string monitorFileSize
        {
            get
            {
                return this._monitorFileSize;
            }
            set
            {
                this._monitorFileSize = value;
            }
        }

        public string monitorStartTime
        {
            get
            {
                return this._monitorStartTime;
            }
            set
            {
                this._monitorStartTime = value;
            }
        }

        public string monitorEndTime
        {
            get
            {
                return this._monitorEndTime;
            }
            set
            {
                this._monitorEndTime = value;
            }
        }

        public int monitorFileStatus
        {
            get
            {
                return this._monitorFileStatus;
            }
            set
            {
                this._monitorFileStatus = value;
            }
        }

        public int transferFlg
        {
            get
            {
                return this._transferFlg;
            }
            set
            {
                this._transferFlg = value;
            }
        }

        public int transferNum
        {
            get
            {
                return this._transferNum;
            }
            set
            {
                this._transferNum = value;
            }
        }

        public int deleteFlg
        {
            get
            {
                return this._deleteFlg;
            }
            set
            {
                this._deleteFlg = value;
            }
        }

        public string deleter
        {
            get
            {
                return this._deleter;
            }
            set
            {
                this._deleter = value;
            }
        }

        public string deleteDate
        {
            get
            {
                return this._deleteDate;
            }
            set
            {
                this._deleteDate = value;
            }
        }

        public string creater
        {
            get
            {
                return this._creater;
            }
            set
            {
                this._creater = value;
            }
        }

        public string createDate
        {
            get
            {
                return this._createDate;
            }
            set
            {
                this._createDate = value;
            }
        }

        public string updater
        {
            get
            {
                return this._updater;
            }
            set
            {
                this._updater = value;
            }
        }

        public string updateDate
        {
            get
            {
                return this._updateDate;
            }
            set
            {
                this._updateDate = value;
            }
        }

        public string restorer
        {
            get
            {
                return this._restorer;
            }
            set
            {
                this._restorer = value;
            }
        }

        public string restoreDate
        {
            get
            {
                return this._restoreDate;
            }
            set
            {
                this._restoreDate = value;
            }
        }
    }
}