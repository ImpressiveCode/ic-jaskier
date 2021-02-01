namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Refactoring2 : DbMigration
    {
        public override void Up()
        {
            this.DropPrimaryKey("dbo.BinaryModels");
            this.AlterColumn("dbo.BinaryModels", "id", c => c.Long(nullable: false, identity: true));
            this.AddPrimaryKey("dbo.BinaryModels", "id");
        }
        
        public override void Down()
        {
            this.DropPrimaryKey("dbo.BinaryModels");
            this.AlterColumn("dbo.BinaryModels", "id", c => c.String(nullable: false, maxLength: 128));
            this.AddPrimaryKey("dbo.BinaryModels", "id");
        }
    }
}
