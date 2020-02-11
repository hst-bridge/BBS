using System;
using System.Collections.Generic;

namespace BudFileCheckListen.Entities
{
	public class fileTypeSet
	{
		public int id { get; set; }
		public string monitorServerFolderName { get; set; }
		public int monitorServerID { get; set; }
		public string exceptAttributeFlg1 { get; set; }
		public string exceptAttribute1 { get; set; }
		public string exceptAttributeFlg2 { get; set; }
		public string exceptAttribute2 { get; set; }
		public string exceptAttributeFlg3 { get; set; }
		public string exceptAttribute3 { get; set; }
		public short systemFileFlg { get; set; }
		public short hiddenFileFlg { get; set; }
		public string attribute1 { get; set; }
		public string attribute2 { get; set; }
		public string attribute3 { get; set; }
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

