using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileCheckListen.Entities;

namespace BudFileCheckListen.Mapping
{
	public class logMap : EntityTypeConfiguration<log>
	{
		public logMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.monitorFileName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFilePath)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileType)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileSize)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.backupServerFileName)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.backupServerFilePath)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.backupServerFileType)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.backupServerFileSize)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.backupTime)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.copyTime)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.deleter)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.creater)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.updater)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.restorer)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileStatus)
				.IsRequired();
				
			// Table & Column Mappings
			this.ToTable("log");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.monitorServerID).HasColumnName("monitorServerID");
			this.Property(t => t.monitorFileName).HasColumnName("monitorFileName");
			this.Property(t => t.monitorFilePath).HasColumnName("monitorFilePath");
			this.Property(t => t.monitorFileType).HasColumnName("monitorFileType");
			this.Property(t => t.monitorFileSize).HasColumnName("monitorFileSize");
			this.Property(t => t.monitorTime).HasColumnName("monitorTime");
			this.Property(t => t.transferFlg).HasColumnName("transferFlg");
			this.Property(t => t.backupServerGroupID).HasColumnName("backupServerGroupID");
			this.Property(t => t.backupServerID).HasColumnName("backupServerID");
			this.Property(t => t.backupServerFileName).HasColumnName("backupServerFileName");
			this.Property(t => t.backupServerFilePath).HasColumnName("backupServerFilePath");
			this.Property(t => t.backupServerFileType).HasColumnName("backupServerFileType");
			this.Property(t => t.backupServerFileSize).HasColumnName("backupServerFileSize");
			this.Property(t => t.backupStartTime).HasColumnName("backupStartTime");
			this.Property(t => t.backupEndTime).HasColumnName("backupEndTime");
			this.Property(t => t.backupTime).HasColumnName("backupTime");
			this.Property(t => t.backupFlg).HasColumnName("backupFlg");
			this.Property(t => t.copyStartTime).HasColumnName("copyStartTime");
			this.Property(t => t.copyEndTime).HasColumnName("copyEndTime");
			this.Property(t => t.copyTime).HasColumnName("copyTime");
			this.Property(t => t.copyFlg).HasColumnName("copyFlg");
			this.Property(t => t.deleteFlg).HasColumnName("deleteFlg");
			this.Property(t => t.deleter).HasColumnName("deleter");
			this.Property(t => t.deleteDate).HasColumnName("deleteDate");
			this.Property(t => t.creater).HasColumnName("creater");
			this.Property(t => t.createDate).HasColumnName("createDate");
			this.Property(t => t.updater).HasColumnName("updater");
			this.Property(t => t.updateDate).HasColumnName("updateDate");
			this.Property(t => t.restorer).HasColumnName("restorer");
			this.Property(t => t.restoreDate).HasColumnName("restoreDate");
			this.Property(t => t.monitorFileStatus).HasColumnName("monitorFileStatus");
			this.Property(t => t.synchronismFlg).HasColumnName("synchronismFlg");
		}
	}
}

