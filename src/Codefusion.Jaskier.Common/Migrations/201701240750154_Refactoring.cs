namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Refactoring : DbMigration
    {
        public override void Up()
        {
            this.RenameTable(name: "dbo.ExportStats", newName: "Metrics");
            this.RenameTable(name: "dbo.RequestCaches", newName: "PredictionRequests");
            this.DropPrimaryKey("dbo.Metrics");
            this.DropPrimaryKey("dbo.PredictionModels");
            this.DropPrimaryKey("dbo.PredictionRequests");
            this.AddColumn("dbo.PredictionModels", "CVK", c => c.Int());
            this.AddColumn("dbo.PredictionModels", "CVAUC", c => c.Double());
            this.AddColumn("dbo.PredictionModels", "AUC", c => c.Double());
            this.AddColumn("dbo.PredictionModels", "CMACC", c => c.Double());
            this.AddColumn("dbo.PredictionModels", "CMA", c => c.Int());
            this.AddColumn("dbo.PredictionModels", "CMB", c => c.Int());
            this.AddColumn("dbo.PredictionModels", "CMC", c => c.Int());
            this.AddColumn("dbo.PredictionModels", "CMD", c => c.Int());
            this.AddColumn("dbo.PredictionRequests", "ProjectName", c => c.String());
            this.AddColumn("dbo.PredictionRequests", "PredictionModelId", c => c.Long());
            this.AlterColumn("dbo.Metrics", "Id", c => c.Long(nullable: false, identity: true));
            this.AlterColumn("dbo.PredictionModels", "Id", c => c.Long(nullable: false, identity: true));
            this.AlterColumn("dbo.PredictionRequests", "Id", c => c.Long(nullable: false, identity: true));
            this.AddPrimaryKey("dbo.Metrics", "Id");
            this.AddPrimaryKey("dbo.PredictionModels", "Id");
            this.AddPrimaryKey("dbo.PredictionRequests", "Id");
        }
        
        public override void Down()
        {
            this.DropPrimaryKey("dbo.PredictionRequests");
            this.DropPrimaryKey("dbo.PredictionModels");
            this.DropPrimaryKey("dbo.Metrics");
            this.AlterColumn("dbo.PredictionRequests", "Id", c => c.Int(nullable: false, identity: true));
            this.AlterColumn("dbo.PredictionModels", "Id", c => c.Int(nullable: false, identity: true));
            this.AlterColumn("dbo.Metrics", "Id", c => c.Int(nullable: false, identity: true));
            this.DropColumn("dbo.PredictionRequests", "PredictionModelId");
            this.DropColumn("dbo.PredictionRequests", "ProjectName");
            this.DropColumn("dbo.PredictionModels", "CMD");
            this.DropColumn("dbo.PredictionModels", "CMC");
            this.DropColumn("dbo.PredictionModels", "CMB");
            this.DropColumn("dbo.PredictionModels", "CMA");
            this.DropColumn("dbo.PredictionModels", "CMACC");
            this.DropColumn("dbo.PredictionModels", "AUC");
            this.DropColumn("dbo.PredictionModels", "CVAUC");
            this.DropColumn("dbo.PredictionModels", "CVK");
            this.AddPrimaryKey("dbo.PredictionRequests", "Id");
            this.AddPrimaryKey("dbo.PredictionModels", "Id");
            this.AddPrimaryKey("dbo.Metrics", "Id");
            this.RenameTable(name: "dbo.PredictionRequests", newName: "RequestCaches");
            this.RenameTable(name: "dbo.Metrics", newName: "ExportStats");
        }
    }
}
