using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class BackupServerGroupDetail
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
        private int _backupServerGroupId = DEFAULTINT_VALUE;
        private int _backupServerId = DEFAULTINT_VALUE;
        private int _deleteFlg = DEFAULTINT_VALUE;
        private string _deleter = DEFAULTCHAR_VALUE;
        private string _deleteDate = DEFAULTDATETIME_VALUE;
        private string _creater = DEFAULTCHAR_VALUE;
        private string _createDate = DEFAULTDATETIME_VALUE;
        private string _updater = DEFAULTCHAR_VALUE;
        private string _updateDate = DEFAULTDATETIME_VALUE;
        private string _restorer = DEFAULTCHAR_VALUE;
        private string _restoreDate = DEFAULTDATETIME_VALUE;

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

        public int backupServerGroupId
        {
            get
            {
                return this._backupServerGroupId;
            }
            set
            {
                this._backupServerGroupId = value;
            }
        }

        public int backupServerId
        {
            get
            {
                return this._backupServerId;
            }
            set
            {
                this._backupServerId = value;
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
