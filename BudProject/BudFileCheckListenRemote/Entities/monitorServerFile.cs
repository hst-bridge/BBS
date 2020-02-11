using System;
using System.Collections.Generic;

namespace BudFileCheckListen.Entities
{
	public class monitorServerFile
	{
		public int id { get; set; }
		public int monitorServerID { get; set; }
		public string monitorFileName { get; set; }
		public string monitorFileDirectory { get; set; }
		public string monitorFilePath { get; set; }
		public string monitorFileType { get; set; }
		public string monitorFileSize { get; set; }
		public System.DateTime monitorStartTime { get; set; }
		public System.DateTime monitorEndTime { get; set; }
		public short monitorFileStatus { get; set; }
		public short transferFlg { get; set; }
		public int transferNum { get; set; }
		public short deleteFlg { get; set; }
		public string deleter { get; set; }
		public System.DateTime deleteDate { get; set; }
		public string creater { get; set; }
		public System.DateTime createDate { get; set; }
		public string updater { get; set; }
		public System.DateTime updateDate { get; set; }
		public string restorer { get; set; }
		public System.DateTime restoreDate { get; set; }
		public Nullable<short> synchronismFlg { get; set; }
	}
}

