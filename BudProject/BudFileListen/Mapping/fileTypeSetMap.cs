using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileListen.Entities;

namespace BudFileListen.Mapping
{
	public class fileTypeSetMap : EntityTypeConfiguration<fileTypeSet>
	{
		public fileTypeSetMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.monitorServerFolderName)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttributeFlg1)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttribute1)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttributeFlg2)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttribute2)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttributeFlg3)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.exceptAttribute3)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.attribute1)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.attribute2)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.attribute3)
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
			this.ToTable("fileTypeSet");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.monitorServerFolderName).HasColumnName("monitorServerFolderName");
			this.Property(t => t.monitorServerID).HasColumnName("monitorServerID");
			this.Property(t => t.exceptAttributeFlg1).HasColumnName("exceptAttributeFlg1");
			this.Property(t => t.exceptAttribute1).HasColumnName("exceptAttribute1");
			this.Property(t => t.exceptAttributeFlg2).HasColumnName("exceptAttributeFlg2");
			this.Property(t => t.exceptAttribute2).HasColumnName("exceptAttribute2");
			this.Property(t => t.exceptAttributeFlg3).HasColumnName("exceptAttributeFlg3");
			this.Property(t => t.exceptAttribute3).HasColumnName("exceptAttribute3");
			this.Property(t => t.systemFileFlg).HasColumnName("systemFileFlg");
			this.Property(t => t.hiddenFileFlg).HasColumnName("hiddenFileFlg");
			this.Property(t => t.attribute1).HasColumnName("attribute1");
			this.Property(t => t.attribute2).HasColumnName("attribute2");
			this.Property(t => t.attribute3).HasColumnName("attribute3");
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

