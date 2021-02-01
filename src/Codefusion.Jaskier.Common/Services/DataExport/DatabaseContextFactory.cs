namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Used by migrations.
    /// </summary>
    public class DatabaseContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create()
        {
            return new DatabaseContext(new AppConfiguration(null).ExportDatabaseConnectionString);
        }
    }
}
