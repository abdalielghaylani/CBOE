using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Xsl;
//CS assemblies referenced
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Exceptions;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Registration.WebServices.Contracts;
using CambridgeSoft.COE.Registration.WebServices.Entities;
using CambridgeSoft.COE.Registration.WebServices.Properties;
//legacy method references
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Text;

namespace CambridgeSoft.COE.Registration.WebServices
{
    /// <summary>
    /// Gateway for Informatics Registration Services. Intended to support XML web service needs.
    /// Should cover the entire functionality for Registration workflows.
    /// </summary>
    [WebService(Namespace = "http:\\www.cambridgesoft.com", Description = "")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class COERegistrationWebService : WebService, ICOERegistrationWebServices
    {
        #region > Constants <

        private const string SERVICENAME = "COERegistration";
        private const string TRANSFORMS_PATH = @"..\Transforms\";
        private const string AUTHENTICATION_ERR_MSG = "Unable to authenticate the request; please provide authentication credentials " +
            "via the SOAP header";
        private const string AUTHORIZATION_FAILURE_MSG = "Caller is not authorized to perform this action.";
        private const string CDX_MIME_TYPE = "chemical/cdx";
        private const string CREDENTIALS = "Credentials";
        private const string GET_SUPPORTED_MIME_TYPES = "Returns a list of the support MIME types when retrieving Registration Records";
        private const string MDL_MIME_MOL = "chemical/mdl-molfile";
        private const string XMLNS = "www.cambridgesoft.com";
        private const string DATEFORMAT = "yyyy-MM-dd hh:mm:ss";

        private const string XPATH_REG_NUM = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
        private const string XPATH_REG_ID = "/MultiCompoundRegistryRecord/ID";

        private const string MESSAGE_NODENAME = "Message";
        private const string DETAILS_NODENAME = "Details";

        #endregion

        /// <summary>
        /// Used internally for authentication purposes.
        /// </summary>
        public COECredentials Credentials = new COECredentials();

        /// <summary>
        /// This constructor for this service class sets the context's application name. This enables
        /// and configures context-sensitive settings (such as log file names).
        /// <para>
        /// NOTE: Please be aware that, depending on the permissions granted to the authenticated user,
        /// invocation of some of these web methods may result in a security error being returned instead
        /// of the intended action being completed.
        /// </para>
        /// </summary>
        public COERegistrationWebService()
        {
            Csla.ApplicationContext.GlobalContext["AppName"] = ConfigurationManager.AppSettings["AppName"];
        }

        #region > Utility <

        /// <summary>
        /// Retrieves the XML string representation of a single registration record. The Component List
        /// is then transformed to the caller's specifications.
        /// </summary>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        /// <returns>The custom-formatted XML representation of a registration object's Component List.</returns>
        [WebMethod(false, Description = "Fetches Components from a Permanent Registration.")]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "SEARCH_REG" })]
        public XmlNode GetXmlPermRegComponentList(string regNum)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                string buf = GetPermReg(regNum).Xml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                // create an XsltArgumentList to pass to the transformer method
                XsltArgumentList xslArgs = new XsltArgumentList();
                xslArgs.AddParam("TransformedOn", string.Empty, DateTime.Now.ToString(DATEFORMAT));

                doc = this.TransformResult("RegistrationComponentList.xslt", doc, xslArgs);
                XmlNode additionalData = doc.DocumentElement;
                retVal = BuildInformationNode(
                    string.Format("Component list retrieved for registration '{0}'", regNum)
                    , MessageLevel.Information
                    , additionalData
                );
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves an xml representation of a display/value list which can be used to validate a value
        /// or generate members of a drop-down list, etc.
        /// </summary>
        /// <param name="criteria">Either the name or the numeric identifier for the picklist requested.</param>
        /// <returns>
        /// An xml representation of a list of key/value pairs which can be used for validating input.
        /// <para>
        /// Given the picklist domain ID <b>2</b>, or the picklist domain name <b>Units</b>,
        /// the result might look like this:
        /// <code>
        /// <![CDATA[
        /// <Picklist id="2" name="Units" count="4">
        ///   <PicklistItem ID="4">g</PicklistItem>
        ///   <PicklistItem ID="6">l</PicklistItem>
        ///   <PicklistItem ID="3">mg</PicklistItem>
        ///   <PicklistItem ID="5">ml</PicklistItem>
        /// </Picklist>
        /// ]]>
        /// </code>
        /// </para>
        /// </returns>
        [WebMethod(false, Description = "Fetches a PickList beased on its 'ID' or 'Name'.")]
        [SoapHeader(CREDENTIALS)]
        public XmlNode GetXmlPicklist(string criteria)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode retVal;
            string buf = string.Empty;
            int domainValue = 0;

            try
            {
                criteria = criteria.Trim();

                PickListNameValueList plSvc = PickListNameValueList.GetAllPickListDomains();
                if (plSvc.ContainsKey(criteria))
                    domainValue = Convert.ToInt32(criteria);
                else
                {
                    //could use 'plSvc.ContainsValue(criteria)' except for the case-sensitivity
                    foreach (Csla.NameValueListBase<string, string>.NameValuePair pair in plSvc)
                    {
                        if (pair.Value.ToUpper() == criteria.ToUpper())
                        {
                            domainValue = Convert.ToInt32(pair.Key);
                            break;
                        }
                    }
                }

                if (domainValue != 0)
                {
                    PicklistDomain plDomain = PicklistDomain.GetPicklistDomain(domainValue);
                    Picklist pl = Picklist.GetPicklist(plDomain);

                    //Create the xml representation of the picklist
                    using (System.IO.StringWriter sw = new System.IO.StringWriter())
                    {
                        XmlTextWriter xtw = new XmlTextWriter(sw);
                        {//root node
                            xtw.WriteStartElement("Picklist");
                            xtw.WriteAttributeString("id", plDomain.Identifier.ToString());
                            xtw.WriteAttributeString("name", plDomain.Description);
                            xtw.WriteAttributeString("count", pl.PickList.Count.ToString());
                            foreach (KeyValuePair<int, string> kvp in pl.PickList)
                            {
                                xtw.WriteStartElement("PicklistItem");
                                xtw.WriteAttributeString("ID", kvp.Key.ToString());
                                xtw.WriteValue(kvp.Value);
                                xtw.WriteEndElement();
                            }
                            xtw.WriteEndElement();

                            xtw.Flush();
                            buf = sw.ToString();
                            xtw.Close();
                        }
                    }
                    doc.LoadXml(buf);

                    retVal = BuildInformationNode(
                        "picklist retrieved", MessageLevel.Information, doc.DocumentElement);
                }
                else
                {
                    string errMsg = "Unable to find PickList with key (or value) of '{0}'";
                    throw new KeyNotFoundException(string.Format(errMsg, criteria));
                }

            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }
            return retVal;
        }

        /// <summary>
        /// Retrieves a structure object as the hydrating XML of the independent entity.
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [WebMethod(false, Description = "Fetches a Structure beased on its 'ID'.")]
        [SoapHeader(CREDENTIALS)]
        public XmlNode GetXmlStructure(int structureId)
        {
            XmlNode retVal;
            try
            {
                CheckCallerAuthentication();
                Structure structure = Structure.GetStructure(structureId);

                string buf = structure.Xml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                retVal = BuildInformationNode(
                    "structure retrieved", MessageLevel.Information, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }
            return retVal;
        }

        /// <summary>
        /// Retrieves the valid MIME types/formats which RegistrationRecord structures can be returned as.
        /// </summary>
        /// <returns>An xml listing of the MIME types supported by the ChemDraw control</returns>
        [WebMethod(false, Description = GET_SUPPORTED_MIME_TYPES)]
        [SoapHeader(CREDENTIALS)]
        public XmlNode GetXmlSupportedMimeTypes()
        {
            XmlNode retVal;

            try
            {
                Dictionary<string, string> mimeDictionary = GetChemDrawFormats();

                XmlDocument doc = new XmlDocument();
                XmlElement mimes = doc.CreateElement("SupportedMimes", XMLNS);

                //'count' attribute
                XmlAttribute count = doc.CreateAttribute("count");
                count.Value = mimeDictionary.Count.ToString();
                mimes.SetAttributeNode(count);

                //'note' attribute
                XmlAttribute note = doc.CreateAttribute("note");
                note.Value = "The prefix 'chemical/' is optional";
                mimes.SetAttributeNode(note);

                //'copyright' attribute
                XmlAttribute copyright = doc.CreateAttribute("copyright");
                copyright.Value = "Copyright © CambridgeSoft, Inc.  2010";
                mimes.SetAttributeNode(copyright);

                foreach (string key in mimeDictionary.Keys)
                {
                    //build the 'Mime' node
                    XmlElement mime = doc.CreateElement("Mime", XMLNS);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = key;
                    mime.SetAttributeNode(type);
                    mime.InnerText = mimeDictionary[key];
                    //append it to the 'SupportedMimes' node
                    mimes.AppendChild(mime);
                }

                retVal = BuildInformationNode("supported MIME types retrieved", MessageLevel.Information , mimes);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            //return the node
            return retVal;
        }

        /// <summary>
        /// Using the duplicate-detection algorithm provided by the <paramref name="duplicateCheck"/> parameter,
        /// checks if mixtures or compounds exist in the database that match the submitted mixture
        /// (the <paramref name="xml"/> parameter) or any of its components. The method will return null or
        /// an empty string if no matches are found. If matches are found, they are returned as an abbreviated
        /// list of system identifiers for those registrations or compounds.
        /// </summary>
        /// <example>
        /// The matching algorithms can be tested by submitting a structure that is already in the database.
        /// <list type="number">
        /// <item>
        /// Retrieve a template XML using the <see cref="RetrieveNewRegistryRecord"/> method.
        /// Copy it to an empty text file.
        /// </item>
        /// <item>
        /// Retrieve an existing mixture's XML representation using the <see cref="RetrieveRegistryRecord"/> method.
        /// </item>
        /// <item>
        /// From this existing record, copy the first component's &lt;Structure&gt; node text into the
        /// corresponding &lt;Structure&gt; node in the text file.
        /// </item>
        /// <item>
        /// Use the entire 'new' XML string from the text file as the <paramref name="xml"/> parameter value.
        /// </item>
        /// <item>
        /// Use 'C' for the <paramref name="duplicateCheck"/> parameter value.
        /// </item>
        /// </list>
        /// Provided that entity (in this example, the sole structure associated with registration AB-000526)
        /// is unique in the database, the return value should look like this:
        /// <code>
        /// <![CDATA[
        /// <Response message="1 duplicated component.">
        ///   <Error>
        ///     <COMPOUNDLIST>
        ///       <COMPOUND><TEMPCOMPOUNDID>-1</TEMPCOMPOUNDID>
        ///         <REGISTRYLIST>
        ///           <REGNUMBER count="1" CompoundID="526" SAMEFRAGMENT="True" SAMEEQUIVALENT="True">AB-000526</REGNUMBER>
        ///         </REGISTRYLIST>
        ///       </COMPOUND>
        ///     </COMPOUNDLIST>
        ///   </Error>
        ///   <Result></Result>
        /// </Response>
        /// ]]>
        /// </code>
        /// The matching algorithm ignores everything except the component structure data of the
        /// <paramref name="duplicateCheck"/> parameter while performing the evaluation. 
        /// </example>
        /// <param name="registryRecordXml">The custom-formatted XML representation of a registration object.</param>
        /// <param name="duplicateCheck">The single character provided represents the duplicate-checking mechanism the service will use.</param>
        /// <returns>Either an empty string (no matches found) or an XML representation of the matches</returns>
        [WebMethod(false, Description = "Retrieves duplciation information about existing registrations.")]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "SEARCH_REG" })]
        public XmlNode GetXmlUniqueRegistrationTest(string registryXml, string duplicateCheck)
        {
            XmlNode retVal;

            try
            {
                duplicateCheck = duplicateCheck.Trim();

                //Business (rules) before pelasure
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                DuplicateCheck dupCheckType = VerifyDuplicateCheck(duplicateCheck);

                RegistryRecord regRecord = GetNewReg();
                regRecord.InitializeFromXml(registryXml, true, true);

                string buf = regRecord.CheckUniqueRegistryRecord(dupCheckType);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                retVal = BuildInformationNode(
                    "operation completed", MessageLevel.Information, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }
            return retVal;
        }

        #endregion

        [WebMethod(false, Description = CREATE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "REGISTER_DIRECT" })]
        public XmlNode CreateRegistration(string registryXml, string resolutionAction)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                string buf = registryXml;
                string msg = string.Empty;
                MessageLevel lvl = MessageLevel.Information;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                RegistryRecord rec = RegistryRecord.NewRegistryRecord();
                rec.InitializeFromXml(doc.OuterXml, true, false);

                DuplicateAction action = this.VerifyDuplicateAction(resolutionAction);
                RegistryRecord newRegistryRecord = rec.Register(action);

                string xml = string.Empty;
                if (!string.IsNullOrEmpty(newRegistryRecord.FoundDuplicates))
                {
                    xml = newRegistryRecord.FoundDuplicates;
                    msg = newRegistryRecord.DalResponseMessage;
                    lvl = MessageLevel.Warning;
                }
                else
                {
                    xml = newRegistryRecord.Xml;
                    msg = string.Format("Created registration '{0}'", newRegistryRecord.RegNum);
                }
                
                doc.LoadXml(xml);
                retVal = BuildInformationNode(msg, lvl, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        [WebMethod(false, Description = CREATE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "ADD_COMPOUND_TEMP" })]
        public XmlNode CreateTemporaryRegistration(string registryXml)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                string buf = registryXml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                RegistryRecord rec = RegistryRecord.NewRegistryRecord();
                rec.InitializeFromXml(doc.OuterXml, true, false);

                RegistryRecord newRegistryRecord = rec.Save();
                doc.LoadXml(newRegistryRecord.Xml);

                retVal = BuildInformationNode(
                    string.Format("Temporary registration '{0}' created", rec.ID)
                    , MessageLevel.Information, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        [WebMethod(false, Description = SAVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public XmlNode CreateRegistrationTemplate(string registryXml)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();

                string buf = registryXml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                RegistryRecord rec = RegistryRecord.NewRegistryRecord();
                rec.InitializeFromXml(doc.OuterXml, true, false);

                if (rec.IsSavable)
                {
                    string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                    COEGenericObjectStorageBO genericStorageObject = COEGenericObjectStorageBO.New(_databaseName);

                    string registryRecordXml = rec.XmlWithAddIns;
                    genericStorageObject.COEGenericObject = registryRecordXml;
                    genericStorageObject.UserName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ? "unknown" : COEUser.Get();
                    genericStorageObject.Save();

                    doc.LoadXml(genericStorageObject.COEGenericObject);
                    retVal = BuildInformationNode(
                        string.Format("Registration template '{0}' created", genericStorageObject.ID.ToString())
                        , MessageLevel.Information, doc.DocumentElement);
                }
                else
                {
                    retVal = BuildErrorNode("Registration template was invalid", doc.DocumentElement);
                }
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves the XML string representation of a single registration record, with all
        /// structural representations presented in the MIME format provided.
        /// </summary>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        /// <param name="structureMimeType">A CambridgeSoft-supported MIME type.</param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = "Fetches a Permanent Registration with MIME-formatted structures.")]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "SEARCH_REG" })]
        public XmlNode ReadRegistration(string regNum, string structureMimeType)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode retVal;
            string buf = string.Empty;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                XmlNode detail = GetPermRegistrationUsingMime(regNum, structureMimeType);
                retVal = BuildInformationNode(
                    string.Format("Registration '{0}' retrieved", regNum)
                    , MessageLevel.Information, detail);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }
            return retVal;
        }

        /// <summary>
        /// Retrieves the XML string representation of a single temporary registration record, with
        /// structural representations presented in the MIME format provided.
        /// </summary>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        /// <param name="structureMimeType">A CambridgeSoft-supported MIME type.</param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = "Fetches a Temporary Registration with MIME-formatted structures.")]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "SEARCH_TEMP" })]
        public XmlNode ReadTemporaryRegistration(int id, string structureMimeType)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode retVal;
            string buf = string.Empty;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                XmlNode detail = GetTempRegistrationUsingMime(id, structureMimeType);
                retVal = BuildInformationNode(
                    string.Format("Temporary registration '{0}' retrieved", id)
                    , MessageLevel.Information, detail);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }
            return retVal;
        }
        
        [WebMethod(false, Description = UPDATE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "EDIT_COMPOUND_REG" })]
        public XmlNode UpdateRegistration(string registryXml)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                string buf = registryXml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                string regNum = string.Empty;
                XmlNode regNode = doc.SelectSingleNode(XPATH_REG_NUM);

                if (regNode == null)
                    throw new Exception("Unable to derive the Registration Number required for the update.");
                regNum = regNode.InnerText;
                RegistryRecord rec = RegistryRecord.GetRegistryRecord(regNum);

                if (rec == null)
                    throw new Framework.Common.Exceptions.BusinessObjectNotFoundException(regNum, typeof(RegistryRecord));

                rec.InitializeFromXml(doc.OuterXml, true, false);
                RegistryRecord savedRegistryRecord = rec.SaveFromCurrentXml();

                doc.LoadXml(savedRegistryRecord.Xml);

                retVal = BuildInformationNode(
                    string.Format("Registration '{0}' updated", regNum)
                    , MessageLevel.Information, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        [WebMethod(false, Description = UPDATE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "EDIT_COMPOUND_TEMP" })]
        public XmlNode UpdateTemporaryRegistration(string registryXml)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());

                string buf = registryXml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(buf);

                string regNum = string.Empty;
                XmlNode regNode = doc.SelectSingleNode(XPATH_REG_ID);

                if (regNode == null)
                    throw new Exception("Unable to derive the Temporary Registration ID required for the update.");
                int regId = 0;
                if (!Int32.TryParse(regNode.InnerText, out regId))
                    throw new Exception("The Temporary Reg ID provided was not numeric.");
                if (regId == 0)
                    throw new Exception("Please provide a Temporary Registration ID from the repository.");

                RegistryRecord rec = RegistryRecord.GetRegistryRecord(regId);
                if (rec == null)
                    throw new Framework.Common.Exceptions.BusinessObjectNotFoundException(regId.ToString(), typeof(RegistryRecord));

                rec.InitializeFromXml(doc.OuterXml, false, false);
                RegistryRecord savedRegistryRecord = rec.SaveFromCurrentXml();

                doc.LoadXml(savedRegistryRecord.Xml);

                retVal = BuildInformationNode(
                    string.Format("Temporary registration '{0}' updated", regId)
                    , MessageLevel.Information, doc.DocumentElement);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        [WebMethod(false, Description = DELETE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        [COEAuthorization(new string[] { "DELETE_TEMP" })]
        public XmlNode DeleteTemporaryRegistration(int regId)
        {
            XmlNode retVal;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(MethodInfo.GetCurrentMethod());
                RegistryRecord.DeleteRegistryRecord(regId);

                string msg = string.Format("Temporary registration '{0}' deleted.", regId.ToString());
                retVal = BuildInformationNode(msg, MessageLevel.Information, null);
            }
            catch (Exception ex)
            {
                retVal = BuildErrorNode(ex);
            }

            return retVal;
        }

        #region > Private methods <

        /// <summary>
        /// Provides XSL-transformative capabilities for some XML documents that are returned by this
        /// service. Note that fragment transforms are not enabled; please provide an XmlDocument
        /// object to this method.
        /// </summary>
        /// <param name="xsltName">The name, including the extension, to the XSLT document.</param>
        /// <param name="xmlTarget">The XmlDocument object you wish to transform.</param>
        /// <param name="xslArgs">A list of XSLT arguments usable by the transformation process.</param>
        /// <returns></returns>
        XmlDocument TransformResult(string xsltName, XmlDocument xmlTarget, XsltArgumentList xslArgs)
        {
            //System.Xml.XPath.IXPathNavigable

            string xslFolder = AssemblyDirectory;
            string xslFile = TRANSFORMS_PATH + xsltName;
            string xslFilePath = Path.Combine(xslFolder, xslFile);
            string buf;

            // make an XslCompiledTransform
            XslCompiledTransform compiler = new XslCompiledTransform(true);
            compiler.Load(xslFilePath);

            // do the transformation
            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms))
            {
                compiler.Transform(xmlTarget, xslArgs, writer);
            }
            ms.Position = 0;

            //XmlTextReader xr = new XmlTextReader(ms);
            //buf = xr.ReadOuterXml();

            StreamReader sr = new StreamReader(ms);
            buf = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(buf);

            return doc;
        }

        /// <summary>
        /// Gets the full folder path of the executing assembly.
        /// </summary>
        /// <remarks>
        /// From : http://stackoverflow.com/questions/52797/c-how-do-i-get-the-path-of-the-assembly-the-code-is-in
        /// </remarks>
        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Determines if the Credentials object has been authenticated for the caller/proxy instance. As necessary,
        /// performs a COEPrincipal.Login() and fetches authentication information.
        /// </summary>
        private void CheckCallerAuthentication()
        {
            string msg = AUTHENTICATION_ERR_MSG;
            bool isAuthenticated = false;

            //try
            //{
            if (Csla.ApplicationContext.User.Identity.IsAuthenticated == true)
                isAuthenticated = true;
            else
            {
                //if this is the initial authentication, perform the full login process
                if (string.IsNullOrEmpty(Credentials.UserName) && string.IsNullOrEmpty(Credentials.AuthenticationTicket))
                {
                    //no credentials have been provided...try to apply 'testing' credentials from the config file
                    string[] users = ConfigurationManager.AppSettings.GetValues("TestingUserName");
                    string[] passwords = ConfigurationManager.AppSettings.GetValues("TestingPassword");

                    if (users != null && passwords != null)
                    {
                        Credentials.UserName = users[0];
                        Credentials.Password = passwords[0];
                    }
                }

                if (string.IsNullOrEmpty(Credentials.UserName))
                    isAuthenticated = COEPrincipal.Login(Credentials.AuthenticationTicket, true);
                else
                    isAuthenticated = COEPrincipal.Login(Credentials.UserName, Credentials.Password);
            }
            //}
            //catch
            //{
            //    throw;
            //TODO: REG TEAM - why isn't this possibility covered by exception-handling/logging?
            //isAuthenticated = false;
            //}

            if (isAuthenticated == false)
                throw new System.Security.Authentication.InvalidCredentialException(msg);

        }

        /// <summary>
        /// Extracts authorization requirements from the calloing method's attributes,
        /// and determines if the user context is authorized to execute that method.
        /// </summary>
        /// <param name="callingMethod">
        /// A System.Reflection.MethodBase representing a System.Reflection.MethodInfo object.
        /// </param>
        private void CheckCallerAuthorizations(MethodBase callingMethod)
        {
            MemberInfo callerInfo = callingMethod as MemberInfo;
            System.Attribute[] attribs =
                System.Attribute.GetCustomAttributes(callerInfo, typeof(COEAuthorizationAttribute));

            if (attribs != null)
            {
                foreach (System.Attribute attrib in attribs)
                {
                    COEAuthorizationAttribute authAttrib = (COEAuthorizationAttribute)attrib;
                    if (string.IsNullOrEmpty(authAttrib.WebAuthorizationZone))
                        CheckCallerAuthorizations(new List<string>(authAttrib.PrivilegeMatches));
                    else
                    {
                        CheckCallerAuthorizations(authAttrib.WebAuthorizationZone);
                    }
                }
            }
        }

        /// <summary>
        /// Privided a list of permissions, determines if the authenticated caller possesses at
        /// least one of them and throws an exception otherwise
        /// </summary>
        /// <remarks>
        /// This method canbe, but is not intended to be, called directly.
        /// </remarks>
        /// <param name="permissions">List of names of permissions</param>
        private void CheckCallerAuthorizations(List<string> permissions)
        {
            bool isAuthorized = false;
            foreach (string permission in permissions)
            {
                if (Csla.ApplicationContext.User.IsInRole(permission) == true)
                    isAuthorized = true; break;
            }
            if (!isAuthorized)
                throw new System.Security.AccessControl.PrivilegeNotHeldException(string.Join(", ", permissions.ToArray()));
        }

        /// <summary>
        /// TODO: Complete the code for this method, which fetches the GUI page-based privileges
        /// based on a defined GUI Page "ControlSetting".
        /// <para>
        /// For example, the ControlSetting node "EditCompoundTemp" wraps the privilege "EDIT_COMPOUND_TEMP".
        /// So this method would derive the privilege-set (in this case, there is only one privilege in the set)
        /// from the ControlSettings.
        /// </para>
        /// </summary>
        /// <param name="authZone"></param>
        private void CheckCallerAuthorizations(string authZone)
        {
            //TODO: Create the service class which will be used to execute a permissions fetch
            // from the database. Have the executor cache this data on fetch.

            /*
            List<string> permissions = serviceClass.GetPermissionsByAuthZone(authZone);
            CheckCallerAuthorizations(permissions);
            */
        }

        /// <summary>
        /// Translates incoming string-based definitions for RegistryRecord.DuplicateCheck
        /// enumeration members;
        /// </summary>
        /// <param name="duplicateCheck"></param>
        /// <returns>A matched enumeration member of Registryrecord.DuplicateCheck</returns>
        private DuplicateCheck VerifyDuplicateCheck(string duplicateCheck)
        {
            DuplicateCheck eDuplicateCheck = DuplicateCheck.None;
            string msg =
                "Invalid duplicate checking mechanism requested. Please use 'C'(ompound), 'M'(ixture), or 'N'(one)";
            switch (duplicateCheck.ToUpper())
            {
                case "COMPOUND":
                case "COMPOUNDCHECK":
                case "C": { eDuplicateCheck = DuplicateCheck.CompoundCheck; break; }
                case "MIXTURE":
                case "MIXCHECK":
                case "M": { eDuplicateCheck = DuplicateCheck.MixCheck; break; }
                case "NONE":
                case "N": { eDuplicateCheck = DuplicateCheck.None; break; }
                default: { throw new InvalidCastException(msg); }
            }
            return eDuplicateCheck;
        }

        /// <summary>
        /// Translates incoming string-based definitions for RegistryRecord.DuplicateAction
        /// enumeration members;
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <returns>A matched enumeration member of Registryrecord.DuplicateAction</returns>
        private DuplicateAction VerifyDuplicateAction(string duplicateAction)
        {
            DuplicateAction eDuplicateAction = DuplicateAction.None;
            string msg =
                "Invalid duplicate-resolution action requested. Please use 'B'(atch), 'D'(uplciate), 'T'(emporary), or 'N'(one)";
            switch (duplicateAction.ToUpper())
            {
                case "BATCH":
                case "B": { eDuplicateAction = DuplicateAction.Batch; break; }
                case "DUPLICATE":
                case "D": { eDuplicateAction = DuplicateAction.Duplicate; break; }
                case "NONE":
                case "N": { eDuplicateAction = DuplicateAction.None; break; }
                case "TEMPORARY":
                case "T": { eDuplicateAction = DuplicateAction.Temporary; break; }
                default: { throw new InvalidCastException(msg); }
            }
            return eDuplicateAction;
        }

        /// <summary>
        /// Abstraction layer for re-use
        /// </summary>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        /// <param name="structureMimeType">A CambridgeSoft-supported MIME type.</param>
        /// <returns></returns>
        private XmlNode GetPermRegistrationUsingMime(string regNum, string structureMimeType)
        {
            regNum = regNum.Trim();
            structureMimeType = structureMimeType.Trim();

            RegistryRecord regRecord = GetPermReg(regNum);
            if (!string.IsNullOrEmpty(structureMimeType))
                regRecord = ConvertRegistryRecordStructures(regRecord, structureMimeType);
            string buf = regRecord.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(buf);
            XmlNode regNode = doc.DocumentElement;

            return regNode;
        }

        /// <summary>
        /// Abstraction layer for re-use
        /// </summary>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        /// <param name="structureMimeType">A CambridgeSoft-supported MIME type.</param>
        /// <returns></returns>
        private XmlNode GetTempRegistrationUsingMime(int id, string structureMimeType)
        {
            structureMimeType = structureMimeType.Trim();

            RegistryRecord regRecord = GetTempReg(id);
            if (!string.IsNullOrEmpty(structureMimeType))
                regRecord = ConvertRegistryRecordStructures(regRecord, structureMimeType);
            string buf = regRecord.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(buf);
            XmlNode regNode = doc.DocumentElement;

            return regNode;
        }

        /// <summary>
        /// Internal method used by both RetrieveRegistryRecord and RetrieveRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        private RegistryRecord GetPermReg(string regNum)
        {
            string[] permissionsReq = new string[] { "SEARCH_REG" };
            CheckCallerAuthorizations(new List<string>(permissionsReq));

            RegistryRecord regRecord = RegistryRecord.GetRegistryRecord(regNum);
            return regRecord;
        }

        /// <summary>
        /// Internal method used by both RetrieveRegistryRecord and RetrieveRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        private RegistryRecord GetTempReg(int id)
        {
            string[] permissionsReq = new string[] { "SEARCH_TEMP" };
            CheckCallerAuthorizations(new List<string>(permissionsReq));

            RegistryRecord regRecord = RegistryRecord.GetRegistryRecord(id);
            return regRecord;
        }

        /// <summary>
        /// Fetches the prototype RegistryRecord object (as hydrated by the prototype xml
        /// returned from the database).
        /// </summary>
        /// <returns></returns>
        private RegistryRecord GetNewReg()
        {
            RegistryRecord regRecord = RegistryRecord.NewRegistryRecord();
            return regRecord;
        }

        /// <summary>
        /// Converts standard registration CDX-formatted structures into a different format as indicated by the
        /// <paramref name="structureMimeType"/> parameter.
        /// </summary>
        /// <remarks>
        /// At this time, only chemical/mdl-molfile (mol, mdl-molfile) values are accepted.
        /// </remarks>
        /// <param name="registration">The source RegistryRecord for which any structures will be converted.</param>
        /// <param name="structureMimeType"></param>
        /// <returns>A RegistryRecord object with updated Structure.Value and Structure.Type properties.</returns>
        private RegistryRecord ConvertRegistryRecordStructures(RegistryRecord registration, string structureMimeType)
        {
            //default the MIME type to be used to the input from the caller
            string usingMimeType = structureMimeType.ToLower();

            //provide a forgiveness-factor to callers that used a "short-hand" MIME string
            switch (usingMimeType)
            {
                case "mol":
                case "mdl-molfile":
                case MDL_MIME_MOL:
                    {
                        usingMimeType = MDL_MIME_MOL;
                        break;
                    }
                default:
                    break;
            }

            //unless properly specified, default to 'chemical' conversion instead of binary
            if (!usingMimeType.StartsWith("chemical/")
                && !usingMimeType.StartsWith("image/")
                && !usingMimeType.StartsWith("application/")
                )
                usingMimeType = "chemical/" + usingMimeType;

            string newValue = string.Empty;

            //First, the aggregate structure value
            newValue = ConvertStructure(registration.StructureAggregation, CDX_MIME_TYPE, usingMimeType);
            if (!string.IsNullOrEmpty(newValue))
            {
                registration.StructureAggregation = newValue;
            }

            //Then, each component's structure value
            foreach (Registration.Services.Types.Component component in registration.ComponentList)
            {
                Structure baseFragmentStructure = component.Compound.BaseFragment.Structure;
                if (!string.IsNullOrEmpty(baseFragmentStructure.Value))
                {
                    newValue = ConvertStructure(baseFragmentStructure.Value, CDX_MIME_TYPE, usingMimeType);
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        baseFragmentStructure.Value = newValue;
                        baseFragmentStructure.Format = usingMimeType;
                    }
                }

                Structure baseFragmentNormalized = component.Compound.BaseFragment.Structure;
                if (!string.IsNullOrEmpty(baseFragmentStructure.Value))
                {
                    //Convert the Structure."Value"
                    newValue = ConvertStructure(baseFragmentStructure.Value, CDX_MIME_TYPE, usingMimeType);
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        baseFragmentStructure.Value = newValue;
                        baseFragmentStructure.Format = usingMimeType;
                    }

                    //Convert the Structure."NormalizedStructure"
                    //TODO: Eventually, rename this property to "NormalizedValue"!
                    newValue = ConvertStructure(baseFragmentStructure.NormalizedStructure, CDX_MIME_TYPE, usingMimeType);
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        baseFragmentStructure.NormalizedStructure = newValue;
                    }
                }
            }

            return registration;
        }

        /// <summary>
        /// Converts one format of structure value to another using chemical MIME types as converter instructions.
        /// </summary>
        /// <param name="originalStructureValue"></param>
        /// <param name="originalMimeType"></param>
        /// <param name="structureMimeType"></param>
        /// <returns></returns>
        private string ConvertStructure(string originalStructureValue, string originalMimeType, string structureMimeType)
        {
            //By default, use the value passed by the caller
            string convertedStructure = originalStructureValue;
            if (!string.IsNullOrEmpty(originalStructureValue) && !originalMimeType.Equals(structureMimeType))
                convertedStructure =
                    COEChemDrawConverterUtils.ConvertStructure(originalStructureValue, originalMimeType, structureMimeType);
            return convertedStructure;
        }

        /// <summary>
        /// Retrieves a Dictionary of name/description pairs for those MIME types supported by the
        /// ChemDraw v.12 Control. This is provided as reference-only at this time, but can be used to
        /// direct other service calls which require MIME-type names for converstion processes.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetChemDrawFormats()
        {
            Dictionary<string, string> cdfs = new Dictionary<string, string>();
            //TODO: determine why this FAILS
            //cdfs.Add("text/xml", "ChemDraw's native cdxml format");

            cdfs.Add("chemical/x-cdx, chemical/cdx", "ChemDraw's native cdx format (Both listed MIME types are equivalent)");
            cdfs.Add("chemical/x-chemdraw", "ChemDraw's historic chm format. Use of this format is strongly discouraged.");
            cdfs.Add("chemical/x-mdl-molfile, chemical/mdl-molfile", "The Molfile format as developed by MDL ISIS/Draw (Both listed MIME types are equivalent)");
            cdfs.Add("chemical/x-mdl-tgf, chemical/mdl-tgf", "The tgf format as developed by MDL ISIS/Draw (Both listed MIME types are equivalent)");
            cdfs.Add("chemical/x-mdl-rxn, chemical/mdl-rxn", "The Rxnfile format as developed by MDL ISIS/Draw (Both listed MIME types are equivalent)");
            cdfs.Add(
                "chemical/x-daylight-smiles, chemical/daylight-smiles, chemical/x-smiles, chemical/smiles",
                "The SMILES format as developed by Daylight. This will read/write the SMARTS format instead, for files that contain reaction information. (The four listed MIME types are all equivalent)"
            );
            cdfs.Add("chemical/x-mdl-isis, chemical/mdl-isis", "The skc format as developed by MDL ISIS/Draw (Both listed MIME types are equivalent)");

            //TODO: request current support information for these formats:
            //cdfs.Add("chemical/x-questel-f1", "The F1 format as developed by Questel");
            //cdfs.Add("chemical/x-questel-f1-query", "The F1 query format as developed by Questel");
            cdfs.Add("chemical/x-msi-molfile, chemical/msi-molfile", "The MSI Molfile format as developed by Molecular Simulations,Inc. (now Accelrys) (Both listed MIME types are equivalent)");
            cdfs.Add("chemical/x-smd, chemical/smd", "The smd format (Both listed MIME types are equivalent)");
            cdfs.Add("chemical/x-ct, chemical/ct", "The Connection Table format used by very old versions of ChemDraw. (Both listed MIME types are equivalent)");

            //TODO: determine how to integrate the resulting full xml document 
            cdfs.Add(
                "chemical/x-cml, chemical/cml"
                , "The XML-based Chemical Markup Language as documented at http://www.xml-cml.org/. (Both listed MIME types are equivalent). This is only supported in ChemDraw 9.0 and later."
            );

            cdfs.Add("chemical/x-inchi, chemical/inchi", "The IUPAC International Chemical Identifier");

            //TODO: determine what these MIMEs *actually* return, becuase it's NOT a base64 image.
            //cdfs.Add("image/tiff", "A base-64 encoding of the TIFF-formatted image");
            //cdfs.Add("image/gif", "A base-64 encoding of the GIF-formatted image");
            //cdfs.Add("image/bmp", "A base-64 encoding of the BMP-formatted image");
            return cdfs;
        }

        /// <summary>
        /// Provides service errors in a format (XML) suitable to all consumers.
        /// <para>
        /// Callers should be able to specify, via their service proxy, how they prefer
        /// to have errors returned to them - as unhandled SOAP exceptions which they must
        /// handle, or as XmlNode elements containing some level of exception details.
        /// </para>
        /// <para>
        /// TODO: Determine the benefits of this.
        /// </para>
        /// </summary>
        /// <param name="error">The System.Exception object from which error details are extracted.</param>
        /// <returns></returns>
        private XmlNode BuildErrorNode(Exception error)
        {
            XmlDocument doc = new XmlDocument();
            Type exType = error.GetType();

            string exName = exType.Name;
            string exFullName = exType.FullName;
            string errMessage = error.Message;
            string errFullDetails = error.GetBaseException().Message;

            switch (exType.Name)
            {
                case "SoapException":
                    {
                        SoapException soapEx = error as SoapException;
                        Exception ex = soapEx.GetBaseException();
                        string[] errorParts = Regex.Split(ex.Message, "\r\n");
                        errMessage = errorParts[0];

                        if (errorParts.Length > 2)
                            errFullDetails = string.Join("\r\n", errorParts, 1, 2);
                        else
                            errFullDetails = soapEx.Detail.InnerXml;
                        break;
                    }
                case "DataPortalException":
                    {
                        Csla.DataPortalException dpEx = error as Csla.DataPortalException;
                        Exception busEx = dpEx.BusinessException;
                        if (busEx != null && busEx is ICOEException)
                            errMessage = (dpEx.BusinessException as ICOEException).ToShortErrorString();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            //create the error element itself
            XmlElement err = doc.CreateElement("Error", XMLNS);

            //apply the 'error type' attribute
            XmlAttribute attrib = doc.CreateAttribute("errorType");
            attrib.Value = exFullName;
            err.SetAttributeNode(attrib);

            //apply the message as a CDATA section
            XmlElement errorDetails = doc.CreateElement(MESSAGE_NODENAME, XMLNS);
            //XmlCDataSection error = doc.CreateCDataSection(errMessage);
            //errorDetails.AppendChild(error);
            errorDetails.InnerText = errMessage;
            err.AppendChild(errorDetails);

            //apply the additionalData element (as CDATA)
            XmlElement extraData = doc.CreateElement(DETAILS_NODENAME, XMLNS);
            XmlCDataSection data = doc.CreateCDataSection(errFullDetails);
            extraData.AppendChild(data);
            err.AppendChild(extraData);

            //return the node
            XmlNode returnValue = err;

            try
            {
                _coeLog.Log(error.ToString(), 0, System.Diagnostics.SourceLevels.Error);
            }
            catch { }

            return returnValue;
        }

        /// <summary>
        /// Provides service errors in a format (XML) suitable to all consumers.
        /// </summary>
        /// <param name="errorText"></param>
        /// <param name="additionalData"></param>
        /// <returns></returns>
        private XmlNode BuildErrorNode(string errorText, XmlNode additionalData)
        {
            XmlDocument doc = new XmlDocument();

            //create the error element itself
            XmlElement err = doc.CreateElement("Error", XMLNS);

            //apply the 'error type' attribute
            XmlAttribute attrib = doc.CreateAttribute("errorType");
            attrib.Value = MessageLevel.Error.ToString();
            err.SetAttributeNode(attrib);

            //apply the message as a CDATA section
            XmlElement errorDetails = doc.CreateElement(MESSAGE_NODENAME, XMLNS);
            XmlCDataSection error = doc.CreateCDataSection(errorText);
            errorDetails.AppendChild(error);
            err.AppendChild(errorDetails);

            //apply the additionalData element
            XmlElement extraData = doc.CreateElement(DETAILS_NODENAME, XMLNS);
            extraData.InnerXml = additionalData.OuterXml;
            err.AppendChild(extraData);

            //return the node
            XmlNode returnValue = err;
            return returnValue;
        }

        /// <summary>
        /// Provides service informational messages in a format (XML) suitable to all consumers.
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="level"></param>
        /// <param name="additionalData"></param>
        /// <returns></returns>
        private XmlNode BuildInformationNode(string messageText, MessageLevel level, XmlNode additionalData)
        {
            XmlDocument doc = new XmlDocument();

            //create the error element itself
            XmlElement info = doc.CreateElement("Information", XMLNS);

            //apply the 'error type' attribute
            XmlAttribute attrib = doc.CreateAttribute("infoType");
            attrib.Value = level.ToString();
            info.SetAttributeNode(attrib);

            //apply the message as a CDATA section
            XmlElement details = doc.CreateElement(MESSAGE_NODENAME, XMLNS);
            XmlCDataSection message = doc.CreateCDataSection(messageText);
            details.AppendChild(message);
            info.AppendChild(details);

            //apply the additionalData element
            if (additionalData != null)
            {
                XmlElement extraData = doc.CreateElement(DETAILS_NODENAME, XMLNS);
                extraData.InnerXml = additionalData.OuterXml;
                info.AppendChild(extraData);
            }

            //return the node
            XmlNode returnValue = info;
            return returnValue;
        }

        #endregion

        #region > Legacy methods <

        #region >Constants<

        private const string GET_TEMP_ID = "Gets a temporal Id to be used for submitted registry records.";
        private const string SAVE_REGISTRY_RECORD = "Saves a registry record for later use.";
        private const string GET_FORMATTED_XML = "A 'pretty-print' method: Accepts a an XML document and reformats it. <br />" +
            "Use, for example, to simplify comparison between a raw XML RegistryRecord and one returned by <br />" +
            "the GetFilteredRegistryXml() method. <br />" +
            "Input: an XML document string <br />" +
            "Output: a formatted XML string";
        private const string GET_FILTERED_REGISTRY_XML = "Accepts an XML representation of a RegistryRecord and re-generates it via the RegistryRecord object. <br />" +
            "This method demonstrates the filtering of unrecognized properties and attributes. <br />" +
            "Input: RegistryRecord XML string <br />" +
            "Output: filtered XML representation of the RegistryRecord";
        private const string USER_IS_IN_ROLES = "Determines whether the user has the provided role(s).<br />" +
            "Input: comma separated list of roles.<br />" +
            "Output: comma separated list of True/False values.";
        private const string RETRIEVE_NEW_REGISTRY_RECORD = "Retrieve NewRegistryRecord XML for use as template for Save or Register.<br />" +
            "Returns XmlWithAddins.<br />" +
            "If add-ins are not desired then the <AddIns> section should be removed from <MultiCompoundRegistryRecord>.";
        private const string CHECK_UNIQUE_REGISTRY_RECORD = "Check whether the registry record would be a duplicate permanent registry record.<BR>" +
            "duplicateCheck (C)ompound, (M)ixture, or (N)one.<br />" +
            "Alternatively, provide a structure string to check for an individual structure." +
            "<br />" + "Requires the 'SEARCH_REG' privilege.";
        private const string RETRIEVE_PICKLIST = "Returns XML with picklist.<br />" +
            "strCriteria is a SQL statement the returns two columns.<br />" +
            "The 1st column is a unique integer ID.<br />" +
            "The 2nd column is a unique string description.<br />" +
            "eg. SELECT PERSON_ID,USER_ID FROM CS_SECURITY.PEOPLE ORDER BY USER_ID<BR>" +
            "strCriteria can also be an interger ID value or a string DESCRIPTION value in the PICKLISTDOMAIN table.<br />" +
            "eg. 2 or Units";

        private const string CREATE_TEMPORARY_REGISTRY_RECORD = "Create a temporary (not yet registered) registry record." +
            "<br />" + "Requires the 'ADD_COMPOUND_TEMP' privilege.";
        private const string RETRIEVE_TEMPORARY_REGISTRY_RECORD = "Retrieve a temporary (not yet registered) registry record." +
            "<br />" + "Requires the 'SEARCH_TEMP' privilege.";
        private const string UPDATE_TEMPORARY_REGISTRY_RECORD = "Update an existing temporary (not yet registered) registry record." +
            "<br />" + "Requires the 'EDIT_COMPOUND_TEMP' privilege.";
        private const string DELETE_TEMPORARY_REGISTRY_RECORD = "Deletes a temporary (not yet registered) registry record." +
            "<br />" + "Requires the 'DELETE_TEMP' privilege.";

        private const string CREATE_REGISTRY_RECORD = "Create a permanent registry record.<br />" +
            "duplicateAction (D)uplicate, add (B)atch, put into (T)emporary, or do (N)ot store." +
            "<br />" + "Requires the 'REGISTER_DIRECT' privilege.";
        private const string RETRIEVE_REGISTRY_RECORD = "Retrieve a permanent registry record." +
            "<br />" + "Requires the 'SEARCH_REG' privilege.";
        private const string RETRIEVE_REGISTRY_INFO_FROM_TEMP_BATCH_ID = "Returns XML with information regarding temp to perm registrations." +
            "<br />" + "Requires the 'SEARCH_REG' privilege.";
        private const string UPDATE_REGISTRY_RECORD = "Update a permanent registry record." +
            "<br />" + "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string RETRIEVE_MATCHING_FRAGMENT_LIST = "Retrieve FragmentList matching the search expression.<br />" +
            "<br />" + "(Requires the 'SEARCH_REG' privilege.)" + "<br />" +
            "A search expression is one or more search terms separated by a vertical bar (|).<br />" +
            "The vertical bar (|) has the effect of an AND.<br />" +
            "A search term is a Name followed by one or more Operator Value pairs separated by OR.<br />" +
            "Name is a Column Name in the RegDB.VW_FRAGMENT view. Valid Names are:<br />" +
            "FRAGMENTID, CODE, DESCRIPTION, FRAGMENTTYPEID, MOLWEIGHT, FORMULA, CREATED, MODIFIED, STRUCTURE, and STRUCTUREFORMAT.<br />" +
            "Name is not case sensitive.<br />" +
            "The operators for numeric values are: =, >, <, and BETWEEN.<br />" +
            "When using BETWEEN two numeric values are separated by AND; eg. MolWeight between 90 AND 100.<br />" +
            "CODE =9 OR =17 is an example of using OR in a search term.<br />" +
            "The operators for structure values are: EXACT, SUBSTRUCTURE, SIMILARITY, and MOLWEIGHT.<br />" +
            "To use the SIMILARITY operator SIMTHRESHOLD should be specified in the search options.<br />" +
            "These operators are not case senstive. Note that the MOLWEIGHT operator is distinct from the MOLWEIGHT name.<br />" +
            "The case   senstive operators for text values are: Equals, Contains, StartsWith, EndWith, and NotContains.<br />" +
            "The case insenstive operators for text values are: equals, contains, startsWith, endWith, and notContains.<br />" +
            "Note that case of the the first letter of the operator detrmines whether the operator if case sensitive or not.";

        #endregion

        /// <summary>
        /// Generates a registration template XML string which can be filled out and
        /// <list type="bullet">
        /// <item>saved in a generic storage table as the template for a registration object,</item>
        /// <item>submitted as a temporary registration, or</item>
        /// <item>registered directly.</item>
        /// </list>
        /// The XML reflects all the customizations that have been configured at the time of execution.
        /// </summary>
        /// <returns>
        /// The template or 'prototype' XML string representing the registration object customized for your system.
        /// </returns>
        [WebMethod(false, Description = RETRIEVE_NEW_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string RetrieveNewRegistryRecord()
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();

                errorMessage = "Unable to retrieve the prototype registration XML.";
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Stores an XML string representation of a registration record in a generic storage table
        /// for later retrieval. Intended for very short term storage, as structures stored this way
        /// are not indexed and so not searchable.
        /// </summary>
        /// <remarks>
        /// Saving a record this way allows the user to use it as a template for multiple registrations
        /// using the ChemBioOffice Registration web application, allowing you to avoid repetitious steps
        /// such as drawing base structural entities for a related family of registrations.
        /// </remarks>
        /// <param name="registryRecordXml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>The unique identifier of the storage record containing the temporary XML.</returns>
        [WebMethod(false, Description = SAVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string SaveRegistryRecord(string registryRecordXml)
        {
            string errorMessage = null;
            bool canLoadFromXml = false;

            try
            {
                CheckCallerAuthentication();

                if (registryRecordXml.Contains("<MultiCompoundRegistryRecord>"))
                {
                    RegistryRecord registryRecord = null;
                    try
                    {
                        registryRecord = LoadRegistryRecordFromXml(registryRecordXml);
                        canLoadFromXml = registryRecord.IsSavable;
                        registryRecordXml = registryRecord.XmlWithAddIns;
                    }
                    catch (Exception e)
                    {
                        errorMessage = e.Message;
                        canLoadFromXml = false;
                    }
                }

                if (canLoadFromXml)
                {
                    //this should create a new object
                    string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                    COEGenericObjectStorageBO genericStorageObject = COEGenericObjectStorageBO.New(_databaseName);

                    genericStorageObject.COEGenericObject = registryRecordXml;
                    genericStorageObject.UserName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ? "unknown" : COEUser.Get();

                    //Here is where the object is persisted to the database
                    genericStorageObject.Save();

                    //Now you can get the ID that was added after the object was persisted
                    return genericStorageObject.ID.ToString();
                }

            }
            catch (Exception ex)
            {
                errorMessage = BuildAndThrowSoapException(errorMessage, ex);
            }
            return errorMessage;
        }

        /// <summary>
        /// Stores an XML string representation of a temporary (unregistered) registration record. The record
        /// is thus queued for later conversion to a permanent registration and (conditionally) any workflow
        /// approvals process.
        /// <para>
        /// <a href="..\..\create_temp_structure_only.xml"><b>View sample (XML format)</b></a>
        /// <br />
        /// <a href="..\..\create_temp_structure_only.txt"><b>View sample (plain text)</b></a>
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// <MultiCompoundRegistryRecord>
        ///   <ID>0</ID>
        ///   <DateCreated>0001/01/01 05:00:00</DateCreated>
        ///   <DateLastModified>0001/01/01 05:00:00</DateLastModified>
        ///   <PersonCreated>15</PersonCreated>
        ///   <StructureAggregation></StructureAggregation>
        ///   <RegNumber>
        ///     <RegID>0</RegID>
        ///     <SequenceNumber>0</SequenceNumber>
        ///     <SequenceID>1</SequenceID>
        ///     <RegNumber></RegNumber>
        ///   </RegNumber>
        ///   <Approved update="yes">Rejected</Approved>
        ///   <IdentifierList></IdentifierList>
        ///   <ProjectList></ProjectList>
        ///   <PropertyList>
        ///     <Property name="REG_COMMENTS"></Property>
        ///   </PropertyList>
        ///   <ComponentList>
        ///     <Component insert="yes">
        ///       <ID>0</ID>
        ///       <ComponentIndex>0</ComponentIndex>
        ///       <Percentage>0</Percentage>
        ///       <Compound insert="yes">
        ///         <CompoundID>-1</CompoundID>
        ///         <DateCreated>0001/01/01 05:00:00</DateCreated>
        ///         <DateLastModified>0001/01/01 05:00:00</DateLastModified>
        ///         <PersonCreated>15</PersonCreated>
        ///         <Tag></Tag>
        ///         <PersonRegistered>15</PersonRegistered>
        ///         <RegNumber>
        ///           <RegID>0</RegID>
        ///           <SequenceNumber>0</SequenceNumber>
        ///           <SequenceID>2</SequenceID>
        ///           <RegNumber></RegNumber>
        ///         </RegNumber>
        ///         <BaseFragment>
        ///           <ID>0</ID>
        ///           <Structure>
        ///             <StructureID>0</StructureID>
        ///             <StructureFormat></StructureFormat>
        ///             <Structure molWeight="0" insert="yes">VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMADwAAAENoZW1EcmF3IDEyLjAE
        /// AhAAzWz8/7Vg//8Sx1YARBQcAQEJCAAAAAAAAAAAAAIJCAAAANwCAAAoAg0IAQAB
        /// CAcBAAE6BAEAATsEAQAARQQBAAE8BAEAAAwGAQABDwYBAAENBgEAAEIEAQAAQwQB
        /// AABEBAEAAAoICAADAGAAyAADAAsICAAEAAAA8AADAAkIBAAzswIACAgEAAAAAgAH
        /// CAQAAAABAAYIBAAAAAQABQgEAAAAHgAECAIAeAADCAQAAAB4ACMIAQAFDAgBAAAo
        /// CAEAASkIAQABKggBAAECCBAAAAAkAAAAJAAAACQAAAAkAAEDAgAAAAIDAgABAAAD
        /// MgAIAP///////wAAAAAAAP//AAAAAP////8AAAAA//8AAAAA/////wAAAAD/////
        /// AAD//wABJAAAAAIAAwDkBAUAQXJpYWwEAOQEDwBUaW1lcyBOZXcgUm9tYW4BgC0A
        /// AAAEAhAAAAAAAAAAAAAAANwCAAAoAhYIBAAAACQAGAgEAAAAJAAZCAAAEAgCAAEA
        /// DwgCAAEAA4ABAAAABAIQAM1s/P+1YP//EsdWAEQUHAEEgAIAAAAAAggAFPBCAF9p
        /// LgAKAAIAAgAAAASAAwAAAAACCAAMfCUAX2kuAAoAAgADAAAABIAEAAAAAAIIABKr
        /// GwCM2REACgACAAQAAAAEgAUAAAAAAggAsMMzAAAAAAAKAAIABQAAAASABgAAAAAC
        /// CABO3EsAjNkRAAoAAgAGAAAABIAHAAAAAAIIAKhPFgDxSkgACgACAAcAAAAEgAgA
        /// AAAAAggADHwlAB5GYQAKAAIACAAAAASACQAAAAACCAAMfCUAWZ5/AAoAAgAJAAAA
        /// BIAKAAAAAAIIAAAAAACG0QkACgACAAoAAgQCAAgAKwQCAAAASAQAAAaAAAAAAAAC
        /// CADMrAMAhukFAAQCEADNbPz/hukFAGbGAwCGqQ0AIwgBAAACBwIAAAAABw0AAQAA
        /// AAMAYADIAAMATwAAAAAEgAsAAAAAAggA7BtSAPFKSAAKAAIACwAAAASADAAAAAAC
        /// CAAU8EIAHkZhAAoAAgAMAAAABIANAAAAAAIIAOwbUgA8KHsACgACAA0AAAAEgA4A
        /// AAAAAggAFPBCAFwKlQAKAAIADgAAAASADwAAAAACCADsG1IA7uuuAAoAAgAPAAAA
        /// BIAQAAAAAAIIABTwQgAMzsgACgACABAAAAAEgBEAAAAAAggA7BtSAGvL4QAKAAIA
        /// EQAAAASAEgAAAAACCAAU8EIA/az7AAoAAgASAAAABIATAAAAAAIIAKhPFgBEnJgA
        /// CgACABMAAAAEgBQAAAAAAggADHwlAGN+sgAKAAIAFAAAAASAFQAAAAACCACsAFMA
        /// RDwYAQoAAgAVAAIEAgAIACsEAgAAAEgEAAAGgAAAAAAAAggAeK1WAERUFAEEAhAA
        /// eG1PAERUFAESx1YARBQcAQEHAQAFAgcCAAAAAAcNAAEAAAADAGAAyAADAE8AAAAA
        /// BIAWAAAAAAIIAMIpKAD9rPsACgACABYAAgQCAAgAKwQCAAEASAQAAAaAAAAAAAAC
        /// CACO1isA/cT3AAQCEACOliQA/cT3ACjwKwAwuAYBIwgBAAACBwIAAAAFBwEAAQAH
        /// DgABAAAAAwBgAMgAAwBPSAAAAAAFgBcAAAAKAAIAFwAEBgQAEwAAAAUGBAAUAAAA
        /// AAAFgBgAAAAKAAIAGAAEBgQACQAAAAUGBAATAAAAAAAFgBkAAAAKAAIAGQAEBgQA
        /// BQAAAAUGBAAGAAAAAAAFgBoAAAAKAAIAGgAEBgQACwAAAAUGBAAMAAAAAAAFgBsA
        /// AAAKAAIAGwAEBgQADAAAAAUGBAANAAAAAAAFgBwAAAAKAAIAHAAEBgQADQAAAAUG
        /// BAAOAAAAAAAFgB0AAAAKAAIAHQAEBgQADgAAAAUGBAAPAAAAAAAFgB4AAAAKAAIA
        /// HgAEBgQADwAAAAUGBAAQAAAAAAAFgB8AAAAKAAIAHwAEBgQAEAAAAAUGBAARAAAA
        /// AAAFgCAAAAAKAAIAIAAEBgQAAgAAAAUGBAALAAAAAAAFgCEAAAAKAAIAIQAEBgQA
        /// BAAAAAUGBAAKAAAAAAYCAAIAAAAFgCIAAAAKAAIAIgAEBgQACAAAAAUGBAAJAAAA
        /// AAYCAAIAAwYCAAEACwYQACMAAAAAAAAAAAAAABgAAAAAAAWAIwAAAAoAAgAjAAQG
        /// BAAHAAAABQYEAAgAAAAAAAWAJAAAAAoAAgAkAAQGBAADAAAABQYEAAcAAAAAAAWA
        /// JQAAAAoAAgAlAAQGBAACAAAABQYEAAYAAAAAAAWAJgAAAAoAAgAmAAQGBAARAAAA
        /// BQYEABIAAAAAAAWAJwAAAAoAAgAnAAQGBAAEAAAABQYEAAUAAAAAAAWAKAAAAAoA
        /// AgAoAAQGBAADAAAABQYEAAQAAAAAAAWAKQAAAAoAAgApAAQGBAACAAAABQYEAAMA
        /// AAAABgIAAgADBgIAAQALBhAAJQAAACAAAAAkAAAAKAAAAAAABYAqAAAACgACACoA
        /// BAYEABIAAAAFBgQAFQAAAAAGAgACAAAABYArAAAACgACACsABAYEABIAAAAFBgQA
        /// FgAAAAAAAAAAAAAAAAA=
        /// </Structure>
        ///             <UseNormalization>T</UseNormalization>
        ///             <NormalizedStructure></NormalizedStructure>
        ///           </Structure>
        ///         </BaseFragment>
        ///         <PropertyList>
        ///           <Property name="CMP_COMMENTS"></Property>
        ///           <Property name="STRUCTURE_COMMENTS_TXT"></Property>
        ///           <Property name="CHEM_NAME_AUTOGEN"></Property>
        ///         </PropertyList>
        ///       </Compound>
        ///     </Component>
        ///   </ComponentList>
        ///   <BatchList>
        ///     <Batch insert="yes">
        ///       <BatchID>0</BatchID>
        ///       <BatchNumber>0</BatchNumber>
        ///       <FullRegNumber></FullRegNumber>
        ///       <DateCreated>0001/01/01 05:00:00</DateCreated>
        ///       <PersonCreated>15</PersonCreated>
        ///       <PersonRegistered>15</PersonRegistered>
        ///       <DateLastModified>0001/01/01 05:00:00</DateLastModified>
        ///       <StatusID></StatusID>
        ///       <PropertyList>
        ///         <Property name="SCIENTIST_ID" pickListDomainID="3"></Property>
        ///         <Property name="CREATION_DATE"></Property>
        ///         <Property name="NOTEBOOK_TEXT"></Property>
        ///         <Property name="AMOUNT"></Property>
        ///         <Property name="AMOUNT_UNITS" pickListDomainID="2"></Property>
        ///         <Property name="APPEARANCE"></Property>
        ///         <Property name="PURITY"></Property>
        ///         <Property name="PURITY_COMMENTS"></Property>
        ///         <Property name="SAMPLEID"></Property>
        ///         <Property name="SOLUBILITY"></Property>
        ///         <Property name="BATCH_COMMENT"></Property>
        ///         <Property name="STORAGE_REQ_AND_WARNINGS"></Property>
        ///         <Property name="FORMULA_WEIGHT"></Property>
        ///         <Property name="BATCH_FORMULA"></Property>
        ///         <Property name="PERCENT_ACTIVE"></Property>
        ///       </PropertyList>
        ///       <BatchComponentList>
        ///         <BatchComponent insert="yes">
        ///           <ID>0</ID>
        ///           <BatchID>0</BatchID>
        ///           <MixtureComponentID>0</MixtureComponentID>
        ///           <CompoundID>0</CompoundID>
        ///           <ComponentIndex>0</ComponentIndex>
        ///           <OrderIndex>1</OrderIndex>
        ///           <PropertyList>
        ///             <Property name="PERCENTAGE"></Property>
        ///           </PropertyList>
        ///           <BatchComponentFragmentList update="yes"></BatchComponentFragmentList>
        ///         </BatchComponent>
        ///       </BatchComponentList>
        ///     </Batch>
        ///   </BatchList>
        /// </MultiCompoundRegistryRecord>
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="xml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>The unique identifier of the temporary registration.</returns>
        [WebMethod(false, Description = CREATE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string CreateTemporaryRegistryRecord(string xml)
        {

            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "ADD_COMPOUND_TEMP" });

                errorMessage = "Unable to NewRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                errorMessage = "Unable to InitializeFromXml.";
                oRegistryRecord.InitializeFromXml(xml, true, false);

                errorMessage = "Unable to Save.";
                RegistryRecord savedRegistryRecord = oRegistryRecord.Save();
                if ((savedRegistryRecord == null) || (savedRegistryRecord.IsValid == false))
                    strRet = BuildAndThrowSoapException(errorMessage);

                strRet = savedRegistryRecord.ID.ToString();
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Retrieves the XML string representation of a temporary (unregistered) registration record
        /// from the registration queue.
        /// </summary>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = RETRIEVE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string RetrieveTemporaryRegistryRecord(int id)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_TEMP" });

                RegistryRecord oRegistryRecord = GetTempReg(id);
                if (oRegistryRecord == null)
                {
                    errorMessage = "GetRegistryRecord returned null.";
                    strRet = BuildAndThrowSoapException(errorMessage);
                }
                errorMessage = "XmlWithAddIns exception.";
                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Updates a queued temporary (unregistered) registration record. This allows changes to structure, property
        /// or custom data fields for either the compound or the batch information without generating an additional record.
        /// </summary>
        /// <param name="xml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>The unique identifier of the temporary registration.</returns>
        [WebMethod(false, Description = UPDATE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string UpdateTemporaryRegistryRecord(string xml)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_TEMP" });

                // Because the ID tag is first in the XML we can safely extract it without using formal XML methods
                int id;
                errorMessage = "Unable to extract ID.";
                {
                    int nLft = xml.IndexOf("<ID>");
                    if (nLft < 0)
                    {
                        strRet = BuildAndThrowSoapException(errorMessage);
                    }
                    nLft += "<ID>".Length;
                    int nRht = xml.IndexOf("</ID>", nLft);
                    if (nRht < 0)
                    {
                        strRet = BuildAndThrowSoapException(errorMessage);
                    }
                    id = Convert.ToInt32(xml.Substring(nLft, nRht - nLft));
                }

                errorMessage = "Unable to GetRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.GetRegistryRecord(id);
                if (oRegistryRecord == null)
                {
                    strRet = BuildAndThrowSoapException(errorMessage);
                }

                errorMessage = "Unable to InitializeFromXml.";
                oRegistryRecord.InitializeFromXml(xml, false, false);
                if (oRegistryRecord.IsValid == false)
                {
                    errorMessage = "Not Valid.";
                    strRet = BuildAndThrowSoapException(errorMessage);
                }

                errorMessage = "Unable to Save.";
                RegistryRecord savedRegistryRecord = oRegistryRecord.Save();
                if ((savedRegistryRecord == null) || (savedRegistryRecord.IsValid == false))
                {
                    strRet = BuildAndThrowSoapException(errorMessage);
                }
                strRet = savedRegistryRecord.ID.ToString();
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Deletes a temporary (unregistered) registration record.
        /// </summary>
        /// <remarks>This action is not reversible.</remarks>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        /// <returns>"Deleted" as an indicator of success.</returns>
        [WebMethod(false, Description = DELETE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string DeleteTemporaryRegistryRecord(int id)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "DELETE_TEMP" });

                errorMessage = "Unable to DeleteRegistryRecord.";
                RegistryRecord.DeleteRegistryRecord(id);
                strRet = "Deleted";
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Generates a new permanent registration from the XML submitted, depending on
        /// the default duplicate-detection algorithm and the <paramref name="duplicateAction"/> parameter used.
        /// </summary>
        /// <example>
        /// Provided the the compound is unique in the database, the project key is 'AB', and the previous entry
        /// in this project sequence was 125, the return value would look like this:
        /// <code>
        /// <![CDATA[
        /// <ReturnList>
        ///   <ActionDuplicateTaken>C</ActionDuplicateTaken>
        ///   <RegID>126</RegID>
        ///   <RegNum><AB-000126/RegNum>
        ///   <BatchNumber>1</BatchNumber>
        ///   <BatchID>193</BatchID>
        /// </ReturnList>
        /// ]]>
        /// </code>
        /// <i>RegID</i> is an automatically assigned, internal sequence number.
        /// The <i>BatchNumber</i> value indicates this is the first batch of this registration.<br />
        /// <i>BatchID</i> is also an automatically assigned, internal sequence number. However, note that
        /// its value is higher that the <i>RegID</i> value, indicating that some of the preceding registrations
        /// had multiple batches.
        /// </example>
        /// <remarks>
        /// NOTE: The outcome of this method is conditional, based on the registration data already stored
        /// and the automated resolution mechanism you choose.
        /// <para>
        /// The DuplicationAction table describes action the service will take if it would result
        /// in a duplication.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>DuplicateAction</term>
        /// <description>The enumeration describing duplicate-resolution action options.</description>
        /// </listheader>
        /// <item>
        /// <term>None (N)</term>
        /// <description>
        /// The submitted XML is ignored and the registration creation process is aborted.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Batch (B)</term>
        /// <description>
        /// A new batche of the existing component is created.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Duplicate (D)</term>
        /// <description>
        /// A new registration is created using a new component.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Temporary (T)</term>
        /// <description>
        /// A temporary (unregistered) registry record is created in lieu of duplicating a registered component.
        /// Temporary records are not subject to duplciate-checking.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="xml">The custom-formatted XML representation of a registration object.</param>
        /// <param name="duplicateAction">
        /// The single character provided represents the duplicate-resolution mechanism the service will use
        /// if matching structures are found in the permanent registry. 
        /// </param>
        /// <returns>
        /// An XML document representing the duplicate-resolution action taken (if any) and some key
        /// values from the new permanent registry record.
        /// </returns>
        [WebMethod(false, Description = CREATE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string CreateRegistryRecord(string xml, string duplicateAction)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "REGISTER_DIRECT" });
                DuplicateAction eDuplicateAction = this.VerifyDuplicateAction(duplicateAction);

                errorMessage = "Unable to exercise NewRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                errorMessage = "Unable to InitializeFromXml.";
                oRegistryRecord.InitializeFromXml(xml, true, false);

                errorMessage = "Unable to Save.";
                RegistryRecord savedRegistryRecord = oRegistryRecord.Register(eDuplicateAction);
                if ((savedRegistryRecord == null) || (savedRegistryRecord.IsValid == false))
                    strRet = BuildAndThrowSoapException(errorMessage);

                strRet = "<ReturnList>";
                strRet += "<ActionDuplicateTaken>" + savedRegistryRecord.ActionDuplicateTaken + "</ActionDuplicateTaken>";
                strRet += "<RegID>" + savedRegistryRecord.ID.ToString() + "</RegID>";
                if (savedRegistryRecord.RegNumber.RegNum != "")
                {
                    strRet += "<RegNum>" + savedRegistryRecord.RegNumber.RegNum + "</RegNum>";
                }
                strRet += "<BatchNumber>" + savedRegistryRecord.BatchList.Count + "</BatchNumber>";
                strRet += "<BatchID>" + savedRegistryRecord.BatchList[savedRegistryRecord.BatchList.Count - 1].ID + "</BatchID>";
                strRet += "</ReturnList>";
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Retrieves the XML string representation of a single registration record.
        /// <para>
        /// NOTE: The term 'permanent' does not indicate that the registration cannot be deleted,
        /// although not all users will have the permission to do so.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Unlike a temporary registration, a <i>permanent</i> registration is given a system
        /// identifier (a "registration number") based on settings available to Registration
        /// administrative users only.
        /// <para>
        /// The registration number is actually an alphanumeric value, derived using a combination
        /// of a project code (ex. 'AB') with an automated numeric sequence. This results in a
        /// system-assigned value such as 'AB-000001' for the first registration added to project 'AB'.
        /// </para>
        /// </remarks>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = RETRIEVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string RetrieveRegistryRecord(string regNum)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                RegistryRecord oRegistryRecord = GetPermReg(regNum);
                if (oRegistryRecord == null)
                {
                    errorMessage = "GetRegistryRecord returned null.";
                    strRet = BuildAndThrowSoapException(errorMessage);
                }
                errorMessage = "XmlWithAddIns exception.";
                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage + "\n" + ex.ToString());
            }
            return strRet;
        }

        /// <summary>
        /// Updates a permanent registration record.
        /// </summary>
        /// <param name="xml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>The registration number of the updated record.</returns>
        [WebMethod(false, Description = UPDATE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        string UpdateRegistryRecord(string xml)
        {
            //TODO: REMINDER - JED requests we honor the following authorizations:
            // (1) ADD_BATCH_PERM
            // (2) ADD_COMPONENT
            // (3) DELETE_BATCH_REG
            // (4) EDIT_COMPOUND_REG
            // (5) SET_APPROVED_FLAG
            //
            //This involves:
            // --> hydrating a new RR object with the incoming registration XML
            // --> fetching the existing RR object
            // --> performing object-to-object comparisons to determine if any of these rules have been broken

            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                //The document is parseable
                errorMessage = "Unable to parse the incoming text as a well-formed xml file.";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //Ensure the registration exists before trying to update it
                errorMessage = "Unable to determine this mixture's registry number.";
                string regNum = string.Empty;
                string regNumXPath = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
                XmlNode regNode = doc.SelectSingleNode(regNumXPath);
                regNum = regNode.InnerText;

                errorMessage = "Unable to GetRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.GetRegistryRecord(regNum);
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                oRegistryRecord.UpdateFromXml(xml);

                //errorMessage = "Unable to InitializeFromXml.";
                //oRegistryRecord.InitializeFromXml(xml, false, false);

                //errorMessage = "Unable to Save.";
                //RegistryRecord savedRegistryRecord = oRegistryRecord.SaveFromCurrentXml();
                //if ((savedRegistryRecord == null) || (savedRegistryRecord.IsValid == false))
                //    strRet = BuildAndThrowSoapException(errorMessage);

                strRet = oRegistryRecord.RegNumber.RegNum;
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Retrieves the next temporary registration number from the Registration repository.
        /// </summary>
        /// <returns>A registration number, reserved for the caller.</returns>
        [WebMethod(false, Description = GET_TEMP_ID)]
        [SoapHeader(CREDENTIALS)]
        public string GetTempID()
        {
            string buf = string.Empty;

            try
            {
                CheckCallerAuthentication();
                buf = GetTempIDCommand.Execute();
            }
            catch (Exception ex)
            {
                return BuildAndThrowSoapException(null, ex);
            }
            return buf;
        }

        /// <summary>
        /// Using the duplicate-detection algorithm provided by the <paramref name="duplicateCheck"/> parameter,
        /// checks if mixtures or compounds exist in the database that match the submitted mixture
        /// (the <paramref name="xml"/> parameter) or any of its components. The method will return null or
        /// an empty string if no matches are found. If matches are found, they are returned as an abbreviated
        /// list of system identifiers for those registrations or compounds.
        /// </summary>
        /// <example>
        /// The matching algorithms can be tested by submitting a structure that is already in the database.
        /// <list type="number">
        /// <item>
        /// Retrieve a template XML using the <see cref="RetrieveNewRegistryRecord"/> method.
        /// Copy it to an empty text file.
        /// </item>
        /// <item>
        /// Retrieve an existing mixture's XML representation using the <see cref="RetrieveRegistryRecord"/> method.
        /// </item>
        /// <item>
        /// From this existing record, copy the first component's &lt;Structure&gt; node text into the
        /// corresponding &lt;Structure&gt; node in the text file.
        /// </item>
        /// <item>
        /// Use the entire 'new' XML string from the text file as the <paramref name="xml"/> parameter value.
        /// </item>
        /// <item>
        /// Use 'C' for the <paramref name="duplicateCheck"/> parameter value.
        /// </item>
        /// </list>
        /// Provided that entity (in this example, the sole structure associated with registration AB-000526)
        /// is unique in the database, the return value should look like this:
        /// <code>
        /// <![CDATA[
        /// <Response message="1 duplicated component.">
        ///   <Error>
        ///     <COMPOUNDLIST>
        ///       <COMPOUND><TEMPCOMPOUNDID>-1</TEMPCOMPOUNDID>
        ///         <REGISTRYLIST>
        ///           <REGNUMBER count="1" CompoundID="526" SAMEFRAGMENT="True" SAMEEQUIVALENT="True">AB-000526</REGNUMBER>
        ///         </REGISTRYLIST>
        ///       </COMPOUND>
        ///     </COMPOUNDLIST>
        ///   </Error>
        ///   <Result></Result>
        /// </Response>
        /// ]]>
        /// </code>
        /// The matching algorithm ignores everything except the component structure data of the
        /// <paramref name="duplicateCheck"/> parameter while performing the evaluation. 
        /// </example>
        /// <param name="xml">The custom-formatted XML representation of a registration object.</param>
        /// <param name="duplicateCheck">The single character provided represents the duplicate-checking mechanism the service will use.</param>
        /// <returns>Either an empty string (no matches found) or an XML representation of the matches</returns>
        [WebMethod(false, Description = CHECK_UNIQUE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string CheckUniqueRegistryRecord(string xml, string duplicateCheck)
        {
            string strRet = string.Empty;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });
                DuplicateCheck eDuplicateCheck = VerifyDuplicateCheck(duplicateCheck);

                errorMessage = "Unable to NewRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                if (xml.Trim().StartsWith("<MultiCompound"))
                {
                    errorMessage = "Unable to InitializeFromXml.";
                    oRegistryRecord.InitializeFromXml(xml, true, true);

                    errorMessage = "Unable to CheckUniqueRegistryRecord.";
                    strRet = oRegistryRecord.CheckUniqueRegistryRecord(eDuplicateCheck);
                    int at = strRet.IndexOf('<');
                    if (at >= 0)
                        strRet = strRet.Substring(at);
                }
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Based on a batch identifier, provides the caller with the XML string representation
        /// of a single registration record.
        /// <para>
        /// This functionality is intended to allow the caller to use the temporary identifier (created
        /// when the 'temporary' registration was submitted) to find the permanent registration once it
        /// has been promoted by a user in a supervisory role.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Applications integrating with the Registration system can submit a registration
        /// </remarks>
        /// <param name="tempBatchID">The unique identifier of the original temporary registration.</param>
        /// <returns>
        /// An XML document (see below for format) containing specific data-points describing the identifiers
        /// for the various parts of the complete registration record. The "RegNumber" can subsequently
        /// be passed to the <see cref="RetrieveRegistryRecord"/> method to fetch the now-permanent
        /// registration record as XML.
        /// <para>
        /// <code>
        /// <![CDATA[  
        /// <ReturnList>
        ///   <RegNumber></RegNumber>
        ///   <ComponentRegNumberList></ComponentRegNumberList>
        ///   <BatchNumberList></BatchNumberList>
        /// </ReturnList>
        /// ]]>
        /// </code>
        /// </para>
        /// </returns>
        [WebMethod(false, Description = RETRIEVE_REGISTRY_INFO_FROM_TEMP_BATCH_ID)]
        [SoapHeader(CREDENTIALS)]
        public string RetrieveRegistryInfoFromTempBatchID(int tempBatchID)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                errorMessage = "Unable to NewRegistryRecord.";
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                if (oRegistryRecord == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                errorMessage = "Unable to CheckUniqueRegistryRecord.";
                strRet = oRegistryRecord.GetRegisteredInfoFromTempBatchID(tempBatchID);
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Retrieves an XML representation of a key/value list that can be used as GUI selector lists
        /// or as a validation tool (to ensure data-relational validity).
        /// </summary>
        /// <param name="strCriteria">Either the unique identifier or the name of the picklist.</param>
        /// <returns>
        /// An XML document representative of the specific picklist requested.
        /// <para>
        /// Given the picklist domain ID <b>2</b>, or the picklist domain name <b>Units</b>,
        /// the result might look like this:
        /// <code>
        /// <![CDATA[
        /// <Picklist id="2" name="Units" count="4">
        ///   <PicklistItem ID="4">g</PicklistItem>
        ///   <PicklistItem ID="6">l</PicklistItem>
        ///   <PicklistItem ID="3">mg</PicklistItem>
        ///   <PicklistItem ID="5">ml</PicklistItem>
        /// </Picklist>
        /// ]]>
        /// </code>
        /// </para>
        /// </returns>
        [WebMethod(false, Description = RETRIEVE_PICKLIST)]
        [SoapHeader(CREDENTIALS)]
        string RetrievePicklist(string strCriteria)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();

                Picklist oPicklist = null;
                PicklistDomain oPicklistDomain = null;
                errorMessage = "Unable to GetPicklist.";
                int nCriteria;
                string domainName = string.Empty;
                string domainId = string.Empty;

                // Assume SQL - Should check against harmful inner expressions
                if (strCriteria.ToUpper().StartsWith("SELECT "))
                    oPicklist = Picklist.GetPicklist(strCriteria);

                // Assume PicklistDomain ID
                if (oPicklist == null)
                {
                    Int32.TryParse(strCriteria, out nCriteria);
                    if (nCriteria > 0)
                    {
                        oPicklistDomain = PicklistDomain.GetPicklistDomain(nCriteria);
                        if (oPicklistDomain != null)
                            oPicklist = Picklist.GetPicklist(oPicklistDomain);
                    }
                }

                // Assume PicklistDomain DESCRIPTION
                if (oPicklist == null)
                {
                    oPicklistDomain = PicklistDomain.GetPicklistDomain(strCriteria);
                    oPicklist = Picklist.GetPicklist(oPicklistDomain);
                }

                if (oPicklist == null)
                    strRet = BuildAndThrowSoapException(errorMessage);

                if (oPicklistDomain != null)
                {
                    domainName = oPicklistDomain.Description;
                    domainId = oPicklistDomain.Identifier.ToString();
                }

                //JED: provide additional information using root node attributes
                strRet = string.Format(
                    "<Picklist id=\"{0}\" name=\"{1}\" count=\"{2}\">",
                    domainId, domainName, oPicklist.PickList.Count.ToString()
                );

                foreach (KeyValuePair<int, string> kvp in oPicklist.PickList)
                {
                    strRet += "<PicklistItem ID=\"" + kvp.Key.ToString() + "\">" + kvp.Value + "</PicklistItem>";
                }
                strRet += "</Picklist>";
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Given a search parameter string, generates an XML document representing a list of matching
        /// fragments and their properties.
        /// The search expression is a SQL-like combination of
        /// <list type="bullet">
        /// <item>a <i>field</i> which is a field in the REGDB.VW_FRAGMENTS view,</item>
        /// <item><i>operator</i>, either a standard numeric operator or a custom verb,</item>
        /// <item>and <i>value</i> that is the search value itself.</item>
        /// </list>
        /// For example, if the value of <paramref name="searchExpression"/> is "description contains maleate",
        /// then the results will include all fragments that have the term "maleate" in their descriptions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The possible <i>field</i> values are:
        /// <list type="table">
        /// <listheader>
        /// <term>Field</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>FRAGMENTID</term>
        /// <description>The unique identifier for the fragment (numeric)</description>
        /// </item>
        /// <item>
        /// <term>CODE</term>
        /// <description>A secondary identifier value</description>
        /// </item>
        /// <item>
        /// <term>DESCRIPTION</term>
        /// <description>Common name</description>
        /// </item>
        /// <item>
        /// <term>FRAGMENTTYPEID</term>
        /// <description>1&#61;salt, 2&#61;solvate</description>
        /// </item>
        /// <item>
        /// <term>MOLWEIGHT</term>
        /// <description>Molecular weight</description>
        /// </item>
        /// <item>
        /// <term>FORMULA</term>
        /// <description>Molecular formula</description>
        /// </item>
        /// <item>
        /// <term>CREATED, MODIFIED</term>
        /// <description>The date of creation or last modification</description>
        /// </item>
        /// <item>
        /// <term>STRUCTURE</term>
        /// <description>Currently, a ChemDraw-formatted string representing the chemical structure</description>
        /// </item>
        /// <item>
        /// <term>STRUCTUREFORMAT</term>
        /// <description>Unused at this time</description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// The possible <i>operator</i> values are;
        /// <list type="table">
        /// <listheader>
        /// <term>Operator</term><description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>&#61;, &gt;, &lt;, and BETWEEN</term>
        /// <description>
        /// The unique identifier for the fragment (numeric). (When using BETWEEN, two numeric values are separated by AND)
        /// </description>
        /// </item>
        /// <item>
        /// <term>Equals, Contains, StartsWith, EndWith, and NotContains</term>
        /// <description>Case-sensitive verbs to be used for text fields</description>
        /// </item>
        /// <item>
        /// <term>equals, contains, startsWith, endWith, and notContains</term>
        /// <description>Case-INsensitive verbs used with text fields</description>
        /// </item>
        /// <item>
        /// <term>EXACT, SUBSTRUCTURE, SIMILARITY, and MOLWEIGHT</term>
        /// <description>For use with the STRUCTURE field only</description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>Some <paramref name="searchExpression"/> values and their possible results:</para>
        /// <para>
        /// "description contains zinc"
        /// <code>
        /// <![CDATA[
        /// <SearchResults>
        ///   <Table_11>
        ///     <CODE>54</CODE>
        ///     <DESCRIPTION>Zinc(II) salt</DESCRIPTION>
        ///     <FRAGMENTTYPEID>1</FRAGMENTTYPEID>
        ///     <MOLWEIGHT>65.39</MOLWEIGHT>
        ///     <FORMULA>Zn+2</FORMULA>
        ///     <CREATED>2009-06-30T02:02:32-04:00</CREATED>
        ///     <MODIFIED>2009-06-30T02:02:32-04:00</MODIFIED>
        ///     <STRUCTURE>
        ///         VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAGAAAAENzQ2FydHJpZGdlIDEx
        ///         LjAuMC4xNjUAAw4AAgD///////8AAAAAAAABgAAAAAADgAQAAAAEgAEAAAACBAIA
        ///         HgArBAIAAAAhBAEAAgAAAAAAAAAAAAA=
        ///       </STRUCTURE>
        ///     <FRAGMENTID>54</FRAGMENTID>
        ///   </Table_11>
        /// </SearchResults>
        /// ]]>
        /// </code>
        /// </para>
        /// <para>
        /// "formula contains CH3"
        /// <code>
        /// <![CDATA[
        /// <SearchResults>
        ///   <Table_11>
        ///     <CODE>88</CODE>
        ///     <DESCRIPTION>Methyl hydrogen sulphate</DESCRIPTION>
        ///     <FRAGMENTTYPEID>1</FRAGMENTTYPEID>
        ///     <MOLWEIGHT>111.097120</MOLWEIGHT>
        ///     <FORMULA>CH3O4S-</FORMULA>
        ///     <CREATED>2009-06-30T02:02:32-04:00</CREATED>
        ///     <MODIFIED>2009-06-30T02:02:32-04:00</MODIFIED>
        ///     <STRUCTURE>
        ///         VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAGAAAAENzQ2FydHJpZGdlIDEx
        ///         LjAuMC4xNjUAAw4AAgD///////8AAAAAAAABgAAAAAADgA4AAAAEgAEAAAAAAASA
        ///         AgAAAAIEAgAIAAAABIADAAAAAgQCABAAAAAEgAQAAAACBAIACAAAAASABQAAAAIE
        ///         AgAIAAAABIAGAAAAAgQCAAgAKwQCAAAAIQQBAP8AAAWACAAAAAQGBAABAAAABQYE
        ///         AAIAAAAAAAWACQAAAAQGBAACAAAABQYEAAMAAAAAAAWACgAAAAQGBAADAAAABQYE
        ///         AAQAAAAABgIAAgAAAAWACwAAAAQGBAADAAAABQYEAAUAAAAABgIAAgAAAAWADAAA
        ///         AAQGBAADAAAABQYEAAYAAAAAAAAAAAAAAAAA
        ///       </STRUCTURE>
        ///     <FRAGMENTID>88</FRAGMENTID>
        ///   </Table_11>
        ///   <Table_11>
        ///     <CODE>518</CODE>
        ///     <DESCRIPTION>Methoxide</DESCRIPTION>
        ///     <FRAGMENTTYPEID>1</FRAGMENTTYPEID>
        ///     <MOLWEIGHT>31.033920</MOLWEIGHT>
        ///     <FORMULA>CH3O-</FORMULA>
        ///     <CREATED>2009-06-30T02:02:34-04:00</CREATED>
        ///     <MODIFIED>2009-06-30T02:02:34-04:00</MODIFIED>
        ///     <STRUCTURE>
        ///         VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAGAAAAENzQ2FydHJpZGdlIDEx
        ///         LjAuMC4xNjUAAw4AAgD///////8AAAAAAAABgAAAAAADgAYAAAAEgAEAAAAAAASA
        ///         AgAAAAIEAgAIACsEAgAAACEEAQD/AAAFgAQAAAAEBgQAAQAAAAUGBAACAAAAAAAA
        ///         AAAAAAAAAA==
        ///       </STRUCTURE>
        ///     <FRAGMENTID>518</FRAGMENTID>
        ///   </Table_11>
        /// </SearchResults>
        /// ]]>
        /// </code>
        /// </para>
        /// </example>
        /// <param name="searchExpression">String containing the required combination of [field][operator][value] values.</param>
        /// <returns>
        /// An XML document containing a list of all the fragments matching the <paramref name="searchExpression"/> provided.
        /// If no fragments match the search criteria provided, the method returns simply "&lt;SearchResults /&gt;".
        /// </returns>
        [WebMethod(false, Description = RETRIEVE_MATCHING_FRAGMENT_LIST)]
        [SoapHeader(CREDENTIALS)]
        public string RetrieveMatchingFragmentList(string searchExpression)
        {
            string strRet;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                errorMessage = "new COESearch exception.";
                COESearch oCOESearch = new COESearch(4007); // <-------- Should not be hard-coded

                errorMessage = "new SearchInput exception.";
                SearchInput oSearchInput = new SearchInput();
                oSearchInput.FieldCriteria = searchExpression.Split('|');
                // Prepend "VW_FRAGMENT." iif Name does not contain a '.'
                for (int index = 0; index < oSearchInput.FieldCriteria.Length; index++)
                {
                    string fieldCriteria = oSearchInput.FieldCriteria[index];
                    string[] strTerm = fieldCriteria.Split(' ');
                    if (strTerm[0].Contains(".") == false)
                        oSearchInput.FieldCriteria[index] = "VW_FRAGMENT." + oSearchInput.FieldCriteria[index];
                }
                oSearchInput.ReturnPartialResults = false;
                oSearchInput.ReturnSimilarityScores = false;
                //oSearchInput.SearchOptions;
                //
                string[] resultFields = new string[] {
                    "VW_FRAGMENT.FRAGMENTID",
                    "VW_FRAGMENT.CODE",
                    "VW_FRAGMENT.DESCRIPTION",
                    "VW_FRAGMENT.FRAGMENTTYPEID",
                    "VW_FRAGMENT.MOLWEIGHT",
                    "VW_FRAGMENT.FORMULA",
                    "VW_FRAGMENT.CREATED",
                    "VW_FRAGMENT.MODIFIED",
                    "VW_FRAGMENT.STRUCTURE",
                    "VW_FRAGMENT.STRUCTUREFORMAT",
                };
                //
                errorMessage = "new ResultPageInfo exception.";
                ResultPageInfo oResultPageInfo = new ResultPageInfo();
                //
                errorMessage = "oCOESearch.DoSearch exception.";
                DataResult oDataResult = oCOESearch.DoSearch(oSearchInput, resultFields, oResultPageInfo);
                strRet = oDataResult.Status;
                if (strRet == "SUCCESS")
                {
                    strRet = oDataResult.ResultSet;
                }
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Determines the permission state(s) of the query-list of permission names for authenticated user
        /// </summary>
        /// <param name="vstrRoles">Comma-delimited list of roles (permissions) to verify.</param>
        /// <returns>
        /// Comma-delimited list of boolean values corresponding to order of the the
        /// <paramref name="vstrRoles"/> query-list
        /// </returns>
        [WebMethod(false, Description = USER_IS_IN_ROLES)]
        [SoapHeader(CREDENTIALS)]
        public string UserIsInRoles(string vstrRoles)
        {
            //JED: Why is this 'UserIsInRoles' method exposed?
            string strRet = string.Empty;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();

                string[] strRoles = vstrRoles.Split(',');
                foreach (string strRole in strRoles)
                {
                    if (strRet != "") strRet += ",";
                    strRet += Csla.ApplicationContext.User.IsInRole(strRole).ToString();
                }
            }
            catch (Exception ex)
            {
                strRet = BuildAndThrowSoapException(errorMessage, ex);
            }
            return strRet;
        }

        /// <summary>
        /// Provides indentation-formatting of any XML document, but is specifically provided as a
        /// utility method for visually comparing registration record XML to that returned by the
        /// <see cref="GetFilteredRegistryXml"/> method.
        /// </summary>
        /// <param name="sourceXml">Any well-formed XML document string.</param>
        /// <returns>An indented, pretty-printed version of the <paramref name="searchExpression"/> submitted.</returns>
        [WebMethod(false, Description = GET_FORMATTED_XML)]
        [SoapHeader(CREDENTIALS)]
        public string GetFormattedXml(string sourceXml)
        {
            string result = sourceXml;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();

                errorMessage = "Unable to read and/or format the document provided. Please provide a valid XML document.";
                result = Utilities.FormatXmlString(result);
            }
            catch (Exception ex)
            {
                result = BuildAndThrowSoapException(errorMessage, ex);
            }
            return result;
        }

        /// <summary>
        /// Demonstrates the standardization of registration XML documents submitted by service callers.
        /// Callers should submit their XML and compare it against the prototypical XML format expected by
        /// other methods of this service.
        /// </summary>
        /// <remarks>
        /// To best support multiple points of integration between the Registration Services and other applications
        /// (ex. CambridgeSoft's Inventory and Electronic Laboratory Notebook modules), the service will
        /// silently bypass unrecognized XML elements to allow for callers to use a slightly modified version of the
        /// registration system standard. Comparison between the incoming and outgoing XML will alert the caller
        /// to which elements will be dropped in order to meet the registration standard.
        /// </remarks>
        /// <param name="registryRecordXml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>
        /// An XML string representation of the 'hydrated' registration object, minus any unrecognized
        /// properties, attributes or xml nodes that were not be interpreted by the registration object.
        /// </returns>
        [WebMethod(false, Description = GET_FILTERED_REGISTRY_XML)]
        [SoapHeader(CREDENTIALS)]
        public string GetFilteredRegistryXml(string registryRecordXml)
        {
            string result = string.Empty;
            string outgoingRegXml = string.Empty;
            string errorMessage = null;

            try
            {
                CheckCallerAuthentication();

                errorMessage = "Unable to create the prototype registration object.";
                RegistryRecord registry = RegistryRecord.NewRegistryRecord();

                errorMessage = "Unable to initialize the prototype registration object.";
                if (registry == null)
                    result = BuildAndThrowSoapException(errorMessage);

                registry.InitializeFromXml(registryRecordXml, false, false);
                registry.UpdateXml();
                result = registry.Xml;
                result = Utilities.FormatXmlString(result);
            }
            catch (Exception ex)
            {
                result = BuildAndThrowSoapException(errorMessage, ex);
            }
            return result;
        }

        #region >Private methods<

        static COELog _coeLog = COELog.GetSingleton(SERVICENAME);

        /// <summary>
        /// Privided a list of permissions, determines if the authenticated caller possesses at
        /// least one of them and throws an exception otherwise
        /// </summary>
        /// <param name="permissions">List of names of permissions</param>
        private void CheckCallerAuthorizations(string[] permissions)
        {
            bool isAuthorized = false;
            foreach (string permission in permissions)
            {
                if (Csla.ApplicationContext.User.IsInRole(permission) == true)
                    isAuthorized = true; break;
            }
            if (!isAuthorized)
                throw new System.Security.AccessControl.PrivilegeNotHeldException(string.Join(",", permissions));
        }

        /// <summary>
        /// Performs the act of initializing a new registry record object with xml
        /// retrieved either from the repository or from an external source.
        /// </summary>
        /// <param name="registryRecordXml"></param>
        /// <returns></returns>
        private RegistryRecord LoadRegistryRecordFromXml(string registryRecordXml)
        {
            //TODO: This 'prototype' object should be cached.
            // Cached clones can be initialized and returned to the caller.
            // The cache should be invalidated if the object definition changes.
            RegistryRecord registryRecord = RegistryRecord.NewRegistryRecord();
            registryRecord.InitializeFromXml(registryRecordXml, true, false);
            return registryRecord;
        }

        private string BuildAndThrowSoapException(string errorMessage)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
            node.InnerText = errorMessage;
            throw new SoapException(errorMessage, SoapException.ClientFaultCode, Context.Request.Url.AbsoluteUri, node);
        }

        private string BuildAndThrowSoapException(string errorMessage, Exception ex)
        {
            Exception baseEx = ex.GetBaseException();
            _coeLog.Log(errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
                return BuildAndThrowSoapException(baseEx.Message);
            else
            {
                string errMsg = string.Empty;
                _coeLog.Log(ex.ToString(), 0, System.Diagnostics.SourceLevels.Error);
                return BuildAndThrowSoapException(errorMessage + "\r\n" + baseEx.Message);
            }
        }

        #endregion

        #endregion

    }
}
