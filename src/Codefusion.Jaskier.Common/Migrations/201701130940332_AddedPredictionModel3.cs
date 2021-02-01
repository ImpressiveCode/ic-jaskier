namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPredictionModel3 : DbMigration
    {
        public override void Up()
        {
            this.DropColumn("dbo.RequestCaches", "Commit");
            this.DropColumn("dbo.RequestCaches", "BuildCommit");
            this.DropColumn("dbo.RequestCaches", "ExportDateUtc");
            this.DropColumn("dbo.RequestCaches", "BuildDateTimeLocal");
            this.DropColumn("dbo.RequestCaches", "BuildCommitDateTimeLocal");
            this.DropColumn("dbo.RequestCaches", "BuildProjectName");
        }
        
        public override void Down()
        {
            this.AddColumn("dbo.RequestCaches", "BuildProjectName", c => c.String());
            this.AddColumn("dbo.RequestCaches", "BuildCommitDateTimeLocal", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.RequestCaches", "BuildDateTimeLocal", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.RequestCaches", "ExportDateUtc", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.RequestCaches", "BuildCommit", c => c.String());
            this.AddColumn("dbo.RequestCaches", "Commit", c => c.String());
        }
    }
}
