namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCcmColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metrics", "CCMMax", c => c.Int());
            AddColumn("dbo.Metrics", "CCMMd", c => c.Double());
            AddColumn("dbo.Metrics", "CCMAvg", c => c.Double());
            AddColumn("dbo.Metrics", "ObjectId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metrics", "ObjectId");
            DropColumn("dbo.Metrics", "CCMAvg");
            DropColumn("dbo.Metrics", "CCMMd");
            DropColumn("dbo.Metrics", "CCMMax");
        }
    }
}
