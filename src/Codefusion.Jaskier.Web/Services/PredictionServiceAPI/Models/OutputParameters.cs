// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Codefusion.Jaskier.Web.Services.PredictionServiceAPI.Models
{
    public partial class OutputParameters
    {
        /// <summary>
        /// Initializes a new instance of the OutputParameters class.
        /// </summary>
        public OutputParameters() { }

        /// <summary>
        /// Initializes a new instance of the OutputParameters class.
        /// </summary>
        /// <param name="result">numeric</param>
        public OutputParameters(double? result = default(double?))
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets or sets numeric
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "result")]
        public double? Result { get; set; }

    }
}
