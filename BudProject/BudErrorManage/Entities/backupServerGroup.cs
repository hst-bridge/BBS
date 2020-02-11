using System;
using System.Collections.Generic;

namespace BudErrorManage.Entities
{
	public class backupServerGroup
	{
		public int id { get; set; }
		public string backupServerGroupName { get; set; }
		public int monitorServerID { get; set; }
		public string memo { get; set; }
		public short deleteFlg { get; set; }
		public string deleter { get; set; }
		public System.DateTime deleteDate { get; set; }
		public string creater { get; set; }
		public System.DateTime createDate { get; set; }
		public string updater { get; set; }
		public System.DateTime updateDate { get; set; }
		public string restorer { get; set; }
		public System.DateTime restoreDate { get; set; }
	}
}

