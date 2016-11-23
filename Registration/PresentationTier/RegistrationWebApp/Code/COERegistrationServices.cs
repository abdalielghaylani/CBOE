using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.Common;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.Common.Exceptions;

using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using RegistrationWebApp.Code;

namespace CambridgeSoft.COE.Registration.Services
{
    /// <summary>
    /// Summary description for COERegistrationServices
    /// <para>
    /// NOTE: Throughout the registration system, there are three 'types' of registration records
    /// that might be part of a chemical registration's life-cycle.
    /// <list type="table">
    /// <listheader>
    /// <term>Registration type</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>Template</term>
    /// <description>
    /// An xml document stored (<i>saved</i>) without structure indexing.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Temporary</term>
    /// <description>
    /// A registration stored (<i>submitted</i>) with its own search context and fewer
    /// permissions restrictions, intended to require supervisory acceptance.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Permanent</term>
    /// <description>
    /// Registration records stored (<i>registered</i>) with this status have their own
    /// search context, enhanced search mechanisms, and generally tighter access/edit restrictions.
    /// <para>
    /// Registration records can have this status at time of creation, or they can be promoted
    /// to this status by way of approval of a temporary record.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    [WebService(
            Namespace = "CambridgeSoft.COE.Registration.Services.Web",
            Description = "Web Services for Registration. For more information see <a href='COERegistrationServicesTesting.htm'>COERegistrationServices Testing</a>",
            Name = "COERegistrationServices"
    )]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class COERegistrationServices : System.Web.Services.WebService
    {
        #region >Constants<

        private const string XMLNS = "www.cambridgesoft.com";
        private const string XPATH_REG_NUM = "/MultiCompoundRegistryRecord/RegNumber/RegNumber";
        private const string XPATH_REG_ID = "/MultiCompoundRegistryRecord/ID";

        private const string CREDENTIALS = "Credentials";
        private const string AUTHORIZATION_FAILURE_MSG = "Caller is not authorized to perform this action.";
        private const string AUTHENTICATION_ERR_MSG = "Unable to authenticate the request; please provide authentication credentials " +
            "via the SOAP header";

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

        private const string ADD_BATCH = "Retrieve a permanent registry record.<br />" +
            "Add a new batch to specific record based on incoming batch xml.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string ADD_COMPONENET = "Retrieve a permanent registry record.<br />" +
            "Add a new component to specific record based on incoming batch xml.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string DELETE_BATCH = "Deletes a batch from the specific registry record.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string DELETE_COMPONENT = "Deletes a component from the specific registry record.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string UPDATE_BATCH = "Update batch list for a specific registry record.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string UPDATE_COMPONENT = "Update component list for a specific registry record.<br />" +
            "Requires the 'EDIT_COMPOUND_REG' privilege.";

        private const string GET_PROTOTYPE_XML = "Get the prototype xml for the specified object type";

        private const string GET_REGISTRATION_SETTING = "Get the Registration setting value of a specified key.";

        private const string SET_MODULE_NAME = "Add a new node 'ModuleName' in configuration file. <br /> Chem Warning addin will take proper action as per module configuration.";

        private const string CREATE_PERM_REGPRIVILEGE_MSG = "Sorry, you don't have sufficient user privileges to execute a permanent registration. Please contact your administrator";

        private const string CREATE_TEMP_REGPRIVILEGE_MSG = "Sorry, you don't have sufficient user privileges to execute a temporary registration. Please contact your administrator";

        private const string CREATE_REGWITH_NOPRIVILEGE_MSG = "Sorry, you don't have sufficient user privileges to execute registration. Please contact your administrator";

        private const string SEARCH_REGWITH_NOPRIVILEGE_MSG = "Sorry, you don't have sufficient user privileges to search registration. Please contact your administrator";
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
        public COERegistrationServices()
        {
            Csla.ApplicationContext.GlobalContext["AppName"] = ConfigurationManager.AppSettings["AppName"];
            //Uncomment the following line if using designed components
            //InitializeComponent(); 
        }

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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();

                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }


        /// <summary>
        /// This method is for creating and adding a node with module name.
        /// This will help to identify calling module and Chem Warning addin can take proper action as per module configuration.
       /// </summary>
       /// <param name="registryRecordXml">Registration xml</param>
       /// <param name="ModuleName">Module name as string</param>
       /// <returns></returns>
        [WebMethod(false, Description = SET_MODULE_NAME)]
        [SoapHeader(CREDENTIALS)]
        public string SetModuleName(string registryRecordXml, string ModuleName)
        {
            string strRet = string.Empty;
            try
            {
                CheckCallerAuthentication();
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                strRet = oRegistryRecord.AddNewNode(registryRecordXml, ModuleName);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();

                string xml = RemoveContentBeforeProlog(registryRecordXml);
                RegistryRecord registryRecord = LoadRegistryRecordFromXml(xml);
                xml = registryRecord.XmlWithAddIns;

                string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                COEGenericObjectStorageBO genericStorageObject = COEGenericObjectStorageBO.New(_databaseName);

                genericStorageObject.COEGenericObject = xml;
                genericStorageObject.UserName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ? "unknown" : COEUser.Get();
                genericStorageObject.Save();

                strRet = genericStorageObject.ID.ToString();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// Stores an XML string representation of a registration record in a generic storage table
        /// for later retrieval. Intended for very short term storage and only differnce with  SaveRegistryRecord is this
        /// save the template with user prefernce value merged
        /// </summary>
        /// <param name="registryRecordXml">The custom-formatted XML representation of a registration object.</param>
        /// <returns>The unique identifier of the storage record containing the temporary XML.</returns>
        [WebMethod(false, Description = SAVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string SaveRegistryRecordWithUserPreference(string registryRecordXml)
        {
            string strRet = null;                    

            try
            {                    
              
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "ADD_COMPOUND_TEMP" });
                string xml = RemoveContentBeforeProlog(registryRecordXml);
                RegistryRecord registryRecord = LoadRegistryRecordFromXml(xml);

                registryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.ELN;
                ChemDrawWarningChecker theChemDrawWarningChecker = new ChemDrawWarningChecker();
                if (!theChemDrawWarningChecker.CheckRedBoxWarnings(registryRecord))
                {
                    //Fetching the user preference values and merging to the original reg record.
                    COEGenericObjectStorageBO genericuserPreferenceBO = COEGenericObjectStorageBO.Get(COEUser.Name, (int)FormGroups.UserPreference);
                    if (genericuserPreferenceBO != null && genericuserPreferenceBO.ID > 0)
                        registryRecord.UpdateUserPreference(genericuserPreferenceBO.COEGenericObject);
                    xml = registryRecord.XmlWithAddIns;
                    string _databaseName = COEConfiguration.GetDatabaseNameFromAppName(COEAppName.Get().ToString());
                    COEGenericObjectStorageBO genericStorageObject = COEGenericObjectStorageBO.New(_databaseName);
                    genericStorageObject.COEGenericObject = xml;
                    genericStorageObject.UserName = (COEUser.Get() == null || COEUser.Get() == string.Empty) ? "unknown" : COEUser.Get();
                    genericStorageObject.Save();

                    strRet = genericStorageObject.ID.ToString();
                }
                else
                {
                    List<BrokenRuleDescription> brokenRuleDescriptionList = registryRecord.GetBrokenRulesDescription();
                    if(brokenRuleDescriptionList.Count > 0)
                        throw new ValidationException(brokenRuleDescriptionList[0].BrokenRulesMessages[0]);                  
                }

            }
            catch (Exception ex)
            {
                                
                if (ex.GetBaseException() is ValidationException)
                    strRet = FormatBrokenRulesMessage(((ValidationException)ex.GetBaseException()), new List<BrokenRuleDescription>());
                else if (ex is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = CREATE_REGWITH_NOPRIVILEGE_MSG;
                else
                    COEExceptionDispatcher.HandleBLLException(ex);
                
            }
            return strRet;
        }
     

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
        public string CreateTemporaryRegistryRecord(string xml)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "ADD_COMPOUND_TEMP" });

                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();

                try
                {
                    string newXml = RemoveContentBeforeProlog(xml);
                    oRegistryRecord.InitializeFromXml(newXml, true, false);
                    oRegistryRecord.BatchPrefixDefaultOverride(true);
                    RegistryRecord savedRegistryRecord = oRegistryRecord.Save();
                    strRet = savedRegistryRecord.ID.ToString();
                }
                catch (Exception exception)
                {
                    if (exception.GetBaseException() is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                        ShowBrokenRules(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                    }
                    else
                    {
                        COEExceptionDispatcher.HandleUIException(exception);
                    }
                }
            }
            catch (Exception otherExcetion)
            {
                if (otherExcetion is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = CREATE_TEMP_REGPRIVILEGE_MSG;
                else
                {
                    COEExceptionDispatcher.HandleUIException(otherExcetion);
                    strRet = otherExcetion.Message.ToString();
                }
            }
            return strRet;
        }
        
        /// <summary>
        /// This method used to create the temporary record in registration with User preference/default value
        /// This method uses the same xml pattern used in the normal createtempregrecord method.
        /// </summary>
        /// <param name="xml">The custom-formatted XML.</param>
        /// <returns>tempid</returns>
        [WebMethod(false, Description = CREATE_TEMPORARY_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string CreateTempRegRecordWithUserPreference(string xml)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "ADD_COMPOUND_TEMP" });
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();

                try
                {
                    string newXml = RemoveContentBeforeProlog(xml);
                    oRegistryRecord.InitializeFromXml(newXml, true, false);
                    oRegistryRecord.BatchPrefixDefaultOverride(true);
                    oRegistryRecord.IsDirectReg = true;
                    oRegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.ELN;

                    ChemDrawWarningChecker theChemDrawWarningChecker = new ChemDrawWarningChecker();
                    if (!theChemDrawWarningChecker.CheckRedBoxWarnings(oRegistryRecord))
                    {
                        //Fetching the user preference values
                        COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(COEUser.Name, (int)FormGroups.UserPreference);
                        if (genericStorageBO != null && genericStorageBO.ID > 0)
                            oRegistryRecord.UpdateUserPreference(genericStorageBO.COEGenericObject);
                        RegistryRecord savedRegistryRecord = oRegistryRecord.Save();
                        //Fix for EN-7876,if Temp registration fails it is now returning a Reg record with ID=0 
                        if (savedRegistryRecord.ID == 0 && savedRegistryRecord.GetBrokenRulesDescription().Count > 0)
                            throw new ValidationException();
                        strRet = savedRegistryRecord.ID.ToString();
                    }
                    else
                    {
                        throw new ValidationException();
                    }
                }
                catch (Exception exception)
                {
                    if (exception.GetBaseException() is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                        strRet = FormatBrokenRulesMessage(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                    }
                    else
                    {
                        COEExceptionDispatcher.HandleUIException(exception);
                        strRet = exception.Message.ToString();
                    }
                }
            }
            catch (Exception otherExcetion)
            {
                if (otherExcetion is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = CREATE_TEMP_REGPRIVILEGE_MSG;
                else
                {
                    COEExceptionDispatcher.HandleUIException(otherExcetion);
                    strRet = otherExcetion.Message.ToString();
                }
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
        public string RetrieveTemporaryRegistryRecord(int id)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_TEMP" });

                RegistryRecord oRegistryRecord = RetrieveTemporaryRegistration(id);
                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
        public string UpdateTemporaryRegistryRecord(string xml)
        {
            string strRet = null;

            CheckCallerAuthentication();
            CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_TEMP" });

            string newXml = RemoveContentBeforeProlog(xml);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(newXml);
            XmlNode idNode = doc.SelectSingleNode(XPATH_REG_ID);
            int id = Convert.ToInt32(idNode.InnerText);

            RegistryRecord oRegistryRecord = RegistryRecord.GetRegistryRecord(id);

            try
            {
                oRegistryRecord.InitializeFromXml(newXml, false, false);
                oRegistryRecord.BatchPrefixDefaultOverride(true);
                RegistryRecord savedRegistryRecord = oRegistryRecord.Save();
                strRet = savedRegistryRecord.ID.ToString();
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ValidationException)
                {
                    List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                    ShowBrokenRules(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                }
                else
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                }
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
        public string DeleteTemporaryRegistryRecord(int id)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "DELETE_TEMP" });
                RegistryRecord.DeleteRegistryRecord(id);
                strRet = String.Format("Deleted temporary registration '{0}'", id);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
        public string CreateRegistryRecord(string xml, string duplicateAction)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "REGISTER_DIRECT" });

                DuplicateAction autoResolutionAction = TranslateDuplicateAutoResolutionInstruction(duplicateAction);
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();

                try
                {
                    string newXml = RemoveContentBeforeProlog(xml);
                    oRegistryRecord.InitializeFromXml(newXml, true, false);
                    oRegistryRecord.BatchPrefixDefaultOverride(true);
                    oRegistryRecord.ApproverForBulkLoading(true);
                    oRegistryRecord.IsDirectReg = true;
                    RegistryRecord savedReg = oRegistryRecord.Register(autoResolutionAction);

                    strRet = "<ReturnList>";
                    strRet += "<ActionDuplicateTaken>" + savedReg.ActionDuplicateTaken + "</ActionDuplicateTaken>";
                    strRet += "<RegID>" + savedReg.ID.ToString() + "</RegID>";
                    if (savedReg.RegNumber.RegNum != "")
                        strRet += "<RegNum>" + savedReg.RegNumber.RegNum + "</RegNum>";
                    strRet += "<BatchNumber>" + savedReg.BatchList.Count + "</BatchNumber>";
                    strRet += "<BatchID>" + savedReg.BatchList[savedReg.BatchList.Count - 1].ID + "</BatchID>";
                    if(savedReg.RegisterCheckRedBoxWarning)
                        strRet += "<RedBoxWarning>" + savedReg.RedBoxWarning.ToString() + "</RedBoxWarning>";
                    if (string.IsNullOrEmpty(savedReg.RegNumber.RegNum) && savedReg.Status == RegistryStatus.NotSet)
                        strRet += "<ErrorMessage>" + savedReg.DalResponseMessage + "</ErrorMessage>";
                    strRet += "</ReturnList>";
                }
                catch (Exception exception)
                {
                    if (exception.GetBaseException() is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                        strRet = "<ReturnList><ErrorMessage>" + FormatBrokenRulesMessage(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList) + "</ErrorMessage></ReturnList>";
                    }
                    else
                    {
                        COEExceptionDispatcher.HandleUIException(exception);
                        strRet = "<ReturnList><ErrorMessage>" + exception.Message.ToString() + "</ErrorMessage></ReturnList>";
                    }
                }
            }
            catch (Exception otherExcetion)
            {
                if (otherExcetion is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = "<ReturnList><ErrorMessage>" + CREATE_PERM_REGPRIVILEGE_MSG + "</ErrorMessage></ReturnList>";
                else
                {
                    COEExceptionDispatcher.HandleUIException(otherExcetion);
                    strRet = otherExcetion.Message.ToString();
                }
            }
            return strRet;
        }

        /// <summary>
        /// This method used to register records in bulk. and the format of input xml is list of multicompoundregisteryrecord
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
        public string CreateRegistryRecordBulk(string xml, string duplicateAction)
        {
            string strRet = null;

            CheckCallerAuthentication();
            CheckCallerAuthorizations(new string[] { "REGISTER_DIRECT" });

            DuplicateAction autoResolutionAction = TranslateDuplicateAutoResolutionInstruction(duplicateAction);

            try
            {
                XmlDocument xmlCompleteList = new XmlDocument();
                xmlCompleteList.LoadXml(xml);
                XmlNodeList xmlCompoundList = xmlCompleteList.SelectNodes("MultiCompoundRegistryRecordList/MultiCompoundRegistryRecord");
                RegistryRecord oRegistryRecordOriginal = RegistryRecord.NewRegistryRecord();
                RegistrationLoadList regLoadList = RegistrationLoadList.NewRegistrationLoadList();
                if (xmlCompoundList != null)
                {
                    foreach (XmlNode XmlToSend in xmlCompoundList)
                    {
                        string newXml = RemoveContentBeforeProlog(XmlToSend.OuterXml);
                        RegistryRecord oRegistryRecord = oRegistryRecordOriginal.Clone();
                        oRegistryRecord.InitializeFromXml(newXml, true, false);
                        oRegistryRecord.BatchPrefixDefaultOverride(true);
                        oRegistryRecord.ApproverForBulkLoading(true);
                        regLoadList.AddRegistration(oRegistryRecord);
                    }
                }
                List<RegRecordSummaryInfo> loadResult = regLoadList.Register(autoResolutionAction);              
                strRet = "<ReturnList>";
                int i = 1;
                foreach (RegRecordSummaryInfo regSummary in loadResult)
                {

                    strRet += "<ReturnListItem itemIndex=\"" + i.ToString() + "\">";
                    strRet += "<ActionDuplicateTaken>" + regSummary.Action + "</ActionDuplicateTaken>";
                    strRet += "<RegID>" + regSummary.RegId + "</RegID>";
                    strRet += "<RegNum>" + regSummary.RegNum + "</RegNum>";
                    strRet += "<BatchNumber>" + regSummary.BatchCount.ToString() + "</BatchNumber>";
                    if(regSummary.IsRedBoxWarningExists)
                        strRet += "<RedBoxWarning>" + regSummary.RedBoxWarningMessage.ToString() + "</RedBoxWarning>";
                    strRet += "</ReturnListItem>";
                    i++;
                }
                strRet += "</ReturnList>";
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ValidationException)
                {
                    //List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                    //ShowBrokenRules(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                }
                else
                {
                    COEExceptionDispatcher.HandleUIException(exception);
                }
            }
            return strRet;
        }

        /// <summary>
        /// This method will register the record in Registration with user preference data
        /// It work almost similar to CreateRegistryRecord but additionaly sync the user preference data to the current record.
        /// </summary>
        /// <param name="xml">input xml in the registry record format</param>
        /// <param name="duplicateAction">duplicate action to be taken</param>
        /// <returns></returns>
        [WebMethod(false, Description = CREATE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string CreateRegRecordWithUserPreference(string xml, string duplicateAction)
        {
            string strRet = null;
            try
                {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "REGISTER_DIRECT" });

                DuplicateAction autoResolutionAction = TranslateDuplicateAutoResolutionInstruction(duplicateAction);
                RegistryRecord oRegistryRecord = RegistryRecord.NewRegistryRecord();
                try
                {
                    string newXml = RemoveContentBeforeProlog(xml);
                    oRegistryRecord.InitializeFromXml(newXml, true, false);

                    oRegistryRecord.ModuleName = ChemDrawWarningChecker.ModuleName.ELN;
                    ChemDrawWarningChecker theChemDrawWarningChecker = new ChemDrawWarningChecker();
                    if (!theChemDrawWarningChecker.CheckRedBoxWarnings(oRegistryRecord))
                    {
                        oRegistryRecord.BatchPrefixDefaultOverride(true);
                        oRegistryRecord.ApproverForBulkLoading(true);
                        oRegistryRecord.IsDirectReg = true;

                        //Fetching the user preference values
                        COEGenericObjectStorageBO genericStorageBO = COEGenericObjectStorageBO.Get(COEUser.Name, (int)FormGroups.UserPreference);
                        if (genericStorageBO != null && genericStorageBO.ID > 0)
                            oRegistryRecord.UpdateUserPreference(genericStorageBO.COEGenericObject);

                        RegistryRecord savedReg = oRegistryRecord.Register(autoResolutionAction);

                        strRet = "<ReturnList>";
                        strRet += "<ActionDuplicateTaken>" + savedReg.ActionDuplicateTaken + "</ActionDuplicateTaken>";
                        strRet += "<RegID>" + savedReg.ID.ToString() + "</RegID>";
                        if (savedReg.RegNumber.RegNum != "")
                            strRet += "<RegNum>" + savedReg.RegNumber.RegNum + "</RegNum>";
                        strRet += "<BatchNumber>" + savedReg.BatchList.Count + "</BatchNumber>";
                        strRet += "<BatchID>" + savedReg.BatchList[savedReg.BatchList.Count - 1].ID + "</BatchID>";
                        if (string.IsNullOrEmpty(savedReg.RegNumber.RegNum) && savedReg.Status == RegistryStatus.NotSet)
                            strRet += "<ErrorMessage>" + savedReg.DalResponseMessage + "</ErrorMessage>";
                        strRet += "</ReturnList>";
                    }
                    else
                    {
                        throw new ValidationException();
                    }
                }
                catch (Exception exception)
                {
                    if (exception.GetBaseException() is ValidationException)
                    {
                        List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                        strRet = "<ReturnList><ErrorMessage>" + FormatBrokenRulesMessage(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList) + "</ErrorMessage></ReturnList>";
                    }
                    else
                    {
                        COEExceptionDispatcher.HandleUIException(exception);
                        strRet = "<ReturnList><ErrorMessage>" + exception.Message.ToString() + "</ErrorMessage></ReturnList>";
                    }
                }
            }
            catch (Exception otherExcetion)
            {
                if (otherExcetion is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = "<ReturnList><ErrorMessage>" + CREATE_PERM_REGPRIVILEGE_MSG + "</ErrorMessage></ReturnList>";
                else
                {
                    COEExceptionDispatcher.HandleUIException(otherExcetion);
                    strRet = otherExcetion.Message.ToString();
                }
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
        /// <param name="batchId">The unique identifier of the permanent batch.</param>
        /// <param name="excludeOtherBatches">
        /// The caller can exclude information about batches if desired
        /// </param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = RETRIEVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string RetrieveRegistryRecordByBatchID(int batchId, bool excludeOtherBatches)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                RegistryRecord oRegistryRecord = RetrievePermanentRegistrationByBatch(batchId);

                if (excludeOtherBatches == true)
                {
                    oRegistryRecord.BatchList.RaiseListChangedEvents = false;
                    Batch desiredBatch = oRegistryRecord.BatchList.GetBatchById(batchId);
                    if (desiredBatch != null)
                    {
                        oRegistryRecord.BatchList.Clear();
                        oRegistryRecord.BatchList.Add(desiredBatch);
                    }
                    oRegistryRecord.BatchList.RaiseListChangedEvents = true;
                }

                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// Retrieves the XML string representation of a single registration record using the Full Registrtion Number.  
        /// This record can be a full record including all batches or the registry record and the specific batch requested.
        /// <para>
        /// NOTE: The term 'permanent' does not indicate that the registration cannot be deleted,
        /// although not all users will have the permission to do so.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The Full Registration Number is  an alphanumeric value, derived using a combination
        /// of a prefix code (ex. 'AB') with an automated numeric sequence and the batch number. 
        /// Depending on configuration additional information such as the salts codes for the 
        /// batch may also be part of the Full Registration Number.
        /// ex. AB-000001/01 {Prefix}{Separator}{Sequence}{Batch Separator}{Padded Batch Number}
        /// </para>
        /// </remarks>
        /// <param name="fullRegNumber">The unique identifier of the permanent batch .</param>
        /// <param name="excludeOtherBatches">
        /// The caller can exclude information about batches if desired
        /// </param>
        /// <returns>The custom-formatted XML representation of a registration object.</returns>
        [WebMethod(false, Description = RETRIEVE_REGISTRY_RECORD)]
        [SoapHeader(CREDENTIALS)]
        public string RetrieveRegistryRecordByFullRegNum(string fullRegNumber, bool excludeOtherBatches)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                RegistryRecord oRegistryRecord = RetrievePermanentRegistrationByBatchRegNum(fullRegNumber);

                if (excludeOtherBatches == true)
                {

                    oRegistryRecord.BatchList.RaiseListChangedEvents = false;
                    Batch desiredBatch = oRegistryRecord.BatchList.GetBatchFromFullRegNum(fullRegNumber);

                    if (desiredBatch != null)
                    {
                        oRegistryRecord.BatchList.Clear();
                        oRegistryRecord.BatchList.Add(desiredBatch);
                    }
                    oRegistryRecord.BatchList.RaiseListChangedEvents = true;
                }

                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
        public string RetrieveRegistryRecord(string regNum)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                RegistryRecord oRegistryRecord = RetrievePermanentRegistration(regNum);
                strRet = oRegistryRecord.XmlWithAddIns;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
        public string UpdateRegistryRecord(string xml)
        {
            //string strRet = null;
            //try
            //{
            //    CheckCallerAuthentication();
            //    CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

            //    string newXml = RemoveContentBeforeProlog(xml);
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(newXml);
            //    XmlNode regNode = doc.SelectSingleNode(XPATH_REG_NUM);
            //    string regNum = regNode.InnerText;
            //    RegistryRecord reg = RegistryRecord.GetRegistryRecord(regNum);
            //    reg.InitializeFromXml(newXml, false, false);
            //    RegistryRecord savedReg = reg.SaveFromCurrentXml();
            //    strRet = savedReg.RegNumber.RegNum;
            //}
            //catch (Exception ex)
            //{
            //    COEExceptionDispatcher.HandleBLLException(ex);
            //}
            //return strRet;

            string strRet = null;
            string errorMessage = null;


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
            string lockRecord = "Record is locked in Registration and not able to update, please unlock the record to modify.";
            try
            {
                if (oRegistryRecord == null)
                    throw new Exception(errorMessage);
                if (oRegistryRecord.Status == RegistryStatus.Locked)
                    throw new Exception(lockRecord);
                oRegistryRecord.UpdateFromXml(xml);
                oRegistryRecord.BatchPrefixDefaultOverride(true);
                oRegistryRecord.Save(DuplicateCheck.CompoundCheck);

                //errorMessage = "Unable to InitializeFromXml.";
                //oRegistryRecord.InitializeFromXml(xml, false, false);

                //errorMessage = "Unable to Save.";
                //RegistryRecord savedRegistryRecord = oRegistryRecord.SaveFromCurrentXml();
                //if ((savedRegistryRecord == null) || (savedRegistryRecord.IsValid == false))
                //    strRet = BuildAndThrowSoapException(errorMessage);

                strRet = oRegistryRecord.RegNumber.RegNum;

            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ValidationException)
                {
                    List<BrokenRuleDescription> brokenRuleDescriptionList = oRegistryRecord.GetBrokenRulesDescription();
                    ShowBrokenRules(((ValidationException)exception.GetBaseException()), brokenRuleDescriptionList);
                }
                else
                {
                    strRet = exception.Message;//setting the exception to display in calling place.
                    COEExceptionDispatcher.HandleUIException(exception);
                }
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                strRet = GetTempIDCommand.Execute();
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                string newXml = RemoveContentBeforeProlog(xml);
                DuplicateCheck checkingMechanism = TranslateDuplicateCheckingInstruction(duplicateCheck);

                RegistryRecord reg = RegistryRecord.NewRegistryRecord();
                reg.InitializeFromXml(newXml, true, true);
                strRet = reg.CheckUniqueRegistryRecord(checkingMechanism);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                RegistryRecord reg = RegistryRecord.NewRegistryRecord();
                strRet = reg.GetRegisteredInfoFromTempBatchID(tempBatchID);
            }
            catch (Exception ex)
            {
                if (ex is System.Security.AccessControl.PrivilegeNotHeldException)
                    strRet = SEARCH_REGWITH_NOPRIVILEGE_MSG;
                else
                    COEExceptionDispatcher.HandleBLLException(ex);
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
        public string RetrievePicklist(string strCriteria)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();

                Picklist pl = null;
                PicklistDomain pld = null;
                int nCriteria;
                string domainName = string.Empty;
                string domainId = string.Empty;

                // Assume SQL - Should check against harmful inner expressions
                if (strCriteria.ToUpper().StartsWith("SELECT "))
                    pl = Picklist.GetPicklist(strCriteria);

                // Assume PicklistDomain ID
                if (pl == null)
                {
                    Int32.TryParse(strCriteria, out nCriteria);
                    if (nCriteria > 0)
                    {
                        pld = PicklistDomain.GetPicklistDomain(nCriteria);
                        if (pld != null)
                            pl = Picklist.GetPicklist(pld);
                    }
                }

                // Assume PicklistDomain DESCRIPTION
                if (pl == null)
                {
                    pld = PicklistDomain.GetPicklistDomain(strCriteria);
                    pl = Picklist.GetPicklist(pld);
                }

                if (pld != null)
                {
                    domainName = pld.Description;
                    domainId = pld.Identifier.ToString();
                }

                //JED: provide additional information using root node attributes
                strRet = string.Format(
                    "<Picklist id=\"{0}\" name=\"{1}\" count=\"{2}\">",
                    domainId, domainName, pl.PickList.Count.ToString()
                );

                foreach (KeyValuePair<int, string> kvp in pl.PickList)
                {
                    strRet += "<PicklistItem ID=\"" + kvp.Key.ToString() + "\">" + kvp.Value + "</PicklistItem>";
                }
                strRet += "</Picklist>";
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "SEARCH_REG" });

                COESearch oCOESearch = new COESearch(4007);
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
                ResultPageInfo oResultPageInfo = new ResultPageInfo();
                DataResult oDataResult = oCOESearch.DoSearch(oSearchInput, resultFields, oResultPageInfo);
                if (oDataResult.Status == "SUCCESS")
                    strRet = oDataResult.ResultSet;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();

                string[] strRoles = vstrRoles.Split(',');
                foreach (string strRole in strRoles)
                {
                    if (!string.IsNullOrEmpty(strRet))
                        strRet += ",";
                    strRet += Csla.ApplicationContext.User.IsInRole(strRole).ToString();
                }
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
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
            string strRet = sourceXml;
            try
            {
                CheckCallerAuthentication();

                strRet = Utilities.FormatXmlString(strRet);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
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
            string strRet = null;
            try
            {
                CheckCallerAuthentication();

                string xml = RemoveContentBeforeProlog(registryRecordXml);
                RegistryRecord reg = RegistryRecord.NewRegistryRecord();
                reg.InitializeFromXml(xml, false, false);
                reg.UpdateXml();
                strRet = Utilities.FormatXmlString(reg.Xml);
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// Gets a Registration setting
        /// </summary>
        /// <param name="key">Settings Name</param>
        /// <returns>Found settting or empty</returns>
        [WebMethod(false, Description = GET_REGISTRATION_SETTING)]
        [SoapHeader(CREDENTIALS)]
        public string GetRegistrySetting(string key)
        {
            return RegUtilities.GetConfigSetting(RegUtilities.Groups.RegAdmin, key);
        }

        /// <summary>
        /// Add a new batch to specific registry record based on the incoming batch xml.
        /// </summary>
        /// <param name="regNum">the id of the specific registry record</param>
        /// <param name="batchXml">the xml which represents the new batch info</param>
        /// <returns>Error info</returns>
        [WebMethod(false, Description = ADD_BATCH)]
        [SoapHeader(CREDENTIALS)]
        public string AddBatch(string regNum, string batchXml)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                Batch newBatch = Batch.NewBatch(batchXml, true, true);
                record.BatchList.Add(newBatch);
                record.BatchPrefixDefaultOverride(true);
                record.Save();
                strRet = "Succeed to add new batch.";
            }
            catch (Exception ex)
            {
                strRet = "Fail to add new batch. Error messages:<br />" +
                    ex.Message + "<br />" +
                    "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// Add a new component to specific registry record based on the incoming batch xml.
        /// </summary>
        /// <param name="regNum">the id of the specific registry record</param>
        /// <param name="compXml">the xml which represents the new component info</param>
        /// <returns>Error info</returns>
        [WebMethod(false, Description = ADD_COMPONENET)]
        [SoapHeader(CREDENTIALS)]
        public string AddComponent(string regNum, string compXml)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                CambridgeSoft.COE.Registration.Services.Types.Component component =
                    CambridgeSoft.COE.Registration.Services.Types.Component.NewComponent(compXml, true, true);
                record.ComponentList.Add(component);
                record.Save();
                strRet = "Succeed to add new component.";
            }
            catch (Exception ex)
            {
                strRet = "Fail to add new component. Error messages:<br />" +
                    ex.Message + "<br />" +
                    "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// Deletes a batch from a specific registry record.
        /// </summary>
        /// <param name="regNum">the id of the specific registry record</param>
        /// <param name="batchId">the id of the specfic batch</param>
        /// <returns>Error info</returns>
        [WebMethod(false, Description = DELETE_BATCH)]
        [SoapHeader(CREDENTIALS)]
        public string DeleteBatch(string regNum, int batchId)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                Batch matchedBatch = null;
                foreach (Batch batch in record.BatchList)
                {
                    if (batch.ID == batchId)
                        matchedBatch = batch;
                }
                if (matchedBatch != null)
                {
                    Batch.DeleteBatch(batchId);
                    record.Save();
                    strRet = "Succeed to delete batch.";
                }
                else
                {
                    strRet = "Fail to delete batch. Couldn't find a matched batch based on incoming batch id";
                }
            }
            catch (Exception ex)
            {
                strRet = "Fail to delete batch. Error messages:<br />" +
                    ex.Message + "<br />" +
                    "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }

        [WebMethod(false, Description = UPDATE_BATCH)]
        [SoapHeader(CREDENTIALS)]
        public string UpdateBatch(string regNum, string batchXml)
        {
            string strRet = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(batchXml);

                string fullRegNumber = doc.SelectSingleNode("//Batch/FullRegNumber").InnerText;
                Batch batchToUpdate = null;
                foreach (Batch batch in record.BatchList)
                {
                    if (batch.FullRegNumber == fullRegNumber)
                    {
                        batchToUpdate = batch;
                        break;
                    }
                }

                if (batchToUpdate == null)
                    throw new ArgumentException("Can't find the Batch with FullRegNumber " + fullRegNumber);

                batchToUpdate.UpdateFromXml(doc.DocumentElement);
                record.BatchPrefixDefaultOverride(true);
                record.Save();

                strRet = "Succeed to update batch.";
            }
            catch (Exception ex)
            {
                strRet = "Fail to update batch. Error messages:<br />" +
                             ex.Message + "<br />" +
                             "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }

            return strRet;
        }

        /// <summary>
        /// Deletes a component from a specific registry record.
        /// </summary>
        /// <param name="regNum">the id of the specific registry record</param>
        /// <param name="componentId">the id of the specfic component</param>
        /// <returns>Error info</returns>
        [WebMethod(false, Description = DELETE_COMPONENT)]
        [SoapHeader(CREDENTIALS)]
        public string DeleteComponent(string regNum, int componentIndex)
        {
            string strRet = null;
            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                CambridgeSoft.COE.Registration.Services.Types.Component matchedComponent = null;
                foreach (CambridgeSoft.COE.Registration.Services.Types.Component component in record.ComponentList)
                {
                    if (component.ComponentIndex == componentIndex)
                        matchedComponent = component;
                }
                if (matchedComponent != null)
                {
                    record.ComponentList.Remove(matchedComponent);
                    record.Save();
                    strRet = "Succeed to delete component.";
                }
                else
                {
                    strRet = "Fail to delete component. Couldn't find a matched component based on incoming batch id";
                }
            }
            catch (Exception ex)
            {
                strRet = "Fail to delete component. Error messages:<br />" +
                    ex.Message + "<br />" +
                    "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }
            return strRet;
        }
        //on the condition that the incoming compXml argument contains all the Structure data, 
        //along with all Structure’s property list
        [WebMethod(false, Description = UPDATE_COMPONENT)]
        [SoapHeader(CREDENTIALS)]
        public string UpdateComponent(string regNum, string componentXml)
        {
            string strRet = null;

            try
            {
                CheckCallerAuthentication();
                CheckCallerAuthorizations(new string[] { "EDIT_COMPOUND_REG" });

                RegistryRecord record = RegistryRecord.GetRegistryRecord(regNum);
                List<int> componentIndexesFromDB = new List<int>();
                foreach (CambridgeSoft.COE.Registration.Services.Types.Component component in record.ComponentList)
                {
                    componentIndexesFromDB.Add(component.ComponentIndex);
                }
                //Update Structure propertylist
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(componentXml);
                XmlNodeList components = doc.SelectNodes("//ComponentList/Component");
                foreach (XmlNode componentNode in components)
                {
                    XmlNode componentIndexNode = componentNode.SelectSingleNode("//Component/ComponentIndex");
                    int componentIndex = int.Parse(componentIndexNode.InnerText);
                    if (componentIndexesFromDB.Contains(componentIndex))
                    {
                        //get the propertylist node
                        XmlNode propertyListNode = componentNode.SelectSingleNode("//Component/Compound/BaseFragment/Structure/PropertyList");
                        foreach (CambridgeSoft.COE.Registration.Services.Types.Component component in record.ComponentList)
                        {
                            if (component.ComponentIndex == componentIndex)
                            {
                                foreach (Property p in component.Compound.BaseFragment.Structure.PropertyList)
                                {
                                    XmlNode matchingChild =
                                        propertyListNode.SelectSingleNode(string.Format("Property[@name='{0}']", p.Name));

                                    if (matchingChild == null)
                                        component.Compound.BaseFragment.Structure.PropertyList.Remove(p);
                                    else
                                        p.Value = matchingChild.InnerText;
                                }

                            }
                        }
                    }
                }
                strRet = "Succeed to update a component.";
            }
            catch (Exception ex)
            {
                strRet = "Fail to update component. Error messages:<br />" +
                             ex.Message + "<br />" +
                             "Your can find more error information from event log.";
                COEExceptionDispatcher.HandleBLLException(ex);
            }

            return strRet;
        }

        [WebMethod(false, Description = GET_PROTOTYPE_XML)]
        [SoapHeader(CREDENTIALS)]
        public string GetPrototypeXml(string objectType)
        {
            PrototypeRegistryXml prototype = PrototypeRegistryXml.GetPrototypeRegistryXml(null);
            string fullPrototypeXml = prototype.Prototype;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fullPrototypeXml);

            switch (objectType.ToUpper())
            {
                case "MIXTURE":
                    return fullPrototypeXml;
                case "BATCH":
                    return doc.SelectSingleNode("//MultiCompoundRegistryRecord/BatchList/Batch").OuterXml;
                    break;
                case "COMPONENT":
                    return doc.SelectSingleNode("//MultiCompoundRegistryRecord/ComponentList/Component").OuterXml;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("objectType", "The specified object type is not valid");
            }

            return null;
        }

        #region >Private methods<

        static COELog _coeLog = COELog.GetSingleton(Registration.Services.Common.Constants.SERVICENAME);

        /// <summary>
        /// Strips a UTF8 or UTF16 byte-order mark from the incoming string.
        /// </summary>
        /// <param name="stringToClean">the string to strip any leading byte-order mark from</param>
        /// <returns>the argument string with any UTF8 or UTF16 preamble removed</returns>
        private string RemoveContentBeforeProlog(string stringToClean)
        {
            if (string.IsNullOrEmpty(stringToClean))
                return stringToClean;

            //enable fall-through for case where no BOM is found
            string bomFree = stringToClean;

            //eliminate any BOM as well as any other unexpected characters
            //forces explicit encoding declaration for xml strings, otherwise assumes UTF-8
            int index = stringToClean.IndexOf("<");
            if (index > 0)
                bomFree = stringToClean.Substring(index, stringToClean.Length - index);

            return bomFree;
        }

        /// <summary>
        /// Performs a COEPrincipal.Login() and fetches authentication information.
        /// </summary>
        private void CheckCallerAuthentication()
        {
            bool isAuthenticated = false;

            if (string.IsNullOrEmpty(Credentials.UserName) && string.IsNullOrEmpty(Credentials.AuthenticationTicket))
            {
                // No credentials have been provided...apply 'testing' credentials (even if they don't exist)
                Credentials.UserName = ConfigurationManager.AppSettings["TestingUserName"];
                Credentials.Password = ConfigurationManager.AppSettings["TestingPassword"];
            }

            if (string.IsNullOrEmpty(Credentials.UserName))
                isAuthenticated = COEPrincipal.Login(Credentials.AuthenticationTicket, true);
            else
                isAuthenticated = COEPrincipal.Login(Credentials.UserName, Credentials.Password);
        }

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
        /// <param name="registryXml"></param>
        /// <returns></returns>
        private RegistryRecord LoadRegistryRecordFromXml(string registryXml)
        {
            RegistryRecord reg = RegistryRecord.NewRegistryRecord();
            reg.InitializeFromXml(registryXml, true, false);
            return reg;
        }

        /// <summary>
        /// Internal method used by both RetrieveRegistryRecord and RetrieveRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="batchId">The unique identifier of a batch of the permanent registration.</param>
        private RegistryRecord RetrievePermanentRegistrationByBatch(int batchId)
        {
            RegistryRecord reg = RegistryRecord.GetRegistryRecordByBatch(batchId);
            return reg;
        }

        /// <summary>
        /// Internal method used by both RetrieveRegistryRecord and RetrieveRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="fullRegNumber">The unique identifier as the Full Registration Number (instead of batch id) of a batch of the permanent registration.</param>
        private RegistryRecord RetrievePermanentRegistrationByBatchRegNum(string fullRegNumber)
        {
            RegistryRecord reg = RegistryRecord.GetRegistryRecordByBatch(fullRegNumber);
            return reg;
        }


        /// <summary>
        /// Internal method used by both RetrieveRegistryRecord and RetrieveRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="regNum">The unique identifier of the permanent registration.</param>
        private RegistryRecord RetrievePermanentRegistration(string regNum)
        {
            RegistryRecord reg = RegistryRecord.GetRegistryRecord(regNum);
            return reg;
        }

        /// <summary>
        /// Internal method used by both RetrieveTemporaryRegistryRecord and RetrieveTemporaryRegistryRecordUsingMime methods
        /// to fetch the underlying RegistryRecord object.
        /// </summary>
        /// <param name="id">The unique identifier of the temporary registration.</param>
        private RegistryRecord RetrieveTemporaryRegistration(int id)
        {
            RegistryRecord reg = RegistryRecord.GetRegistryRecord(id);
            return reg;
        }

        /// <summary>
        /// Translates incoming string-based definitions for RegistryRecord.DuplicateCheck
        /// enumeration members;
        /// </summary>
        /// <param name="duplicateCheck"></param>
        /// <returns>A matched enumeration member of Registryrecord.DuplicateCheck</returns>
        private DuplicateCheck
            TranslateDuplicateCheckingInstruction(string duplicateCheck)
        {
            DuplicateCheck checkingMechanism = DuplicateCheck.None;
            string msg =
                "Invalid duplicate checking mechanism requested. Please use 'C'(ompound), 'M'(ixture), or 'N'(one)";
            switch (duplicateCheck.ToUpper())
            {
                case "COMPOUND":
                case "COMPOUNDCHECK":
                case "C": { checkingMechanism = DuplicateCheck.CompoundCheck; break; }
                case "MIXTURE":
                case "MIXCHECK":
                case "M": { checkingMechanism = DuplicateCheck.MixCheck; break; }
                case "NONE":
                case "N": { checkingMechanism = DuplicateCheck.None; break; }
                default: { throw new InvalidCastException(msg); }
            }
            return checkingMechanism;
        }

        /// <summary>
        /// Translates incoming string-based definitions for RegistryRecord.DuplicateAction
        /// enumeration members;
        /// </summary>
        /// <param name="duplicateAction"></param>
        /// <returns>A matched enumeration member of Registryrecord.DuplicateAction</returns>
        private DuplicateAction
            TranslateDuplicateAutoResolutionInstruction(string duplicateAction)
        {
            DuplicateAction autoResolutionAction = DuplicateAction.None;
            string msg =
                "Invalid duplicate-resolution action requested. Please use 'B'(atch), 'D'(uplciate), 'T'(emporary), or 'N'(one)";
            switch (duplicateAction.ToUpper())
            {
                case "BATCH":
                case "B": { autoResolutionAction = DuplicateAction.Batch; break; }
                case "DUPLICATE":
                case "D": { autoResolutionAction = DuplicateAction.Duplicate; break; }
                case "NONE":
                case "N": { autoResolutionAction = DuplicateAction.None; break; }
                case "TEMPORARY":
                case "T": { autoResolutionAction = DuplicateAction.Temporary; break; }
                default: { throw new InvalidCastException(msg); }
            }
            return autoResolutionAction;
        }

        /// <summary>
        /// Validate input xml against the RegistryRecord schema.
        /// </summary>
        /// <param name="registryXml">xml representation of RegistryRecord to be validated</param>
        /// <returns>an error message with schema-validation errors</returns>
        [WebMethod(false, Description = "Validate Registration XML")]
        public string ValidateRegistryRecordXml(string registryXml)
        {
            StringBuilder validationError = new StringBuilder();
            string[] resourceNames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string schemaResourceName = null;
            string errorMessage = null;
            foreach (string resource in resourceNames)
            {
                if (resource.Contains("RegistryRecord.xsd"))
                {
                    schemaResourceName = resource;
                    break;
                }
            }
            Stream schemaStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            XmlSchema schema = XmlSchema.Read(schemaStream, null);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e)//collect schema validaiton error message
            {
                XmlSchemaException exception = e.Exception;
                string message = string.Format(Resources.Resource.SchemaValidationError, exception.LineNumber, exception.LinePosition, exception.Message);
                validationError.AppendLine(message);
            };
            using (StringReader regRecordReader = new StringReader(registryXml))
            {
                using (XmlReader reader = XmlReader.Create(regRecordReader, settings))
                {
                    try
                    {
                        while (reader.Read()) ;
                    }
                    catch (Exception e)//other kinds of error
                    {
                        validationError.AppendLine(e.Message);
                    }
                    finally
                    {
                        errorMessage = validationError.ToString();
                    }
                }
            }
            return errorMessage;
        }

        /// <summary>
        /// Displays broken rules  for which register record was failed to save.
        /// </summary>
        private void ShowBrokenRules(ValidationException exception, List<BrokenRuleDescription> brokenRules)
        {
            List<string> sbPropertyList = new List<string>(); 
            
            string propListFormatter = ".";
            string headerMesgFormatter = ":";
            string newLineFormatter = "<br/>";
            string propCounterFormatter = ") ";
            string brokenRulesMessage = string.Empty;
            string propRule = string.Empty;
            string headerMesg = "The requested registration task could not be completed due to the following validation errors";
            int iCounter = 1;

            headerMesg = string.IsNullOrEmpty(exception.Message) ? headerMesg : string.Format("{0}, {1}", exception.Message, headerMesg);

            foreach (BrokenRuleDescription brokenRule in brokenRules)
            {
                foreach (string rule in brokenRule.BrokenRulesMessages)
                {
                    propRule = Convert.ToString(string.Format(@"{0}{1}{2}{3}", iCounter, propCounterFormatter, rule, newLineFormatter));
                    sbPropertyList.Add(propRule);
                    iCounter += 1;
                }
            }

            brokenRulesMessage = headerMesg + headerMesgFormatter +
                                 newLineFormatter +
                                 string.Join(propListFormatter, sbPropertyList.ToArray());

            throw new Exception(brokenRulesMessage);
        }

        /// <summary>
        /// method returns the formatted broken rule message for which register record was failed to save.
        /// </summary>
        private string  FormatBrokenRulesMessage(ValidationException exception, List<BrokenRuleDescription> brokenRules)
        {
            List<string> sbPropertyList = new List<string>();
            string propListFormatter = ".";
            string propCounterFormatter = ") ";
            string brokenRulesMessage = string.Empty;
            string propRule = string.Empty;
            int iCounter = 1;


            foreach (BrokenRuleDescription brokenRule in brokenRules)
            {
                foreach (string rule in brokenRule.BrokenRulesMessages)
                {
                    propRule = Convert.ToString(string.Format(@"{0}{1}{2}", iCounter, propCounterFormatter, rule ));
                    sbPropertyList.Add(propRule);
                    iCounter += 1;
                }
            }

            brokenRulesMessage = string.Join(propListFormatter, sbPropertyList.ToArray());
            if (string.IsNullOrEmpty(brokenRulesMessage))
                brokenRulesMessage = brokenRulesMessage + exception.Message;

            return brokenRulesMessage;
        }
        #endregion

    }
}
