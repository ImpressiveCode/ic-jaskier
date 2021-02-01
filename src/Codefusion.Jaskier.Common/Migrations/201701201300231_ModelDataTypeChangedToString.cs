namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ModelDataTypeChangedToString : DbMigration
    {
        public override void Up()
        {
            this.AlterColumn("dbo.PredictionModels", "Model", c => c.String());
        }
        
        public override void Down()
        {
            this.AlterColumn("dbo.PredictionModels", "Model", c => c.Binary());
        }
    }
}
