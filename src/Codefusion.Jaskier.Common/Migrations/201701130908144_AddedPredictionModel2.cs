namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPredictionModel2 : DbMigration
    {
        public override void Up()
        {
            this.AlterColumn("dbo.RequestCaches", "BuildResultPredictionClass", c => c.Boolean());
            this.AlterColumn("dbo.RequestCaches", "BuildResultFaildPrediction", c => c.Double());
        }
        
        public override void Down()
        {
            this.AlterColumn("dbo.RequestCaches", "BuildResultFaildPrediction", c => c.Double(nullable: false));
            this.AlterColumn("dbo.RequestCaches", "BuildResultPredictionClass", c => c.Boolean(nullable: false));
        }
    }
}
