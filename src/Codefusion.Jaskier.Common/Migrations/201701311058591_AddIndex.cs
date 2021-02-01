namespace Codefusion.Jaskier.Common.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddIndex : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"CREATE NONCLUSTERED INDEX [Index_ProjectName_ASC] ON [dbo].[Metrics]
(
	[ProjectName] ASC
)
INCLUDE ( 	[Id],
	[Path],
	[OldPath],
	[NumberOfRevisions],
	[NumberOfDistinctCommitters],
	[NumberOfModifiedLines],
	[BuildResult],
	[Commit],
	[BuildCommit],
	[ExportDateUtc],
	[TotalNumberOfRevisions],
	[BuildDateTimeLocal],
	[BuildCommitDateTimeLocal],
	[BuildProjectName],
	[Author],
	[PreviousBuildResult]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");    

        }

        public override void Down()
        {
            this.DropIndex("dbo.Metrics", "Index_ProjectName_ASC");

        }
    }
}
