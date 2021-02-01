namespace Codefusion.Jaskier.Web.Services.Configurations
{
    using Codefusion.Jaskier.Common.Data;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class NTPredictionServiceConfiguration : IPredictionServiceConfiguration
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
                                { "CCMMax", predictionRequestFile.CCMMax.ToString() },
                                { "NumberOfDistinctCommitters", predictionRequestFile.NumberOfDistinctCommitters.ToString() },
                                { "NumberOfModifiedLines", predictionRequestFile.NumberOfModifiedLines.ToString() },
                                { "TotalNumberOfRevisions", predictionRequestFile.TotalNumberOfRevisions.ToString() },
                                {
                                    // Do as if the change was made now - minutes from mindnight are used in the model
                                    "BuildCommitDateTimeLocal",
                                    (DateTime.Now.Hour*60+DateTime.Now.Minute).ToString()
                                },

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