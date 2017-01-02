using System;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Access
{
    /// <summary>
    /// Represents the database response properties for most CRUD operations on a
    /// Multi-Component Registry Record.
    /// </summary>
    public class RegistrationCrudResponse : Csla.ReadOnlyBase<RegistrationCrudResponse>
    {
        #region >Members<

        private string _error = string.Empty;
        private string _key = string.Empty;
        private int _id = 0;
        private string _result = string.Empty;
        //irony: the longest name has the shortest value
        private string _duplicateActionTakenIndicator = "N";
        private DuplicateAction _duplicateActionTaken = DuplicateAction.None;
        private string _briefMessage = string.Empty;

        //data
        private bool _actionDetermined = false;
        private bool _responseExtracted = false;
        private string _dalMessage = string.Empty;
        private string _customFieldsResponseXml = string.Empty; 

        #endregion

        #region > Csla.ReadOnlyBase members <

        /// <summary>
        /// The identifier associated with the RegistrationResponse.
        /// Added for Csla.ReadOnlyBase compatibility.
        /// </summary>
        /// <returns>If available, the RegNumber; otherwise, the Temporary Id.</returns>
        protected override object GetIdValue()
        {
            string identifier = string.Empty;
            if (!string.IsNullOrEmpty(_key))
                identifier = _key;
            else
                identifier = _id.ToString();
            return identifier;
        }

        #endregion

        /// <summary>
        /// The raw message data from the repository.
        /// </summary>
        public string RawDalMessage{ get { return _dalMessage; } }

        /// <summary>
        /// Any business error (non-exception) encountered during a data-access event.
        /// </summary>
        public string Error
        {
            get
            {
                if (!_responseExtracted)
                    this.ExtractResponse();
                return _error;
            }
        }

        /// <summary>
        /// The identifier given to finalized registration records.
        /// </summary>
        public string RegistrationKey { get { return _key; } }

        /// <summary>
        /// The identifier given to queued registrations.
        /// </summary>
        public int RegistrationId { get { return _id; } }

        /// <summary>
        /// If not empty, the XML string to initialize a RegistryRecord instance.
        /// </summary>
        public string Result
        {
            get
            {
                if (!_responseExtracted)
                    this.ExtractResponse();
                return _result;
            }
        }

        /// <summary>
        /// A brief summary of the outcome of the data-access event.
        /// </summary>
        public string Message
        {
            get
            {
                if (!_responseExtracted)
                    this.ExtractResponse();
                return _briefMessage;
            }
        }

        /// <summary>
        /// Any duplication-resolution that occured within the data-access event.
        /// </summary>
        public DuplicateAction DuplicateActionTaken
        {
            get
            {
                if (_actionDetermined == false)
                {
                    switch (_duplicateActionTakenIndicator)
                    {
                        case "B":
                            this._duplicateActionTaken = DuplicateAction.Batch;
                            break;
                        case "D":
                            this._duplicateActionTaken = DuplicateAction.Duplicate;
                            break;
                        case "T":
                            this._duplicateActionTaken = DuplicateAction.Temporary;
                            break;
                        case "C":
                            this._duplicateActionTaken = DuplicateAction.Compound;
                            break;
                        default:
                            this._duplicateActionTaken = DuplicateAction.None;
                            break;
                    }
                    _actionDetermined = true;
                }
                return _duplicateActionTaken;
            }
        }

        public string CustomFieldsResponse
        {
            get
            {
                CanReadProperty();
                return _customFieldsResponseXml;
            }
        }
        /// <summary>
        /// Extracts the Message, Error and Result properties from the raw DAL response.
        /// Called 'on-demand' by property accessors.
        /// </summary>
        [COEUserActionDescription("ExtractDalResponse")]
        private void ExtractResponse()
        {
            //Extract the rest from the output of the DAL procedure
            if (!string.IsNullOrEmpty(_dalMessage))
            {
                try
                {
                    StringReader responseReader = new StringReader(_dalMessage);
                    XmlTextReader xtReader = new XmlTextReader(responseReader);
                    xtReader.WhitespaceHandling = WhitespaceHandling.None;
                    while (xtReader.Read())
                    {
                        //xtReader.MoveToContent();
                        if (xtReader.HasAttributes)
                        {
                            xtReader.MoveToAttribute("message");
                            this._briefMessage = xtReader.Value;
                            //Console.WriteLine("{0}={1}", xtReader.Name, xtReader.Value);
                            xtReader.MoveToElement();
                        }
                        xtReader.ReadToDescendant("Error");
                        this._error = xtReader.ReadInnerXml();
                        //Console.WriteLine("{0}={1}", xtReader.Name, xtReader.ReadInnerXml());

                        xtReader.MoveToContent();
                        this._result = xtReader.ReadInnerXml();
                        //Console.WriteLine("{0}={1}", xtReader.Name, xtReader.ReadInnerXml());
                        xtReader.MoveToContent();
                        this._customFieldsResponseXml = xtReader.ReadOuterXml();
                        //Console.WriteLine("{0}={1}", xtReader.Name, xtReader.ReadInnerXml());
                        break;
                    }

                    xtReader.Close();
                    responseReader.Close();
                }
                catch
                {
                    //TODO: Determine course of action if message undecipherable.
                }
            }
            _responseExtracted = true;
        }

        /// <summary>
        /// To be instantiated from the DAL mechanism only, from a data-access event.
        /// </summary>
        /// <param name="dalMessage">The output variable containing the an object's representation as xml.</param>
        /// <param name="identifier"></param>
        /// <param name="key"></param>
        /// <param name="duplicateActionTaken"></param>
        public RegistrationCrudResponse(string dalMessage, int identifier)
        {
            this._id = identifier;
            this._key = string.Empty;
            this._dalMessage = dalMessage;
            ExtractResponse();
        }

        /// <summary>
        /// Typical constructor, used when the DAL has attempted to register a record but might have
        /// instead created a temporary one.
        /// </summary>
        /// <param name="dalMessage"></param>
        /// <param name="regNum"></param>
        /// <param name="duplicateActionTaken">
        /// a one-letter indicator of a RegistryRecord.DuplicateAction enum value
        /// </param>
        public RegistrationCrudResponse(string dalMessage, string regNum, string duplicateActionTaken)
        {
            this._duplicateActionTakenIndicator = duplicateActionTaken;
            if (DuplicateActionTaken == DuplicateAction.Temporary)
                this._id = Convert.ToInt32(regNum);
            else
                this._key = regNum;
            this._dalMessage = dalMessage;
            ExtractResponse();
        }

    }
}
