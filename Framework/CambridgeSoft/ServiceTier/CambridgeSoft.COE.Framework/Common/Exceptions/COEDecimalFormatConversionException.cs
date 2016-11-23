using System;
using System.Collections.Generic;
using System.Text;



namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised when there is a problem converting a string to decimal
    /// Provides a custom friendly message which includes the expected format for a number
    /// The caller must pass in the string value that failed during conversion
    /// </summary>
    [Serializable]
    public class COEDecimalFormatConversionException : Exception
    {

        public COEDecimalFormatConversionException(string failedValue) : base(failedValue) { }
        public COEDecimalFormatConversionException(string failedValue, Exception innerException) : base(failedValue, innerException) { }
        public COEDecimalFormatConversionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }

        public override string Message
        {
            get
            {
                string customMessage = string.Format("Could not convert {0} to a decimal number.\r\n" + 
                                                     "Expected numerical format: {1:N}",
                                                     base.Message,
                                                    123456.78F);
                return customMessage;
            }
        }

        // Since this execption overrides the Message with a clear end-user message
        // we want this to be the base exception so that applications that display 
        // the baseexeption message will get this message.
        public override Exception GetBaseException()
        {          
            return this;
        }
    }
}
