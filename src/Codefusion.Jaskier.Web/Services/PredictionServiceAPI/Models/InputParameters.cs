// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Codefusion.Jaskier.Web.Services.PredictionServiceAPI.Models
{
    public partial class InputParameters
    {
        /// <summary>
        /// Initializes a new instance of the InputParameters class.
        /// </summary>
        public InputParameters() { }

        /// <summary>
        /// Initializes a new instance of the InputParameters class.
        /// </summary>
        /// <param name="guid">character</param>
        public InputParameters(string guid = default(string))
        {
            this.Guid = guid;
        }

        /// <summary>
        /// Gets or sets character
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

    }
}
