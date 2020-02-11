using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class FileTypeSet
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
        private string _monitorServerFolderName = DEFAULTCHAR_VALUE;
        private int _monitorServerID = DEFAULTINT_VALUE;
        private string _exceptAttribute1 = DEFAULTCHAR_VALUE;
        private string _exceptAttribute2 = DEFAULTCHAR_VALUE;
        private string _exceptAttribute3 = DEFAULTCHAR_VALUE;
        private string _exceptAttributeFlg1 = DEFAULTCHAR_VALUE;
        private string _exceptAttributeFlg2 = DEFAULTCHAR_VALUE;
        private string _exceptAttributeFlg3 = DEFAULTCHAR_VALUE;
        private string _systemFileFlg = DEFAULTINT_VALUE.ToString();
        private string _hiddenFileFlg = DEFAULTINT_VALUE.ToString();
        private string _attribute1 = DEFAULTCHAR_VALUE;
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
        public string monitorServerFolderName
        {
            get
            {
                return this._monitorServerFolderName;
            }
            set
            {
                this._monitorServerFolderName = value;
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
        public string exceptAttribute1
        {
            get
            {
                return this._exceptAttribute1;
            }
            set
            {
                this._exceptAttribute1 = value;
            }
        }
        public string exceptAttribute2
        {
            get
            {
                return this._exceptAttribute2;
            }
            set
            {
                this._exceptAttribute2 = value;
            }
        }
        public string exceptAttribute3
        {
            get
            {
                return this._exceptAttribute3;
            }
            set
            {
                this._exceptAttribute3 = value;
            }
        }
        public string exceptAttributeFlg1
        {
            get
            {
                return this._exceptAttributeFlg1;
            }
            set
            {
                this._exceptAttributeFlg1 = value;
            }
        }
        public string exceptAttributeFlg2
        {
            get
            {
                return this._exceptAttributeFlg2;
            }
            set
            {
                this._exceptAttributeFlg2 = value;
            }
        }
        public string exceptAttributeFlg3
        {
            get
            {
                return this._exceptAttributeFlg3;
            }
            set
            {
                this._exceptAttributeFlg3 = value;
            }
        }
        public string systemFileFlg
        {
            get
            {
                return this._systemFileFlg;
            }
            set
            {
                this._systemFileFlg = value;
            }
        }
        public string hiddenFileFlg
        {
            get
            {
                return this._hiddenFileFlg;
            }
            set
            {
                this._hiddenFileFlg = value;
            }
        }
        public string attribute1
        {
            get
            {
                return this._attribute1;
            }
            set
            {
                this._attribute1 = value;
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
