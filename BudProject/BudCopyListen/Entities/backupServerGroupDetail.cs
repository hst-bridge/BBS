using System;
using System.Collections.Generic;

namespace BudCopyListen.Entities
{
	public class backupServerGroupDetail
	{
		public int id { get; set; }
		public int backupServerGroupID { get; set; }
		public int backupServerID { get; set; }
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

