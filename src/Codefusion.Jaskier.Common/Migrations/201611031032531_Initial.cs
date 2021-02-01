namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ExportStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        OldPath = c.String(),
                        NumberOfRevisions = c.Int(),
                        NumberOfDistinctCommitters = c.Int(),
                        NumberOfModifiedLines = c.Int(),
                        BuildResult = c.Int(nullable: false),
                        Commit = c.String(),
                        BuildCommit = c.String(),
                        ExportDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            this.DropTable("dbo.ExportStats");
        }
    }
}
