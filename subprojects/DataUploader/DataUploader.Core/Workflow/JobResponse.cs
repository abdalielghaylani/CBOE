using System.Collections.Generic;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// Contains all information that represents the job execution result.
    /// </summary>
    public class JobResponse
    {
        private JobParameters _jobParameters = null;
        private IDictionary<string, object> _responseContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobParameters">Job parameters for this job service</param>
        public JobResponse(JobParameters jobParameters)
        {
            _responseContext = new Dictionary<string, object>();
            this._jobParameters = jobParameters;
        }

        /// <summary>
        /// Gets the job parameters.
        /// </summary>
        public JobParameters JobParameters
        {
            get
            {
                return _jobParameters;
            }
        }

        /// <summary>
        /// The response from the job service after executing the job.
        /// </summary>
        public IDictionary<string, object> ResponseContext
        {
            get
            {
                return _responseContext;
            }
        }
    }
}
