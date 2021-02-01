namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactoredRbgLogic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "DistinctDeveloper", c => c.String());
            AddColumn("dbo.RbgDatas", "ExperimentDay", c => c.Int(nullable: false));
            DropColumn("dbo.Participants", "DistinctCommiterId");
            DropTable("dbo.DistinctCommiters");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DistinctCommiters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ActiveDaysCounter = c.Int(nullable: false),
                        LastActiveDateUtc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Participants", "DistinctCommiterId", c => c.Int());
            DropColumn("dbo.RbgDatas", "ExperimentDay");
            DropColumn("dbo.Participants", "DistinctDeveloper");
        }
    }
}
