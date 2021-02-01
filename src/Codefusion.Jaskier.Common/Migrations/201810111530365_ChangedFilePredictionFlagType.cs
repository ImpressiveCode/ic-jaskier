namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedFilePredictionFlagType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FilePredictions", "PredictionEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FilePredictions", "PredictionEnabled", c => c.Int(nullable: false));
        }
    }
}
