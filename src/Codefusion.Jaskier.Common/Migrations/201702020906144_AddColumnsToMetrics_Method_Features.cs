namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnsToMetrics_Method_Features : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PredictionModels", "Method", c => c.String(maxLength: 100));
            AddColumn("dbo.PredictionModels", "Features", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PredictionModels", "Features");
            DropColumn("dbo.PredictionModels", "Method");
        }
    }
}
