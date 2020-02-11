using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudErrorManage.Entities;

namespace BudErrorManage.Mapping
{
	public class backupServerGroupMap : EntityTypeConfiguration<backupServerGroup>
	{
		public backupServerGroupMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.backupServerGroupName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.memo)
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
			this.ToTable("backupServerGroup");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.backupServerGroupName).HasColumnName("backupServerGroupName");
			this.Property(t => t.monitorServerID).HasColumnName("monitorServerID");
			this.Property(t => t.memo).HasColumnName("memo");
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

