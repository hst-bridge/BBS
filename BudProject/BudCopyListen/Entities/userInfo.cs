using System;
using System.Collections.Generic;

namespace BudCopyListen.Entities
{
	public class userInfo
	{
		public int id { get; set; }
		public string loginID { get; set; }
		public string password { get; set; }
		public string name { get; set; }
		public string mail { get; set; }
		public short mailFlg { get; set; }
		public short authorityFlg { get; set; }
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

