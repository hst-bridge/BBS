using System;
using System.Collections.Generic;

namespace BudFileCheckListen.Entities
{
	public class backupServerFile
	{
		public int id { get; set; }
		public int backupServerGroupID { get; set; }
		public int backupServerID { get; set; }
		public string backupServerFileName { get; set; }
		public string backupServerFilePath { get; set; }
		public string backupServerFileType { get; set; }
		public string backupServerFileSize { get; set; }
		public System.DateTime backupStartTime { get; set; }
		public System.DateTime backupEndTime { get; set; }
		public string backupTime { get; set; }
		public short backupFlg { get; set; }
		public System.DateTime copyStartTime { get; set; }
		public System.DateTime copyEndTime { get; set; }
		public string copyTime { get; set; }
		public short copyFlg { get; set; }
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

