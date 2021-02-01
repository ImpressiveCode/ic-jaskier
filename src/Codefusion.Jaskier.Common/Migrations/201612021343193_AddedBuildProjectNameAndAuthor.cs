namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedBuildProjectNameAndAuthor : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ExportStats", "BuildProjectName", c => c.String());
            this.AddColumn("dbo.ExportStats", "Author", c => c.String());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.ExportStats", "Author");
            this.DropColumn("dbo.ExportStats", "BuildProjectName");
        }
    }
}
