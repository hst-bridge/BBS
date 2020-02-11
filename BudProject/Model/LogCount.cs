using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class LogCount
    {
        private string _date;
        private string _time;
        private int _filecount;
        private long _volumn;

        public string date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
            }
        }

        public string time
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
            }
        }

        public int filecount
        {
            get
            {
                return this._filecount;
            }
            set
            {
                this._filecount = value;
            }
        }

        public long volumn
        {
            get
            {
                return this._volumn;
            }
            set
            {
                this._volumn = value;
            }
        }
    }
}
