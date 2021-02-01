namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCCMColumnsToPredictionRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PredictionRequests", "CCMMax", c => c.Int());
            AddColumn("dbo.PredictionRequests", "CCMAvg", c => c.Double());
            AddColumn("dbo.PredictionRequests", "CCMMd", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PredictionRequests", "CCMMd");
            DropColumn("dbo.PredictionRequests", "CCMAvg");
            DropColumn("dbo.PredictionRequests", "CCMMax");
        }
    }
}
