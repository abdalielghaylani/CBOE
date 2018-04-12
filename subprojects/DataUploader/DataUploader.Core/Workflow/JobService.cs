using System;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// Defines the contracts that all individual job services should conform to.
    /// </summary>
    public abstract class JobService
    {
        private JobParameters _jobParameters = null;
        public event EventHandler<EventArgs> JobComplete;

        protected JobService() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sessionContext">The session context used to initialize this specific job service</param>
        public JobService(JobParameters jobParameters)
        {
            _jobParameters = jobParameters;
            //BuildJobParameters(jobParameters);
        }

        /// <summary>
        /// Gets the parameters for this job service.
        /// </summary>
        public JobParameters JobParameters
        {
            get
            {
                return _jobParameters;
            }
        }

        /// <summary>
        /// Whether or not this job supports cancel operations. By default, it's not supported.
        /// </summary>
        public virtual bool SupportCancel
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Whether or not this job needs to be notified when the source file was changed externally.
        /// By default, it doesn't monitor the source file change.
        /// </summary>
        public virtual bool MonitorSourceFileChange
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Performs the actual work, based on the job parameters.
        /// </summary>
        /// <returns></returns>
        public JobResponse DoJob()
        {
            if (AreJobParametersValid())
                return DoJobInternal();
            else
                throw new ArgumentException("Invalid job parameters");
        }

        /// <summary>
        /// Internal de facto job execution method.
        /// </summary>
        /// <returns></returns>
        protected abstract JobResponse DoJobInternal();

        /// <summary>
        /// Responds to source file change event. By default, it does nothing.
        /// </summary>
        protected virtual void OnSourceFileChanged() { }

        /// <summary>
        /// Validates whether or not the passed in job parameters are valid in order to perform
        /// this job.
        /// </summary>
        protected abstract bool AreJobParametersValid();

        protected void OnJobComplete(object sender, EventArgs e)
        {
            if(JobComplete!=null)
                JobComplete(sender, e);
        }
    }
}
