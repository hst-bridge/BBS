using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileListen.Entities;

namespace BudFileListen.Mapping
{
	public class monitorServerFileMap : EntityTypeConfiguration<monitorServerFile>
	{
		public monitorServerFileMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.monitorFileName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileDirectory)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorFilePath)
				.IsRequired()
				.HasMaxLength(500);
				
			this.Property(t => t.monitorFileType)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorFileSize)
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
				
			// Table & Column Mappings
			this.ToTable("monitorServerFile");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.monitorServerID).HasColumnName("monitorServerID");
			this.Property(t => t.monitorFileName).HasColumnName("monitorFileName");
			this.Property(t => t.monitorFileDirectory).HasColumnName("monitorFileDirectory");
			this.Property(t => t.monitorFilePath).HasColumnName("monitorFilePath");
			this.Property(t => t.monitorFileType).HasColumnName("monitorFileType");
			this.Property(t => t.monitorFileSize).HasColumnName("monitorFileSize");
			this.Property(t => t.monitorStartTime).HasColumnName("monitorStartTime");
			this.Property(t => t.monitorEndTime).HasColumnName("monitorEndTime");
			this.Property(t => t.monitorFileStatus).HasColumnName("monitorFileStatus");
			this.Property(t => t.transferFlg).HasColumnName("transferFlg");
			this.Property(t => t.transferNum).HasColumnName("transferNum");
			this.Property(t => t.deleteFlg).HasColumnName("deleteFlg");
			this.Property(t => t.deleter).HasColumnName("deleter");
			this.Property(t => t.deleteDate).HasColumnName("deleteDate");
			this.Property(t => t.creater).HasColumnName("creater");
			this.Property(t => t.createDate).HasColumnName("createDate");
			this.Property(t => t.updater).HasColumnName("updater");
			this.Property(t => t.updateDate).HasColumnName("updateDate");
			this.Property(t => t.restorer).HasColumnName("restorer");
			this.Property(t => t.restoreDate).HasColumnName("restoreDate");
			this.Property(t => t.synchronismFlg).HasColumnName("synchronismFlg");
		}
	}
}

