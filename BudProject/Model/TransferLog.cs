using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class TransferLog
    {
        private string _transferDate;
        private string _transferTime;
        private string _transferFileCount;
        private string _transferFileSize;

        public string transferDate {
            get {
                return this._transferDate;
            }
            set {
                this._transferDate= value;
            }
        }

        public string transferTime {
            get {
                return this._transferTime;
            }
            set {
                this._transferTime = value;
            }
        }
        public string transferFileCount
        {
            get {
                return this._transferFileCount;
            }
            set {
                this._transferFileCount = value;
            }
        }
        public string transferFileSize
        {
            get {
                return this._transferFileSize;
            }
            set {
                this._transferFileSize = value;
            }
        }
    }
}
