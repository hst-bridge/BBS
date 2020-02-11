using System;
using System.Collections.Generic;

namespace BudBackupCopy.Entities
{
	public class monitorFileListen
	{
		public int id { get; set; }
		public int monitorServerID { get; set; }
		public string monitorFileName { get; set; }
		public string monitorType { get; set; }
		public string monitorFileDirectory { get; set; }
		public string monitorFileFullPath { get; set; }
		public System.DateTime monitorFileLastWriteTime { get; set; }
		public string monitorFileSize { get; set; }
		public string monitorFileExtension { get; set; }
		public string monitorFileLocalDirectory { get; set; }
		public string monitorFileLocalPath { get; set; }
		public System.DateTime monitorFileCreateTime { get; set; }
		public System.DateTime monitorFileLastAccessTime { get; set; }
		public string monitorStatus { get; set; }
		public System.DateTime monitorFileStartTime { get; set; }
		public System.DateTime monitorFileEndTime { get; set; }
		public short deleteFlg { get; set; }
		public string deleter { get; set; }
		public System.DateTime deleteDate { get; set; }
		public string creater { get; set; }
		public System.DateTime createDate { get; set; }
		public string updater { get; set; }
		public System.DateTime updateDate { get; set; }
	}
}

