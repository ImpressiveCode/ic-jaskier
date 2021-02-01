namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Added_BuildCommitDateTimeLocal : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ExportStats", "BuildCommitDateTimeLocal", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.ExportStats", "BuildCommitDateTimeLocal");
        }
    }
}
