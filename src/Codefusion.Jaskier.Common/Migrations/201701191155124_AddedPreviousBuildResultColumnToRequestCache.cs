namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPreviousBuildResultColumnToRequestCache : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.RequestCaches", "PreviousBuildResult", c => c.Int());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.RequestCaches", "PreviousBuildResult");
        }
    }
}
