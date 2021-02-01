namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBlockToParticipantTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "Block", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Participants", "Block");
        }
    }
}
