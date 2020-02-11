using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class UserInfo
    {
        private string _id;
        private string _loginID;
        private string _password;
        private string _name;
        private string _mail;
        private int _mailFlg;
        private int _authorityFlg;
        private int _deleteFlg;
        private string _deleter;
        private string _deleteDate;
        private string _creater;
        private string _createDate;
        private string _updater;
        private string _updateDate;
        private string _restorer;
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

        public string loginID
        {
            get
            {
                return this._loginID;
            }
            set
            {
                this._loginID = value;
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

        public string name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public string mail
        {
            get
            {
                return this._mail;
            }
            set
            {
                this._mail = value;
            }
        }

        public int mailFlg
        {
            get
            {
                return this._mailFlg;
            }
            set
            {
                this._mailFlg = value;
            }
        }

        public int authorityFlg
        {
            get
            {
                return this._authorityFlg;
            }
            set
            {
                this._authorityFlg = value;
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