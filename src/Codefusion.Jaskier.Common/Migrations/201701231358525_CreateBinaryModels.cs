namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateBinaryModels : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.BinaryModels",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        value = c.Binary(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            this.DropTable("dbo.BinaryModels");
        }
    }
}
