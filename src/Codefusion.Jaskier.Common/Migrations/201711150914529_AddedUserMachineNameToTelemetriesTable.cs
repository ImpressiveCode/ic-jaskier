namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserMachineNameToTelemetriesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Telemetries", "UserMachineName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Telemetries", "UserMachineName");
        }
    }
}
