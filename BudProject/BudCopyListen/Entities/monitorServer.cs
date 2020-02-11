using System;
using System.Collections.Generic;

namespace BudCopyListen.Entities
{
	public class monitorServer
	{
		public int id { get; set; }
		public string monitorServerName { get; set; }
		public string monitorServerIP { get; set; }
		public short monitorSystem { get; set; }
		public string memo { get; set; }
		public string account { get; set; }
		public string password { get; set; }
		public string startFile { get; set; }
		public string monitorDrive { get; set; }
		public string monitorDriveP { get; set; }
		public string monitorMacPath { get; set; }
		public string monitorLocalPath { get; set; }
		public short copyInit { get; set; }
		public short deleteFlg { get; set; }
		public string deleter { get; set; }
		public System.DateTime deleteDate { get; set; }
		public string creater { get; set; }
		public System.DateTime createDate { get; set; }
		public string updater { get; set; }
		public System.DateTime updateDate { get; set; }
		public string restorer { get; set; }
		public System.DateTime restoreDate { get; set; }
		public short synchronismFlg { get; set; }
	}
}

