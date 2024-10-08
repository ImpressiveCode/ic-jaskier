// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Codefusion.Jaskier.Web.Services.TrainingServiceAPI.Models
{
    public partial class BatchWebServiceResult
    {
        /// <summary>
        /// Initializes a new instance of the BatchWebServiceResult class.
        /// </summary>
        public BatchWebServiceResult() { }

        /// <summary>
        /// Initializes a new instance of the BatchWebServiceResult class.
        /// </summary>
        /// <param name="state">State of the execution. Can be of the
        /// following values:
        /// - Pending: The batch execution was submitted but is not yet
        /// scheduled.
        /// Ready: The batch execution was submitted and can be executed.
        /// InProgress: The batch execution is currently being processed.
        /// Complete: The batch execution has been completed. Possible values
        /// include: 'pending', 'inProgress', 'ready', 'complete'</param>
        /// <param name="completedItemCount">Number of completed items in this
        /// batch operation.</param>
        /// <param name="totalItemCount">Number of total items in this batch
        /// operation.</param>
        /// <param name="batchExecutionResults">The responses of the
        /// individual executions.</param>
        public BatchWebServiceResult(string state = default(string), int? completedItemCount = default(int?), int? totalItemCount = default(int?), System.Collections.Generic.IList<WebServiceResult> batchExecutionResults = default(System.Collections.Generic.IList<WebServiceResult>))
        {
            this.State = state;
            this.CompletedItemCount = completedItemCount;
            this.TotalItemCount = totalItemCount;
            this.BatchExecutionResults = batchExecutionResults;
        }

        /// <summary>
        /// Gets or sets state of the execution. Can be of the following
        /// values:
        /// - Pending: The batch execution was submitted but is not yet
        /// scheduled.
        /// Ready: The batch execution was submitted and can be executed.
        /// InProgress: The batch execution is currently being processed.
        /// Complete: The batch execution has been completed. Possible values
        /// include: 'pending', 'inProgress', 'ready', 'complete'
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets number of completed items in this batch operation.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "completedItemCount")]
        public int? CompletedItemCount { get; set; }

        /// <summary>
        /// Gets or sets number of total items in this batch operation.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "totalItemCount")]
        public int? TotalItemCount { get; set; }

        /// <summary>
        /// Gets or sets the responses of the individual executions.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "batchExecutionResults")]
        public System.Collections.Generic.IList<WebServiceResult> BatchExecutionResults { get; set; }

    }
}
