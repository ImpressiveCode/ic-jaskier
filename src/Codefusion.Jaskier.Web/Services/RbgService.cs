namespace Codefusion.Jaskier.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Helpers;
    using Codefusion.Jaskier.Common.Services.DataExport;

    using PredictionRequest = Codefusion.Jaskier.Common.Data.PredictionRequest;

    public enum BlockGenerationMode
    {
        RandomBlockGenerator,
        ImposedParticipant
    }

    public interface IRbgService
    {
        char? GetNextBlockChar(PredictionRequest predictionRequest, BlockGenerationMode mode);
        bool GetRandomPredictionEnabledFlag(int max = 1);
    }

    public class RbgService : IRbgService
    {
        private readonly IServiceConfiguration serviceConfiguration;
        private RandomBlockGenerator fileBlockGenerator;

        public RbgService(IServiceConfiguration serviceConfiguration)
        {
            this.serviceConfiguration = serviceConfiguration;
            this.fileBlockGenerator = RandomBlockGenerator.Create(new[] { 'A', 'B' });
        }

        public bool GetRandomPredictionEnabledFlag(int max = 1)
        {
            var blockChar = this.fileBlockGenerator.Next(max);

            if (blockChar == 'A')
            {
                return true;
            }

            return false;
        }

        public char? GetNextBlockChar(PredictionRequest predictionRequest, BlockGenerationMode mode)
        {
            string developer = GetDeveloperName(predictionRequest);

            // MaxOccurrencesOfCharacterInBlock = 1 - random block generator.
            // MaxOccurrencesOfCharacterInBlock > 1 - alternating tratments design.
            const int MaxOccurrencesOfCharacterInBlock = 2;

            using (var databaseContext = this.CreateContext())
            {
                var lastRbgData = GetLastRbgDatas(databaseContext, developer, MaxOccurrencesOfCharacterInBlock);

                int experimentDay = CalculateCurrentExperimentDay(lastRbgData);

                // Do nothing if we have RBG data for current experiment day.
                if (lastRbgData.Any() && experimentDay == lastRbgData[0].ExperimentDay)
                {
                    return lastRbgData[0].Mode[0];
                }

                // Calculate for new day.
                char? nextCharacterInBlock = null;

                if (mode == BlockGenerationMode.RandomBlockGenerator)
                {
                    // [0] 2018-01-05 JOHN A
                    // [1] 2018-01-04 JOHN B
                    // [2] 2018-01-03 JOHN A
                    // [3] 2018-01-02 JOHN B
                    var block = lastRbgData
                        .Select(g => g.Mode[0])
                        .Reverse()
                        .ToArray();

                    var blockGenerator = RandomBlockGenerator.Create(new[] { 'A', 'B' });
                    blockGenerator.SetBlock(block);

                    // block = { 'B', 'A', 'B', 'A' }
                    nextCharacterInBlock = blockGenerator.Next(MaxOccurrencesOfCharacterInBlock);
                }
                else if (mode == BlockGenerationMode.ImposedParticipant)
                {
                    Participant participant = GetParticipant(developer, databaseContext);

                    if (participant != null)
                    {
                        var generator = new ImposedBlockGenerator();
                        generator.SetBlock(participant.Block.ToList());
                        generator.SetPosition(experimentDay);
                        nextCharacterInBlock = generator.GetForIndex(generator.Position);
                    }
                }   
                
                AppendRbgData(databaseContext, developer, experimentDay, nextCharacterInBlock);

                return nextCharacterInBlock;
            }
        }

        private static void AppendRbgData(DatabaseContext databaseContext, string developer, int experimentDay, char? nextCharacterInBlock)
        {
            var rbgd = databaseContext.RbgDatas.Create();
            rbgd.Developer = developer;
            rbgd.Date = DateTime.UtcNow;
            rbgd.Mode = nextCharacterInBlock?.ToString() ?? string.Empty;
            rbgd.ExperimentDay = experimentDay;
            databaseContext.RbgDatas.Add(rbgd);
            databaseContext.SaveChanges();
        }

        private static Participant GetParticipant(string developer, DatabaseContext databaseContext)
        {
            string distinctDeveloperName = CommiterNameHelper.Normalize(developer);

            // Try find existing participant record for developer.
            var participant = databaseContext.Participants.FirstOrDefault(g => g.DistinctDeveloper == distinctDeveloperName);
            if (participant != null)
            {
                return participant;
            }

            // Developer has no participant record assigned. Take free record and assign developer.
            participant = databaseContext.Participants
                .Where(g => g.DistinctDeveloper == null)
                .OrderBy(g => g.Id)
                .FirstOrDefault();

            if (participant != null)
            {
                participant.DistinctDeveloper = distinctDeveloperName;
                databaseContext.SaveChanges();
            }

            return participant;
        }

        private static int CalculateCurrentExperimentDay(List<RbgData> lastRbgData)
        {
            int experimentDay = 0;
            if (lastRbgData.Any())
            {
                var last = lastRbgData.First();

                var calculator = new ExperimentDayCalculator();
                calculator.Days = last.ExperimentDay;
                calculator.PreviousDate = last.Date.Date;
                calculator.Calculate(DateTime.UtcNow.Date);

                experimentDay = calculator.Days;
            }

            return experimentDay;
        }

        private static List<RbgData> GetLastRbgDatas(DatabaseContext databaseContext, string developer, int count)
        {
            return databaseContext.RbgDatas
                .Where(g => g.Developer == developer)
                .OrderByDescending(g => g.Date)
                .Take(count)
                .ToList();
        }

        private static string GetDeveloperName(PredictionRequest predictionRequest)
        {
            string developer = "Unknown";

            foreach (PredictionRequestFile reqf in predictionRequest.Items)
            {
                developer = reqf.Author;
            }

            return developer;
        }

        private DatabaseContext CreateContext()
        {
            return new DatabaseContext(this.serviceConfiguration.ExportDatabaseConnectionString);
        }
    }
}