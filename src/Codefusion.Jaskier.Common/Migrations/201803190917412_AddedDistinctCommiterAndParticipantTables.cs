namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDistinctCommiterAndParticipantTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DistinctCommiters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DaysCounter = c.Int(nullable: false),
                        LastActiveDay = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        DistinctCommiterId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Participants");
            DropTable("dbo.DistinctCommiters");
        }
    }
}
