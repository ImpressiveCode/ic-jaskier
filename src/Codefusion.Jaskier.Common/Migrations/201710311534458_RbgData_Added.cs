namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RbgData_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RbgDatas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Developer = c.String(maxLength: 500),
                        Date = c.DateTime(nullable: false),
                        Mode = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RbgDatas");
        }
    }
}
