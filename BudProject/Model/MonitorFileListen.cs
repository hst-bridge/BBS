using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class MonitorFileListen
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
        private string _DBServerIP = DEFAULTCHAR_VALUE;
        private string _monitorServerID = DEFAULTCHAR_VALUE;
        private string _monitorFileName = DEFAULTCHAR_VALUE;
        private string _monitorType = DEFAULTCHAR_VALUE;
        private string _monitorServerIP = DEFAULTCHAR_VALUE;
        private string _sharePoint = DEFAULTCHAR_VALUE;
        private string _monitorLocalPath = DEFAULTCHAR_VALUE;
        private string _monitorFileRelativeDirectory = DEFAULTCHAR_VALUE;
        private string _monitorFileRelativeFullPath = DEFAULTCHAR_VALUE;
        private string _monitorFileLastWriteTime = DEFAULTCHAR_VALUE;
        private string _monitorFileSize = DEFAULTCHAR_VALUE;
        private string _monitorFileExtension = DEFAULTCHAR_VALUE;
        private string _monitorFileCreateTime = DEFAULTCHAR_VALUE;
        private string _monitorFileLastAccessTime = DEFAULTCHAR_VALUE;
        private string _monitorStatus = DEFAULTCHAR_VALUE;
        private string _monitorFileStartTime = DEFAULTCHAR_VALUE;
        private string _monitorFileEndTime = DEFAULTDATETIME_VALUE;
        private int _deleteFlg = DEFAULTINT_VALUE;
        private string _deleter = DEFAULTCHAR_VALUE;
        private string _deleteDate = DEFAULTCHAR_VALUE;
        private string _creater = DEFAULTCHAR_VALUE;
        private string _createDate = DEFAULTDATETIME_VALUE;
        private string _updater = DEFAULTCHAR_VALUE;
        private string _updateDate = DEFAULTDATETIME_VALUE;

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
        public string DBServerIP
        {
            get
            {
                return this._DBServerIP;
            }
            set
            {
                this._DBServerIP = value;
            }
        }
        public string monitorServerID
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
        public string monitorType
        {
            get
            {
                return this._monitorType;
            }
            set
            {
                this._monitorType = value;
            }
        }
        public string monitorServerIP
        {
            get
            {
                return this._monitorServerIP;
            }
            set
            {
                this._monitorServerIP = value;
            }
        }
        public string sharePoint
        {
            get
            {
                return this._sharePoint;
            }
            set
            {
                this._sharePoint = value;
            }
        }
        public string monitorLocalPath
        {
            get
            {
                return this._monitorLocalPath;
            }
            set
            {
                this._monitorLocalPath = value;
            }
        }
        public string monitorFileRelativeDirectory
        {
            get
            {
                return this._monitorFileRelativeDirectory;
            }
            set
            {
                this._monitorFileRelativeDirectory = value;
            }
        }
        public string monitorFileRelativeFullPath
        {
            get
            {
                return this._monitorFileRelativeFullPath;
            }
            set
            {
                this._monitorFileRelativeFullPath = value;
            }
        }
        public string monitorFileLastWriteTime
        {
            get
            {
                return this._monitorFileLastWriteTime;
            }
            set
            {
                this._monitorFileLastWriteTime = value;
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
        public string monitorFileExtension
        {
            get
            {
                return this._monitorFileExtension;
            }
            set
            {
                this._monitorFileExtension = value;
            }
        }
        public string monitorFileCreateTime
        {
            get
            {
                return this._monitorFileCreateTime;
            }
            set
            {
                this._monitorFileCreateTime = value;
            }
        }
        public string monitorFileLastAccessTime
        {
            get
            {
                return this._monitorFileLastAccessTime;
            }
            set
            {
                this._monitorFileLastAccessTime = value;
            }
        }
        public string monitorStatus
        {
            get
            {
                return this._monitorStatus;
            }
            set
            {
                this._monitorStatus = value;
            }
        }
        public string monitorFileStartTime
        {
            get
            {
                return this._monitorFileStartTime;
            }
            set
            {
                this._monitorFileStartTime = value;
            }
        }
        public string monitorFileEndTime
        {
            get
            {
                return this._monitorFileEndTime;
            }
            set
            {
                this._monitorFileEndTime = value;
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

    }
}
