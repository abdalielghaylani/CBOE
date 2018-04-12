using System;
using System.Data;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Data.OutputObjects
{
    /// <summary>
    /// <see cref="OutputObject"/> for adding containers to inventory
    /// </summary>
    class OutputObjectInvContainer : OutputObject
    {
        private int _nAddSubstanceTo = 0;
        private int _nDuplicateAction = 0;
        public OutputObjectInvContainer()
        {
            OutputType = "Load substances into containers in inventory";   // "Load Structures into Containers"
            IsValid = Csla.ApplicationContext.User.Identity.IsAuthenticated;
            if (IsValid)
            {
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("OutputConfiguration");
                    oCOEXmlTextWriter.WriteAttributeString("text", "Configuration");
                    {
                        oCOEXmlTextWriter.WriteStartElement("GroupBox");
                        oCOEXmlTextWriter.WriteAttributeString("text", "Add substance to");
                        oCOEXmlTextWriter.WriteAttributeString("member", "_nAddSubstanceTo");
                        {
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Inventory");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("RadioButton");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Registration");
                            oCOEXmlTextWriter.WriteEndElement();
                            oCOEXmlTextWriter.WriteStartElement("GroupBox");
                            oCOEXmlTextWriter.WriteAttributeString("text", "Duplicate action");
                            oCOEXmlTextWriter.WriteAttributeString("member", "_nDuplicateAction");
                            {
                                oCOEXmlTextWriter.WriteStartElement("RadioButton");
                                oCOEXmlTextWriter.WriteAttributeString("text", "Add as a duplicate registered substance");
                                oCOEXmlTextWriter.WriteEndElement();
                                oCOEXmlTextWriter.WriteStartElement("RadioButton");
                                oCOEXmlTextWriter.WriteAttributeString("text", "Add as a new batch of the registered substance");
                                oCOEXmlTextWriter.WriteEndElement();
                                oCOEXmlTextWriter.WriteStartElement("RadioButton");
                                oCOEXmlTextWriter.WriteAttributeString("text", "Ignore substance");
                                oCOEXmlTextWriter.WriteEndElement();
                            }
                            oCOEXmlTextWriter.WriteEndElement();
                        }
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    oCOEXmlTextWriter.WriteEndElement();

                    UnboundConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
            } // if (Csla.ApplicationContext.User.Identity.IsAuthenticated)

            return;
        } // OutputObjectInvContainer()

        protected override bool StartWrite()
        {
            ClearMessages();
            AddMessage(LogMessage.LogSeverity.Critical, LogMessage.LogSource.Output, 0, "StartWrite() Unimplemented");
            switch (_nAddSubstanceTo)
            {
                default: break;
            };
            switch (_nDuplicateAction) {
                default: break;
            };
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

    } // class OutputObjectInvContainer
}
