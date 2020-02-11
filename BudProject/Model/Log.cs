using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class Log
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
        private string _monitorFileStatus = DEFAULTCHAR_VALUE;
        private string _monitorFilePath = DEFAULTCHAR_VALUE;
        private string _monitorFileType = DEFAULTCHAR_VALUE;
        private string _monitorFileSize = DEFAULTCHAR_VALUE;
        private string _monitorTime = DEFAULTDATETIME_VALUE;
        private int _transferFlg = DEFAULTINT_VALUE;
        private int _backupServerGroupID = DEFAULTINT_VALUE;
        private int _backupServerID = DEFAULTINT_VALUE;
        private string _backupServerFileName = DEFAULTCHAR_VALUE;
        private string _backupServerFilePath = DEFAULTCHAR_VALUE;
        private string _backupServerFileType = DEFAULTCHAR_VALUE;
        private string _backupServerFileSize = DEFAULTCHAR_VALUE;
        private string _backupStartTime = DEFAULTDATETIME_VALUE;
        private string _backupEndTime = DEFAULTDATETIME_VALUE;
        private string _backupTime = DEFAULTDATETIME_VALUE;
        private int _backupFlg = DEFAULTINT_VALUE;
        private string _copyStartTime = DEFAULTDATETIME_VALUE;
        private string _copyEndTime = DEFAULTDATETIME_VALUE;
        private string _copyTime = DEFAULTCHAR_VALUE;
        private int _copyFlg = DEFAULTINT_VALUE;
        private int _deleteFlg = DEFAULTINT_VALUE;
        private string _deleter = DEFAULTCHAR_VALUE;
        private string _deleteDate = DEFAULTDATETIME_VALUE;
        private string _creater = DEFAULTCHAR_VALUE;
        private string _createDate = DEFAULTDATETIME_VALUE;
        private string _updater = DEFAULTCHAR_VALUE;
        private string _updateDate = DEFAULTDATETIME_VALUE;
        private string _restorer = DEFAULTCHAR_VALUE;
        private string _restoreDate = DEFAULTDATETIME_VALUE;
        private int _totalCount = DEFAULTINT_VALUE;

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

        public string monitorFileStatus
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

        public string monitorTime
        {
            get
            {
                return this._monitorTime;
            }
            set
            {
                this._monitorTime = value;
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

        public int backupServerGroupID
        {
            get
            {
                return this._backupServerGroupID;
            }
            set
            {
                this._backupServerGroupID = value;
            }
        }

        public int backupServerID
        {
            get
            {
                return this._backupServerID;
            }
            set
            {
                this._backupServerID = value;
            }
        }

        public string backupServerFileName
        {
            get
            {
                return this._backupServerFileName;
            }
            set
            {
                this._backupServerFileName = value;
            }
        }

        public string backupServerFilePath
        {
            get
            {
                return this._backupServerFilePath;
            }
            set
            {
                this._backupServerFilePath = value;
            }
        }

        public string backupServerFileType
        {
            get
            {
                return this._backupServerFileType;
            }
            set
            {
                this._backupServerFileType = value;
            }
        }

        public string backupServerFileSize
        {
            get
            {
                return this._backupServerFileSize;
            }
            set
            {
                this._backupServerFileSize = value;
            }
        }

        public string backupStartTime
        {
            get
            {
                return this._backupStartTime;
            }
            set
            {
                this._backupStartTime = value;
            }
        }
        public string backupEndTime
        {
            get
            {
                return this._backupEndTime;
            }
            set
            {
                this._backupEndTime = value;
            }
        }
        public string backupTime
        {
            get
            {
                return this._backupTime;
            }
            set
            {
                this._backupTime = value;
            }
        }
        public int backupFlg
        {
            get
            {
                return this._backupFlg;
            }
            set
            {
                this._backupFlg = value;
            }
        }
        public string copyStartTime
        {
            get
            {
                return this._copyStartTime;
            }
            set
            {
                this._copyStartTime = value;
            }
        }
        public string copyEndTime
        {
            get
            {
                return this._copyEndTime;
            }
            set
            {
                this._copyEndTime = value;
            }
        }
        public string copyTime
        {
            get
            {
                return this._copyTime;
            }
            set
            {
                this._copyTime = value;
            }
        }
        public int copyFlg
        {
            get
            {
                return this._copyFlg;
            }
            set
            {
                this._copyFlg = value;
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

        public int totalCount
        {
            get
            {
                return this._totalCount;
            }
            set
            {
                this._totalCount = value;
            }
        }
    }
}
