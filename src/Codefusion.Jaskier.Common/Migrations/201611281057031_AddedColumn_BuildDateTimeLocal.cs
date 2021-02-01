namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedColumn_BuildDateTimeLocal : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ExportStats", "BuildDateTimeLocal", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.ExportStats", "BuildDateTimeLocal");
        }
    }
}
