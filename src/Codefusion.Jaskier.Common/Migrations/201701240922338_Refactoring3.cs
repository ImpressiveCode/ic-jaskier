namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Refactoring3 : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Metrics", "ProjectName", c => c.String());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.Metrics", "ProjectName");
        }
    }
}
