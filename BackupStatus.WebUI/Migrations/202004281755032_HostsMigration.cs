namespace BackupStatus.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HostsMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hosts", "LastStatusUpdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hosts", "LastStatusUpdate", c => c.DateTime(nullable: false));
        }
    }
}
