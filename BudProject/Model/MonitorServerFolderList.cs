using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class MonitorServerFolderList
    {
        private string _monitorServerID;
        private string _nodeID;
        private string _nodeName;
        private string _parentNodeID;
        private string _parentNodeName;
        private string _folderPath;


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

        public string nodeID
        {
            get
            {
                return this._nodeID;
            }
            set
            {
                this._nodeID = value;
            }
        }
        public string nodeName
        {
            get
            {
                return this._nodeName;
            }
            set
            {
                this._nodeName = value;
            }
        }

        public string parentNodeID
        {
            get
            {
                return this._parentNodeID;
            }
            set
            {
                this._parentNodeID = value;
            }
        }
        public string parentNodeName
        {
            get
            {
                return this._parentNodeName;
            }
            set
            {
                this._parentNodeName = value;
            }
        }

        public string folderPath
        {
            get
            {
                return this._folderPath;
            }
            set
            {
                this._folderPath = value;
            }
        }
    }
}
