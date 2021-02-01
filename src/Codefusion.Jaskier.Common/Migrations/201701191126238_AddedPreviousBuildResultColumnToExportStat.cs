namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPreviousBuildResultColumnToExportStat : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ExportStats", "PreviousBuildResult", c => c.Int());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.ExportStats", "PreviousBuildResult");
        }
    }
}
