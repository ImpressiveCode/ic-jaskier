namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedTelemetriesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Telemetries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 255),
                        UserIPAddress = c.String(maxLength: 255),
                        Action = c.String(),
                        Payload = c.String(),
                        DateUtc = c.DateTime(nullable: false),
                        VisualStudioVersion = c.String(maxLength: 255),
                        PluginVersion = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Telemetries");
        }
    }
}
