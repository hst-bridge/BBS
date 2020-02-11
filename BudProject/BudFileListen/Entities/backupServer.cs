using System;
using System.Collections.Generic;

namespace BudFileListen.Entities
{
	public class backupServer
	{
       
		public int id { get; set; }
		public string backupServerName { get; set; }
		public string backupServerIP { get; set; }
		public string memo { get; set; }
		public string account { get; set; }
		public string password { get; set; }
		public string startFile { get; set; }
		public string ssbpath { get; set; }
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

