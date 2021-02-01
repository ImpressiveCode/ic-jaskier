namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFilePredictionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FilePredictions",
                c => new
                    {
                        FileName = c.String(nullable: false, maxLength: 255),
                        PredictionEnabled = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FileName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FilePredictions");
        }
    }
}
