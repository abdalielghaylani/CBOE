using System;
using System.Collections.Generic;
using System.Text;
using  CambridgeSoft.COE.Framework.Properties;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Base class for Business Logic Layer
    /// </summary>
    [Serializable]
    public class BLLBase: LoaderBase {
        [NonSerialized]
        public Notification notifications = new Notification();
        [NonSerialized]
        public string notification = string.Empty;
        [NonSerialized]
        private string serviceName = String.Empty;
        [NonSerialized]
        private string appName = String.Empty;
        [NonSerialized]
        public SecurityInfo securityInfo = null;

        /// <summary>
        /// SecurityInfo object
        /// </summary>
        public SecurityInfo SecurityInfo
        {
           get { return this.securityInfo; }
           set { this.securityInfo = value; }
        }

        /// <summary>
        /// Error and warning notifications
        /// </summary>
        public Notification Notifications
        {
            get { return notifications; }
            set { this.notifications = value; }
        }

        /// <summary>
        /// Service name 
        /// </summary>
        public string ServiceName
        {
            get { return this.serviceName; }
            set { this.serviceName = value; }
        }

        /// <summary>
        /// App name 
        /// </summary>
        public string AppName
        {
            get { return this.appName; }
            set { this.appName = value; }
        }

        protected  virtual void HandleError(System.Exception e)
        {
            //We need to add all of the erro stuff here.  We should log as well so we can pick up dataaccess issues.
            switch (e.Message)
            {

                case "InvalidCastException":
                    throw e;

                case "InvalidOperationException":
                    throw e;

                default:
                    throw e;
                    break;
            }
        }
    }
}
