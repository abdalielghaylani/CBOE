using System;
using System.Data;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for adding substances to inventory
    /// </summary>
    class OutputObjectInvAdd :OutputObject
    {
        public OutputObjectInvAdd()
        {
            OutputType = "Load substances to inventory";                   // "Load Structures Only"
            IsValid = Csla.ApplicationContext.User.Identity.IsAuthenticated;
            return;
        } // OutputObjectInvAdd()

        protected override bool StartWrite()
        {
            ClearMessages();
            AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Output, 0, "StartWrite() Unimplemented");
            return HasMessages;
        } // StartWrite()

        protected override bool DataSetWrite(int vnRecord)
        {
            ClearMessages();
            AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Output, 0, "DataSetWrite Unimplemented");
            return HasMessages;
        } // DataSetWrite()

        public override bool EndWrite()
        {
            ClearMessages();
            return HasMessages;
        } // EndWrite()

    } // class OutputObjectInvAdd
}
