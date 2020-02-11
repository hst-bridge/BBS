using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudBackupCopy.Entities;

namespace BudBackupCopy.Mapping
{
	public class monitorFileListenMap : EntityTypeConfiguration<monitorFileListen>
	{
		public monitorFileListenMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.monitorFileName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorType)
				.IsRequired()
				.HasMaxLength(20);
				
			this.Property(t => t.monitorFileDirectory)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorFileFullPath)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorFileSize)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileExtension)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileLocalDirectory)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorFileLocalPath)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorStatus)
				.IsRequired()
				.HasMaxLength(20);
				
			this.Property(t => t.deleter)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.creater)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.updater)
				.IsRequired()
				.HasMaxLength(255);
				
			// Table & Column Mappings
			this.ToTable("monitorFileListen");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.monitorServerID).HasColumnName("monitorServerID");
			this.Property(t => t.monitorFileName).HasColumnName("monitorFileName");
			this.Property(t => t.monitorType).HasColumnName("monitorType");
			this.Property(t => t.monitorFileDirectory).HasColumnName("monitorFileDirectory");
			this.Property(t => t.monitorFileFullPath).HasColumnName("monitorFileFullPath");
			this.Property(t => t.monitorFileLastWriteTime).HasColumnName("monitorFileLastWriteTime");
			this.Property(t => t.monitorFileSize).HasColumnName("monitorFileSize");
			this.Property(t => t.monitorFileExtension).HasColumnName("monitorFileExtension");
			this.Property(t => t.monitorFileLocalDirectory).HasColumnName("monitorFileLocalDirectory");
			this.Property(t => t.monitorFileLocalPath).HasColumnName("monitorFileLocalPath");
			this.Property(t => t.monitorFileCreateTime).HasColumnName("monitorFileCreateTime");
			this.Property(t => t.monitorFileLastAccessTime).HasColumnName("monitorFileLastAccessTime");
			this.Property(t => t.monitorStatus).HasColumnName("monitorStatus");
			this.Property(t => t.monitorFileStartTime).HasColumnName("monitorFileStartTime");
			this.Property(t => t.monitorFileEndTime).HasColumnName("monitorFileEndTime");
			this.Property(t => t.deleteFlg).HasColumnName("deleteFlg");
			this.Property(t => t.deleter).HasColumnName("deleter");
			this.Property(t => t.deleteDate).HasColumnName("deleteDate");
			this.Property(t => t.creater).HasColumnName("creater");
			this.Property(t => t.createDate).HasColumnName("createDate");
			this.Property(t => t.updater).HasColumnName("updater");
			this.Property(t => t.updateDate).HasColumnName("updateDate");
		}
	}
}

