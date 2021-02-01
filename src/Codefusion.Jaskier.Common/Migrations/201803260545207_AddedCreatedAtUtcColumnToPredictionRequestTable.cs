namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreatedAtUtcColumnToPredictionRequestTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PredictionRequests", "CreatedAtUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PredictionRequests", "CreatedAtUtc");
        }
    }
}
