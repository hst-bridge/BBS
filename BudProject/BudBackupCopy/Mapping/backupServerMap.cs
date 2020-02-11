using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudBackupCopy.Entities;

namespace BudBackupCopy.Mapping
{
	public class backupServerMap : EntityTypeConfiguration<backupServer>
	{
		public backupServerMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.backupServerName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.backupServerIP)
				.IsRequired()
				.HasMaxLength(40);
				
			this.Property(t => t.memo)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.account)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.password)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.startFile)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.ssbpath)
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
			this.ToTable("backupServer");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.backupServerName).HasColumnName("backupServerName");
			this.Property(t => t.backupServerIP).HasColumnName("backupServerIP");
			this.Property(t => t.memo).HasColumnName("memo");
			this.Property(t => t.account).HasColumnName("account");
			this.Property(t => t.password).HasColumnName("password");
			this.Property(t => t.startFile).HasColumnName("startFile");
			this.Property(t => t.ssbpath).HasColumnName("ssbpath");
			this.Property(t => t.deleteFlg).HasColumnName("deleteFlg");
			this.Property(t => t.deleter).HasColumnName("deleter");
			this.Property(t => t.deleteDate).HasColumnName("deleteDate");
			this.Property(t => t.creater).HasColumnName("creater");
			this.Property(t => t.createDate).HasColumnName("createDate");
			this.Property(t => t.updater).HasColumnName("updater");
			this.Property(t => t.updateDate).HasColumnName("updateDate");
			this.Property(t => t.restorer).HasColumnName("restorer");
			this.Property(t => t.restoreDate).HasColumnName("restoreDate");
		}
	}
}

