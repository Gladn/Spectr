namespace Spectr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YourMigrationName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Devices", newName: "Device");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Device", newName: "Devices");
        }
    }
}
