using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileCheckListen.Entities;

namespace BudFileCheckListen.Mapping
{
	public class manualBackupServerMap : EntityTypeConfiguration<manualBackupServer>
	{
		public manualBackupServerMap()
		{
			// Primary Key
			this.HasKey(t => new { t.id, t.serverIP, t.account, t.password, t.drive, t.startFile, t.deleteFlg, t.creater, t.createDate, t.updater, t.updateDate, t.restorer, t.synchronismFlg });

			// Properties
			this.Property(t => t.id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				
			this.Property(t => t.serverIP)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.account)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.password)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.drive)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.startFile)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.deleter)
				.HasMaxLength(50);
				
			this.Property(t => t.deleteDate)
				.HasMaxLength(50);
				
			this.Property(t => t.creater)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.createDate)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.updater)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.updateDate)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.restorer)
				.IsRequired()
				.HasMaxLength(50);
				
			this.Property(t => t.restoreDate)
				.HasMaxLength(50);
				
			// Table & Column Mappings
			this.ToTable("manualBackupServer");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.serverIP).HasColumnName("serverIP");
			this.Property(t => t.account).HasColumnName("account");
			this.Property(t => t.password).HasColumnName("password");
			this.Property(t => t.drive).HasColumnName("drive");
			this.Property(t => t.startFile).HasColumnName("startFile");
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

