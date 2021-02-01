namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ReduceColumnLengths : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Metrics", "Path", c => c.String(maxLength: 500));
            AlterColumn("dbo.Metrics", "OldPath", c => c.String(maxLength: 500));
            AlterColumn("dbo.Metrics", "Commit", c => c.String(maxLength: 255));
            AlterColumn("dbo.Metrics", "BuildCommit", c => c.String(maxLength: 255));
            AlterColumn("dbo.Metrics", "BuildProjectName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Metrics", "Author", c => c.String(maxLength: 255));
            AlterColumn("dbo.Metrics", "ProjectName", c => c.String(maxLength: 255));
            AlterColumn("dbo.PredictionModels", "ProjectName", c => c.String(maxLength: 255));
            AlterColumn("dbo.PredictionRequests", "Guid", c => c.String(maxLength: 255));
            AlterColumn("dbo.PredictionRequests", "Path", c => c.String(maxLength: 500));
            AlterColumn("dbo.PredictionRequests", "OldPath", c => c.String(maxLength: 500));
            AlterColumn("dbo.PredictionRequests", "Author", c => c.String(maxLength: 255));
            AlterColumn("dbo.PredictionRequests", "ProjectName", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.PredictionRequests", "ProjectName", c => c.String());
            AlterColumn("dbo.PredictionRequests", "Author", c => c.String());
            AlterColumn("dbo.PredictionRequests", "OldPath", c => c.String());
            AlterColumn("dbo.PredictionRequests", "Path", c => c.String());
            AlterColumn("dbo.PredictionRequests", "Guid", c => c.String());
            AlterColumn("dbo.PredictionModels", "ProjectName", c => c.String());
            AlterColumn("dbo.Metrics", "ProjectName", c => c.String());
            AlterColumn("dbo.Metrics", "Author", c => c.String());
            AlterColumn("dbo.Metrics", "BuildProjectName", c => c.String());
            AlterColumn("dbo.Metrics", "BuildCommit", c => c.String());
            AlterColumn("dbo.Metrics", "Commit", c => c.String());
            AlterColumn("dbo.Metrics", "OldPath", c => c.String());
            AlterColumn("dbo.Metrics", "Path", c => c.String());
        }
    }
}
