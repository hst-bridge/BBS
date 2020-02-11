using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileCheckListen.Entities;

namespace BudFileCheckListen.Mapping
{
	public class monitorServerMap : EntityTypeConfiguration<monitorServer>
	{
		public monitorServerMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.monitorServerName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorServerIP)
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
				
			this.Property(t => t.monitorDrive)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorDriveP)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorMacPath)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.monitorLocalPath)
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
			this.ToTable("monitorServer");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.monitorServerName).HasColumnName("monitorServerName");
			this.Property(t => t.monitorServerIP).HasColumnName("monitorServerIP");
			this.Property(t => t.monitorSystem).HasColumnName("monitorSystem");
			this.Property(t => t.memo).HasColumnName("memo");
			this.Property(t => t.account).HasColumnName("account");
			this.Property(t => t.password).HasColumnName("password");
			this.Property(t => t.startFile).HasColumnName("startFile");
			this.Property(t => t.monitorDrive).HasColumnName("monitorDrive");
			this.Property(t => t.monitorDriveP).HasColumnName("monitorDriveP");
			this.Property(t => t.monitorMacPath).HasColumnName("monitorMacPath");
			this.Property(t => t.monitorLocalPath).HasColumnName("monitorLocalPath");
			this.Property(t => t.copyInit).HasColumnName("copyInit");
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

