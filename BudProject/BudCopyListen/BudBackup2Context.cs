using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using BudCopyListen.Entities;
using BudCopyListen.Mapping;

namespace BudCopyListen
{
	public class BudBackup2Context : DbContext
	{
		static BudBackup2Context()
		{ 
			Database.SetInitializer<BudBackup2Context>(null);
		}

		public DbSet<backupServer> backupServers { get; set; }
		public DbSet<backupServerFile> backupServerFiles { get; set; }
		public DbSet<backupServerGroup> backupServerGroups { get; set; }
		public DbSet<backupServerGroupDetail> backupServerGroupDetails { get; set; }
		public DbSet<fileTypeSet> fileTypeSets { get; set; }
		public DbSet<log> logs { get; set; }
		public DbSet<manualBackupServer> manualBackupServers { get; set; }
		public DbSet<monitorBackupServer> monitorBackupServers { get; set; }
		public DbSet<monitorFileListen> monitorFileListens { get; set; }
		public DbSet<monitorServer> monitorServers { get; set; }
		public DbSet<monitorServerFile> monitorServerFiles { get; set; }
		public DbSet<monitorServerFolder> monitorServerFolders { get; set; }
		public DbSet<userInfo> userInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
			modelBuilder.Configurations.Add(new backupServerMap());
			modelBuilder.Configurations.Add(new backupServerFileMap());
			modelBuilder.Configurations.Add(new backupServerGroupMap());
			modelBuilder.Configurations.Add(new backupServerGroupDetailMap());
			modelBuilder.Configurations.Add(new fileTypeSetMap());
			modelBuilder.Configurations.Add(new logMap());
			modelBuilder.Configurations.Add(new manualBackupServerMap());
			modelBuilder.Configurations.Add(new monitorBackupServerMap());
			modelBuilder.Configurations.Add(new monitorFileListenMap());
			modelBuilder.Configurations.Add(new monitorServerMap());
			modelBuilder.Configurations.Add(new monitorServerFileMap());
			modelBuilder.Configurations.Add(new monitorServerFolderMap());
			modelBuilder.Configurations.Add(new userInfoMap());
		}
	}
}

