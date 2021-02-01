namespace Codefusion.Jaskier.Web.Services.Configurations
{
    using Codefusion.Jaskier.Common.Data;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class BASPredictionServiceConfiguration : IPredictionServiceConfiguration
    {
        public string Serialize(PredictionRequestFile predictionRequestFile)
        {
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>
                {
                    {
                        "input1",
                        new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                        { "NumberOfRevisions", predictionRequestFile.NumberOfRevisions.ToString() },
                                        { "NumberOfDistinctCommitters", predictionRequestFile.NumberOfDistinctCommitters.ToString() },
                                        { "NumberOfModifiedLines", predictionRequestFile.NumberOfModifiedLines.ToString() },
                                        { "BuildResult", "0" },
                                        { "TotalNumberOfRevisions", predictionRequestFile.TotalNumberOfRevisions.ToString() },
                                        { "BuildCommitDateTimeLocal", "" },
                                        { "PreviousBuildResult", predictionRequestFile.PreviousBuildResult.ToString() },
                                        { "CCMMax", predictionRequestFile.CCMMax.ToString() },
                                        { "CCMMd", predictionRequestFile.CCMMd.GetValueOrDefault().ToString(System.Globalization.CultureInfo.InvariantCulture) },
                                        { "CCMAvg", predictionRequestFile.CCMAvg.GetValueOrDefault().ToString(System.Globalization.CultureInfo.InvariantCulture) },

                            }
                        }
                    },
                },
                GlobalParameters = new Dictionary<string, string>
                {
                    { "Append score columns to output1", "true" },
                }
            };

            return JsonConvert.SerializeObject(scoreRequest);
        }
    }
}