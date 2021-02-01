namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Make sure to copy EntityFramework.SqlServer to the output directory.
        /// </summary>
        private static readonly Type Hack = typeof(System.Data.Entity.SqlServer.SqlFunctions);

        private const int ConnectionTimeoutInSeconds = 1 * 60 * 60;

        public DatabaseContext(string connectionString)
            : base(connectionString)
        {
            this.Database.CommandTimeout = ConnectionTimeoutInSeconds;
        }

        public DbSet<BinaryModel> BinaryModels { get; set; }

        public DbSet<Metric> Metrics { get; set; }

        public DbSet<PredictionModel> PredictionModels { get; set; }

        public DbSet<PredictionRequest> PredictionRequests { get; set; }

        public DbSet<RbgData> RbgDatas { get; set; }

        public DbSet<Telemetry> Telemetries { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<FilePrediction> FilePredictions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}
