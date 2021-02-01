namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPredictionModel : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.PredictionModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Model = c.Binary(),
                        CreateDateUtc = c.DateTime(nullable: false),
                        ProjectName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            this.CreateTable(
                "dbo.RequestCaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BuildResultPredictionClass = c.Boolean(nullable: false),
                        BuildResultFaildPrediction = c.Double(nullable: false),
                        Guid = c.String(),
                        Path = c.String(),
                        OldPath = c.String(),
                        TotalNumberOfRevisions = c.Int(),
                        NumberOfRevisions = c.Int(),
                        NumberOfDistinctCommitters = c.Int(),
                        NumberOfModifiedLines = c.Int(),
                        Commit = c.String(),
                        BuildCommit = c.String(),
                        ExportDateUtc = c.DateTime(nullable: false),
                        BuildDateTimeLocal = c.DateTime(nullable: false),
                        BuildCommitDateTimeLocal = c.DateTime(nullable: false),
                        BuildProjectName = c.String(),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            this.DropTable("dbo.RequestCaches");
            this.DropTable("dbo.PredictionModels");
        }
    }
}
