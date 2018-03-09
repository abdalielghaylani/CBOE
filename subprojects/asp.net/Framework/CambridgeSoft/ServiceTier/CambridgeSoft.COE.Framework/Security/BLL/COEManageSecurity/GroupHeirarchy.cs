using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Xml;

namespace CambridgeSoft.COE.Framework.COESecurityService
{

    [Serializable()]
    public class GroupHeirarchy
    {


        public static XmlDocument GetXML()
        {
            try
            {
                GetGroupHeirarchyXMLCommand result;
                result = DataPortal.Execute<GetGroupHeirarchyXMLCommand>(new GetGroupHeirarchyXMLCommand());
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result.HeirarchyXML);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static XmlDocument GetXML(int groupID)
        {
            try
            {
                GetGroupHeirarchyXMLCommand result;
                result = DataPortal.Execute<GetGroupHeirarchyXMLCommand>(new GetGroupHeirarchyXMLCommand(groupID));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result.HeirarchyXML);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        #region command objects


        [Serializable]
            private class GetGroupHeirarchyXMLCommand : CommandBase
            {
                private int _groupOrgID = -1;
               

                [NonSerialized]
                private DAL _coeDAL = null;
                [NonSerialized]
                private DALFactory _dalFactory = new DALFactory();
                private string _serviceName = "COESecurity";

                private string _heirarchyXML = string.Empty;

                public GetGroupHeirarchyXMLCommand(int groupOrgID)
                {
                    _groupOrgID = groupOrgID;
                  
                }

                public GetGroupHeirarchyXMLCommand()
                {
                   

                }

                public string HeirarchyXML
                {
                    get { return _heirarchyXML; }
                    set { _heirarchyXML = value; }
                }

                protected override void DataPortal_Execute()
                {
                    try
                    {
                        COEDatabaseName.Set(Resources.CentralizedStorageDB);
                        if (_coeDAL == null) { LoadDAL(); }
                        // Coverity Fix CID - 11633
                        if (_coeDAL != null)
                            HeirarchyXML = _coeDAL.GetGroupHeirarchyXML(_groupOrgID);
                        else
                            throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));                                     
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }


                private void LoadDAL()
                {
                    if (_dalFactory == null) { _dalFactory = new DALFactory(); }
                    _dalFactory.GetDAL<DAL>(ref _coeDAL, _serviceName, COEDatabaseName.Get().ToString(), true);
                }

               
            }
#endregion


    }
}
