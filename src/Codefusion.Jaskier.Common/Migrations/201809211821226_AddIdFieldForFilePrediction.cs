namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdFieldForFilePrediction : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.FilePredictions");
            AddColumn("dbo.FilePredictions", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.FilePredictions", "FileName", c => c.String(maxLength: 255));
            AddPrimaryKey("dbo.FilePredictions", "Id");
            CreateIndex("dbo.FilePredictions", "FileName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.FilePredictions", new[] { "FileName" });
            DropPrimaryKey("dbo.FilePredictions");
            AlterColumn("dbo.FilePredictions", "FileName", c => c.String(nullable: false, maxLength: 255));
            DropColumn("dbo.FilePredictions", "Id");
            AddPrimaryKey("dbo.FilePredictions", "FileName");
        }
    }
}
