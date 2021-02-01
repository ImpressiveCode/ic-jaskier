namespace Codefusion.Jaskier.Common.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Codefusion.Jaskier.Common.Services.DataExport;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContext context)
        {
            SeedParticipants(context);
        }

        private static void SeedParticipants(DatabaseContext context)
        {
            AddOrUpdateParticipant(context, "Participant 1", "A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B ");
            AddOrUpdateParticipant(context, "Participant 2", "A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	A 	A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B ");
            AddOrUpdateParticipant(context, "Participant 3", "A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	A 	A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B ");
            AddOrUpdateParticipant(context, "Participant 4", "A 	A 	A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	A 	A 	A 	A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B ");
            AddOrUpdateParticipant(context, "Participant 5", "A 	A 	A 	A 	A 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	A 	A 	A 	A 	A 	A 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B 	B ");
        }

        private static void AddOrUpdateParticipant(DatabaseContext context, string displayName, string block)
        {
            var participant = context.Participants.FirstOrDefault(g => g.DisplayName == displayName);
            if (participant != null)
            {
                participant.Block = NormalizeBlock(block);
                context.Entry(participant).State = EntityState.Modified;
            }
            else
            {
                var newParticipant = context.Participants.Create();
                newParticipant.DisplayName = displayName;
                newParticipant.Block = NormalizeBlock(block);
                context.Participants.Add(newParticipant);
            }

            context.SaveChanges();
        }

        private static string NormalizeBlock(string input)
        {
            return input?.ToUpperInvariant().Replace((char)160, ' ').Replace(" ", "").Replace("\t", "") ?? string.Empty;
        }
    }
}
