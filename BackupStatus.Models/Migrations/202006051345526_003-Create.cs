namespace BackupStatus.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 15, unicode: false),
                        Address = c.String(nullable: false, maxLength: 15, unicode: false),
                        ReturnCode = c.Int(nullable: false),
                        LastStatusUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Address, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hosts", new[] { "Address" });
            DropIndex("dbo.Hosts", new[] { "Name" });
            DropTable("dbo.Hosts");
        }
    }
}
