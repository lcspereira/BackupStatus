namespace BackupStatus.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueNameAndAddress2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hosts", "Name", c => c.String(nullable: false, maxLength: 15, unicode: false));
            AlterColumn("dbo.Hosts", "Address", c => c.String(nullable: false, maxLength: 15, unicode: false));
            CreateIndex("dbo.Hosts", "Name", unique: true);
            CreateIndex("dbo.Hosts", "Address", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hosts", new[] { "Address" });
            DropIndex("dbo.Hosts", new[] { "Name" });
            AlterColumn("dbo.Hosts", "Address", c => c.String());
            AlterColumn("dbo.Hosts", "Name", c => c.String());
        }
    }
}
