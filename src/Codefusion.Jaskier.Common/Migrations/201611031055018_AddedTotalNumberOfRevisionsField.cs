namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedTotalNumberOfRevisionsField : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ExportStats", "TotalNumberOfRevisions", c => c.Int());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.ExportStats", "TotalNumberOfRevisions");
        }
    }
}
