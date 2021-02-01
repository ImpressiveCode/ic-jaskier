namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedFewColumnsInDistinctCommiterTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DistinctCommiters", "ActiveDaysCounter", c => c.Int(nullable: false));
            AddColumn("dbo.DistinctCommiters", "LastActiveDateUtc", c => c.DateTime());
            DropColumn("dbo.DistinctCommiters", "DaysCounter");
            DropColumn("dbo.DistinctCommiters", "LastActiveDay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DistinctCommiters", "LastActiveDay", c => c.DateTime(nullable: false));
            AddColumn("dbo.DistinctCommiters", "DaysCounter", c => c.Int(nullable: false));
            DropColumn("dbo.DistinctCommiters", "LastActiveDateUtc");
            DropColumn("dbo.DistinctCommiters", "ActiveDaysCounter");
        }
    }
}
