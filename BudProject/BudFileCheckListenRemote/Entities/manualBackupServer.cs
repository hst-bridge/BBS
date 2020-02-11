using System;
using System.Collections.Generic;

namespace BudFileCheckListen.Entities
{
	public class manualBackupServer
	{
		public int id { get; set; }
		public string serverIP { get; set; }
		public string account { get; set; }
		public string password { get; set; }
		public string drive { get; set; }
		public string startFile { get; set; }
		public byte deleteFlg { get; set; }
		public string deleter { get; set; }
		public string deleteDate { get; set; }
		public string creater { get; set; }
		public string createDate { get; set; }
		public string updater { get; set; }
		public string updateDate { get; set; }
		public string restorer { get; set; }
		public string restoreDate { get; set; }
		public short synchronismFlg { get; set; }
	}
}

