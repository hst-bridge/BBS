using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BudFileCheckListen.Entities;

namespace BudFileCheckListen.Mapping
{
	public class userInfoMap : EntityTypeConfiguration<userInfo>
	{
		public userInfoMap()
		{
			// Primary Key
			this.HasKey(t => t.id);

			// Properties
			this.Property(t => t.loginID)
				.IsRequired()
				.HasMaxLength(40);
				
			this.Property(t => t.password)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.name)
				.IsRequired()
				.HasMaxLength(255);
				
			this.Property(t => t.mail)
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
			this.ToTable("userInfo");
			this.Property(t => t.id).HasColumnName("id");
			this.Property(t => t.loginID).HasColumnName("loginID");
			this.Property(t => t.password).HasColumnName("password");
			this.Property(t => t.name).HasColumnName("name");
			this.Property(t => t.mail).HasColumnName("mail");
			this.Property(t => t.mailFlg).HasColumnName("mailFlg");
			this.Property(t => t.authorityFlg).HasColumnName("authorityFlg");
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

