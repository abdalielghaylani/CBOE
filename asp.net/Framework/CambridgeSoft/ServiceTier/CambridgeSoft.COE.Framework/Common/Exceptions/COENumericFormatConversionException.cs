using System;
using System.Collections.Generic;
using System.Text;



namespace CambridgeSoft.COE.Framework.Types.Exceptions
{
    /// <summary>
    /// Exception raised when there is a problem converting a string to Double
    /// Provides a custom friendly message which includes the field type and its field value.
    /// The caller must pass in the string value that failed during conversion
    /// </summary>
    [Serializable]
    public class COENumericFormatConversionException : Exception
    {

        public COENumericFormatConversionException(string failedValue) : base(failedValue) { }
        public COENumericFormatConversionException(string failedValue, Exception innerException) : base(failedValue, innerException) { }
        public COENumericFormatConversionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context) : base(serializationInfo, context) { }

        public override string Message
        {
            get
            {
                try
                {
                    string customMessage = string.Empty;
                    if (!(string.IsNullOrEmpty(base.Message)))
                        customMessage = base.Message;
                    if (customMessage.IndexOf(':') != -1)
                    {
                        String[] fieldData = customMessage.Split(':');
                        string fieldValue = fieldData[0];
                        string fieldType = fieldData[1];
                        customMessage = string.Format("Could not convert {0} to type double for Field {1}\n",
                                                        fieldValue, fieldType);
                    }
                    return customMessage;                    
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // Since this exception overrides the Message with a clear end-user message
        // we want this to be the base exception so that applications that display 
        // the baseexeption message will get this message.
        public override Exception GetBaseException()
        {
            return this;
        }
    }
}