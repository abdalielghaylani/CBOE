using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SpotfireIntegration.Common
{
    [DataContract]
    public class TableLoadFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        public TableLoadFault() { }
        
        public TableLoadFault(Exception cause)
        {
            this.Message = cause.Message;
            this.StackTrace = cause.StackTrace;
        }
    }
}
