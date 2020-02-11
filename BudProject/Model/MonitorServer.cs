using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class MonitorServer
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
        private string _DBServerIP = null;
        private string _monitorServerName = DEFAULTCHAR_VALUE;
        private string _monitorServerIP = DEFAULTCHAR_VALUE;
        private int _monitorSystem = DEFAULTINT_VALUE;
        private string _memo = DEFAULTCHAR_VALUE;
        private string _account = DEFAULTCHAR_VALUE;
        private string _password = DEFAULTCHAR_VALUE;
        private string _startFile = DEFAULTCHAR_VALUE;
        private string _monitorDrive = DEFAULTCHAR_VALUE;
        private string _monitorDriveP = DEFAULTCHAR_VALUE;
        private string _monitorMacPath = DEFAULTCHAR_VALUE;
        private string _monitorLocalPath = DEFAULTCHAR_VALUE;
        private int _copyInit = DEFAULTINT_VALUE;
        private int _deleteFlg = DEFAULTINT_VALUE;
        private string _deleter = DEFAULTCHAR_VALUE;
        private string _deleteDate = DEFAULTDATETIME_VALUE;
        private string _creater = DEFAULTCHAR_VALUE;
        private string _createDate = DEFAULTDATETIME_VALUE;
        private string _updater = DEFAULTCHAR_VALUE;
        private string _updateDate = DEFAULTDATETIME_VALUE;
        private string _restorer = DEFAULTCHAR_VALUE;
        private string _restoreDate = DEFAULTDATETIME_VALUE;
        private short _synchronismFlg = DEFAULTINT_VALUE;

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

        public string monitorServerName
        {
            get
            {
                return this._monitorServerName;
            }
            set
            {
                this._monitorServerName = value;
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

        public int monitorSystem 
        {
            get 
            {
                return this._monitorSystem;
            }
            set 
            {
                this._monitorSystem = value;
            }
        }

        public string memo
        {
            get
            {
                return this._memo;
            }
            set
            {
                this._memo = value;
            }
        }

        public string account
        {
            get
            {
                return this._account;
            }
            set
            {
                this._account = value;
            }
        }

        public string password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
            }
        }

        public string startFile
        {
            get
            {
                return this._startFile;
            }
            set
            {
                this._startFile = value;
            }
        }
        
        public string monitorDrive
        {
            get
            {
                return this._monitorDrive;
            }
            set
            {
                this._monitorDrive = value;
            }
        }

        public string monitorDriveP
        {
            get
            {
                return this._monitorDriveP;
            }
            set
            {
                this._monitorDriveP = value;
            }
        }

        public string monitorMacPath
        {
            get
            {
                return this._monitorMacPath;
            }
            set
            {
                this._monitorMacPath = value;
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

        public int copyInit
        {
            get
            {
                return this._copyInit;
            }
            set
            {
                this._copyInit = value;
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

        public short synchronismFlg
        {
            get
            {
                return this._synchronismFlg;
            }
            set
            {
                this._synchronismFlg = value;
            }
        }
    }
}