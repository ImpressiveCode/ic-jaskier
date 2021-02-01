namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddIndexesOnMetrics : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"CREATE NONCLUSTERED INDEX [Index_Path_BuildCommitDateTimeLocal_ASC] ON [dbo].[Metrics]
(
	[Path] ASC,
	[BuildCommitDateTimeLocal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            this.DropIndex("dbo.Metrics", "Index_Path_BuildCommitDateTimeLocal_ASC");
        }
    }
}
