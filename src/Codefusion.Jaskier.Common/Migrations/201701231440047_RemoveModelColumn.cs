namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemoveModelColumn : DbMigration
    {
        public override void Up()
        {
            this.DropColumn("dbo.PredictionModels", "Model");
        }
        
        public override void Down()
        {
            this.AddColumn("dbo.PredictionModels", "Model", c => c.String());
        }
    }
}
