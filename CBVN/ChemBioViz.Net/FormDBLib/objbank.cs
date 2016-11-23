using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;

using CBVUtilities;

namespace FormDBLib
{
    public class ObjectBank
    {
        #region Variables
        private COEGenericObjectStorageBOList m_stgList;
        #endregion

        #region Properties
        protected COEGenericObjectStorageBOList StgList
        {
            get { return m_stgList; }
            set { m_stgList = value; }
        }
        #endregion

        #region Constructors
        public ObjectBank()
        {
        }
        #endregion

        #region Methods
        public virtual void Store(String name, String xml) { }
        public virtual void Store(int id, String name, String xml) { }
        public virtual int StoreNew(String name, String xml) { return -1; }
        public virtual String Retrieve(String name) { return String.Empty; }
        public virtual String Retrieve(int id) { return String.Empty; }
        public virtual int RetrieveID(String name) { return -1; }
        public virtual void Delete(String name) { }
        public virtual void Delete(int id) { }
        public virtual List<String> GetNameList() { return new List<String>(); }
        public virtual Dictionary<int, string> GetIdValuePairList() { return new Dictionary<int,string>(); }
        //---------------------------------------------------------------------
        public bool HasName(String name)
        {
            foreach (String s in GetNameList())
                if (CBVUtil.Eqstrs(s, name))
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        protected int IndexOf(int id)
        {
            int index = -1;
            if (id > 0)
            {
                for (int i = 0; i < this.StgList.Count; i++)
                {
                    if (StgList[i].ID == id)
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }
        //---------------------------------------------------------------------
        public bool HasObject(int id)
        {
            bool exist = false;
            int index = this.IndexOf(id);
            if (index >= 0)
                exist = true;

            return exist;
        }
        #endregion
    }

    /// <summary>
    /// LOCAL object bank: stores a batch of files in a specified directory
    /// </summary>
    public class LocalObjectBank : ObjectBank
    {
        #region Variables
        private String m_dirPath;
        #endregion

        #region Properties
        public String DirPath
        {
            get { return m_dirPath; }
            set { m_dirPath = value; }
        }
        #endregion

        #region Constructors
        public LocalObjectBank(String dirPath)
        {
            m_dirPath = dirPath;
        }
        #endregion

        #region Methods
        public override void Store(String name, String xml)
        {
            String filename = String.Concat(m_dirPath, "\\", name, ".xml");		// ignore type for now
            using (StreamWriter swDS = new StreamWriter(filename, false, Encoding.Unicode))
                swDS.Write(xml);
        }
        //---------------------------------------------------------------------
        public override String Retrieve(String name)
        {
            String s = String.Empty, filename = String.Concat(m_dirPath, "\\", name, ".xml");
            if (File.Exists(filename))
            {
                //Coverity Bug Fix CID 13153 
                using (StreamReader reader = new StreamReader(filename))
                {
                    s = reader.ReadToEnd();
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public override List<String> GetNameList()
        {
            // enumerate xml files in the root directory
            List<String> names = new List<String>();
            DirectoryInfo di = new DirectoryInfo(m_dirPath);
            FileInfo[] fi = di.GetFiles();
            foreach (FileInfo fiTemp in fi)
            {
                if (CBVUtil.Eqstrs(fiTemp.Extension, ".xml"))
                    names.Add(fiTemp.Name.Substring(0, fiTemp.Name.Length - 4));
            }
            return names;
        }
        //---------------------------------------------------------------------
        #endregion
    }

    /// <summary>
    /// DB object bank: stores files in generic object storage on server
    /// </summary>
    public class DbObjectBank : ObjectBank
    {
        #region Variables
        protected String m_userName;
        protected bool m_bPublic;
        protected String m_description;
        protected List<String> m_names;
        protected static COEGenericObjectStorageBOList m_publicStgList = null;
        protected static COEGenericObjectStorageBOList m_privateStgList = null;
        public static string m_AssociatedDataviewID = string.Empty;
        public static Dictionary<int, string> m_dv_ID_name_pair = null;
        #endregion

        #region Properties
        protected String UserName
        {
            get { return m_userName; }
            set { m_userName = value; }
        }
        protected String Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
        protected List<String> Names
        {
            get { return m_names; }
            set { m_names = value; }
        }
        #endregion

        #region Constructors
        public DbObjectBank(String userName, bool bPublic)
        {
            // every db bank is identified by the three parameters:
            // public forms: username blank, bPublic true, description "CBVN Form" [= CBVConstants.COE_GENERIC_OBJECT_FORM]
            // private forms: username nonblank, bPublic false, "CBVN Form"
            // private settings: username nonblank, bPublic false, "CBVN Settings"
            // public styles: username blank, bPublic true, "CBVN Style"
            this.m_userName = userName;
            this.m_bPublic = bPublic;
            this.m_description = CBVConstants.COE_GENERIC_OBJECT_FORM;  // change this in subclass constructors
        }
        #endregion

        #region Methods

        /// <summary>
        /// Removes public form from list when User has no rights to access dataview from which the form is created
        /// </summary>
        public void RemovePublicFormsWhenNoAccess()
        {
            if (FormDbMgr.dataviews_Id_List != null)
            {
                List<COEGenericObjectStorageBO> tempPublicFormsList = new List<COEGenericObjectStorageBO>();
                
                foreach (COEGenericObjectStorageBO form in m_publicStgList)
                {                    
                    bool canBeAccessed = FormDbMgr.dataviews_Id_List.Contains(String.IsNullOrEmpty(form.AssociatedDataviewID) ? 0 : Convert.ToInt32(form.AssociatedDataviewID));

                    if (!canBeAccessed && !String.IsNullOrEmpty(form.AssociatedDataviewID))
                    {
                        tempPublicFormsList.Add(form);
                    }
                }

                foreach (COEGenericObjectStorageBO form in tempPublicFormsList)
                {
                    m_publicStgList.Remove(form);
                }
                
            }            
        }

        /// <summary>
        ///  Gets all the names of the objects which are in the COEGenericObjectStorageBOList
        /// </summary>
        /// <returns></returns>
        public override List<String> GetNameList()
        {
            // retrieve full pub/priv list and filter by description
            // all methods in this class are for use by all subclasses
            // get the complete list of public or private objects
            m_names = new List<String>();

            // retrieve public or private list on first call only
            if (m_bPublic)
            {
                if (m_publicStgList == null)
                    m_publicStgList = COEGenericObjectStorageBOList.GetList();

                // Remove public forms from the list which has no access rights to the user
                RemovePublicFormsWhenNoAccess();
                // note: previous call to GetList(bool bIsPublic, string excludeUser) retrieves full data, not just name list
                this.StgList = m_publicStgList;
            }
            else
            {
                if (m_privateStgList == null)
                    m_privateStgList = COEGenericObjectStorageBOList.GetList(m_userName, 0, false);
                // note: previous call to GetList(string userName, bool bIsPublic) gets list not matching but excluding userName (!)
                this.StgList = m_privateStgList;
            }

            // filter by required type (description = "CBVN Form," "CBVN Style," etc.
            foreach (COEGenericObjectStorageBO stgObj in StgList)
            {
                if (m_bPublic != stgObj.IsPublic)
                    continue;   // happens when we have both public and private forms under same user

                // check that USERNAME property matches what we asked for
                if (!m_bPublic && !CBVUtil.Eqstrs(stgObj.UserName, m_userName))
                    Debug.Assert(false);

                if (CBVUtil.Eqstrs(stgObj.Description, m_description) && stgObj.Name.Length != 0)
                    m_names.Add(stgObj.Name);
            }
            return m_names;
        }
        /// <summary>
        ///  Gets (ID,Value) pair of the stored objects on the DB
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, string> GetIdValuePairList()
        {
            Dictionary<int, string> gObjCollection = new Dictionary<int, string>();

            // retrieve public or private list on first call only
            if (m_bPublic)
            {
                if (m_publicStgList == null)
                    m_publicStgList = COEGenericObjectStorageBOList.GetList();

                // Remove public forms from the list which has no access rights to the user
                RemovePublicFormsWhenNoAccess();
                // note: previous call to GetList(bool bIsPublic, string excludeUser) retrieves full data, not just name list
                this.StgList = m_publicStgList;
            }
            else
            {
                if (m_privateStgList == null)
                    m_privateStgList = COEGenericObjectStorageBOList.GetList(m_userName, 0, false);
                // note: previous call to GetList(string userName, bool bIsPublic) gets list not matching but excluding userName (!)
                this.StgList = m_privateStgList;
            }

            // filter by required type (description = "CBVN Form," "CBVN Style," etc.
            foreach (COEGenericObjectStorageBO stgObj in StgList)
            {
                if (m_bPublic != stgObj.IsPublic)
                    continue;   // happens when we have both public and  nbprivate forms under same user

                // check that USERNAME property matches what we asked for
                if (!m_bPublic && !CBVUtil.Eqstrs(stgObj.UserName, m_userName))
                    Debug.Assert(false);

                if (CBVUtil.Eqstrs(stgObj.Description, m_description) && stgObj.Name.Length != 0)
                    gObjCollection.Add(stgObj.ID, stgObj.Name); 
            }
            return gObjCollection;
        }
        //---------------------------------------------------------------------
        public void Rename(String newName, int id)
        {
            COEGenericObjectStorageBO stgObj = COEGenericObjectStorageBO.Get(id);
            int index = this.IndexOf(id);
            if (stgObj != null)
            {
                stgObj.Name = newName;
                try
                {
                    StgList[index].Name = newName;
                    StgList[index].COEGenericObject = stgObj.COEGenericObject;
                    stgObj = stgObj.Save();
                    //Warning! Do not use this line because it causes exceptions when this atribute StgList[index].COEGenericObject has a value
                    //StgList[index] = stgObj.Save();
                }
                catch (Exception ex)
                {
                    throw new FormDBLib.Exceptions.ObjectBankException("Error renaming object", ex);
                }
            }
        }
        //---------------------------------------------------------------------
        public String GetObjectDataItem(String objName, String itemPath)
        {
            // retrieve attribute from stored xml; itemPath is like "//cbvnform/connection/@comments"
            int index = this.IndexOf(objName);
            if (index >= 0)
            {
                COEGenericObjectStorageBO stgObj = COEGenericObjectStorageBO.Get(StgList[index].ID);
                if (stgObj != null)
                {
                    String sXml = stgObj.COEGenericObject;
                    return CBVUtil.FindItemInXml(sXml, itemPath);
                }
            }
            return "";
        }
        //---------------------------------------------------------------------
        public void UpdateObjectDataItem(String objName, String nodePath, String itemName, 
                                        String newValue, String newName)
        {
            // modify stored name and/or comments embedded in xml
            // nodePath, itemName are like "//cbvnform/connection", "comments"
            int index = this.IndexOf(objName);
            if (index >= 0)
            {
                COEGenericObjectStorageBO stgObj = COEGenericObjectStorageBO.Get(StgList[index].ID);
                if (stgObj != null)
                {
                    if (!String.IsNullOrEmpty(newValue))
                    {
                        String sXml = stgObj.COEGenericObject;
                        String sNewXml = CBVUtil.ReplaceItemInXml(sXml, nodePath, itemName, newValue);
                        if (!String.IsNullOrEmpty(sNewXml)) 
                            stgObj.COEGenericObject = sNewXml;
                    }
                    if (!String.IsNullOrEmpty(newName))
                    {
                        stgObj.Name = newName;
                    }
                    try
                    {
                        StgList[index] = stgObj.Save();
                    }
                    catch (Exception e)
                    {
                        // this happens if you store updates to the same object more than once
                        // gets error "Invalid for root objects - use Delete instead"
                        // but it seems to be harmless, the changes do get stored
                        Debug.WriteLine("ERR STORING OBJ " + e.Message);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Save new object to DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="xml"></param>
        public override int StoreNew(String name, String xml)
        {
            int newID = -1;
            // create new
            try
            {
                COEGenericObjectStorageBO stgObj = COEGenericObjectStorageBO.New(CBVConstants.APP_NAME);
                stgObj.Name = name;
                stgObj.IsPublic = this.m_bPublic;
                stgObj.UserName = this.m_userName;
                stgObj.COEGenericObject = xml;
                stgObj.Description = this.m_description;
                stgObj.AssociatedDataviewID = DbObjectBank.m_AssociatedDataviewID;

                stgObj = stgObj.Save();
                newID = stgObj.ID;
                StgList.Add(stgObj);
            }
            catch (Exception ex)
            {
                throw new FormDBLib.Exceptions.ObjectBankException("Error adding object to the database", ex);
            }
            return newID;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Update an existing one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="xml"></param>
        public override void Store(int id, String name, String xml)
        {
            COEGenericObjectStorageBO stgObj = null;
            int index = this.IndexOf(id);
            if (index >= 0)
            {
                stgObj = COEGenericObjectStorageBO.Get(id);
                stgObj.COEGenericObject = xml;
                // update name; might have changed case
                stgObj.Name = name;
                try
                {
                    StgList[index] = stgObj.Save();
                }
                catch (Exception e)
                {
                    // see comment in UpdateObjectDataItem above
                    Debug.WriteLine("ERR STORING OBJ " + e.Message);
                }
            }
        }
        //---------------------------------------------------------------------
        public override void Store(String name, String xml)
        {
            // store a new object with the given data and name
            COEGenericObjectStorageBO stgObj = null;            
            int index = this.IndexOf(name);
            if (index >= 0)
            {
                //update an existent object
                stgObj = COEGenericObjectStorageBO.Get(StgList[index].ID);
                stgObj.COEGenericObject = xml;
                stgObj.Name = name;     // update name; might have changed case
                try
                {
                    StgList[index] = stgObj.Save();
                }
                catch (Exception e)
                {
                    // see comment in UpdateObjectDataItem above
                    Debug.WriteLine("ERR STORING OBJ " + e.Message);
                }
            }
            else
            {
                // create new
                stgObj = COEGenericObjectStorageBO.New(CBVConstants.APP_NAME);
                stgObj.Name = name;
                stgObj.IsPublic = this.m_bPublic;
                stgObj.UserName = this.m_userName;
                stgObj.COEGenericObject = xml;
                stgObj.Description = this.m_description;
                stgObj.AssociatedDataviewID = DbObjectBank.m_AssociatedDataviewID;
                try 
                {
                    StgList.Add(stgObj.Save());
                }
                catch (Exception e)
                {
                    // can get ORA-00001: unique constraint (COEDB.PK_CHEMINVDB2_COEGENERICOBJE) violated
                    Debug.WriteLine("ERR STORING NEW OBJ " + e.Message);
                }
            }
        }
        //---------------------------------------------------------------------
        protected COEGenericObjectStorageBO FindByName(String name)
        {
            if (this.StgList.Count > 0)
            {
                foreach (COEGenericObjectStorageBO stgObj in this.StgList)
                    if (CBVUtil.Eqstrs(stgObj.Name, name))
                        return stgObj;
            }
            return null;
        }
        //---------------------------------------------------------------------
        protected COEGenericObjectStorageBO FindByID(int id)
        {
            COEGenericObjectStorageBO stgObj = null;
            if (this.StgList.Count > 0)
            {
                foreach (COEGenericObjectStorageBO obj in this.StgList)
                {
                    if (obj.ID == id)
                    {
                        stgObj = obj;
                        break;
                    }
                }
            }
            return stgObj;
        }
        //---------------------------------------------------------------------
        // TO DO: return a collection of the all found instances with that name
        protected int IndexOf(String name)
        {
            for (int i = 0; i < this.StgList.Count; i++)
            {
                if (CBVUtil.Eqstrs(StgList[i].Name, name))
                    return i;
            }
            return -1;
        }
        //---------------------------------------------------------------------
        public override String Retrieve(String name)
        {
            COEGenericObjectStorageBO stgObj = FindByName(name);
            if (stgObj != null)
            {
                COEGenericObjectStorageBO stgBO = COEGenericObjectStorageBO.Get(stgObj.ID);
                String sXml = (stgBO == null) ? "" : stgBO.COEGenericObject.ToString();
                return sXml;
            }
            return null;
        }
        //---------------------------------------------------------------------
        public override int RetrieveID(String name)
        {
            int id = -1;
            try
            {
                COEGenericObjectStorageBO stgObj = FindByName(name);
                if (stgObj != null)
                    id = stgObj.ID;
            }
            catch (Exception ex)
            {
                throw new FormDBLib.Exceptions.ObjectBankException("Error retrieving object id", ex);
            }
            return id;
        }
        //---------------------------------------------------------------------
        public override String Retrieve(int id)
        {
            String sXml = string.Empty;
            try
            {
                COEGenericObjectStorageBO stgObj = null;
                if (id > 0)
                {
                    int index = this.IndexOf(id);
                    if (index >= 0)
                    {
                        stgObj = COEGenericObjectStorageBO.Get(id);
                        if (stgObj != null)
                            sXml = stgObj.COEGenericObject.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FormDBLib.Exceptions.ObjectBankException("Error retrieving object", ex);
            }
            return sXml;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Permanently removes the record with the given <paramref name="name"/> from the database 
        /// </summary>
        /// <param name="name">The name of the object</param>
        public override void Delete(String name)
        {
            int index = IndexOf(name);
            if (index >= 0)
            {
                COEGenericObjectStorageBO.Delete(StgList[index].ID);
                this.UpdateStgList();               
            }
        }
        //---------------------------------------------------------------------
        public override void Delete(int id)
        {    
            try
            {
                COEGenericObjectStorageBO stgObj = null;
                if (id > 0)
                {
                    int index = this.IndexOf(id);
                    if (index >= 0)
                    {
                        stgObj = COEGenericObjectStorageBO.Get(id);
                        if (stgObj != null)
                        {
                            COEGenericObjectStorageBO.Delete(id);
                            // StgList.RemoveAt(index) throws an error so rebuild the entire list
                            this.UpdateStgList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FormDBLib.Exceptions.ObjectBankException("Error deleting object", ex);
            }
        }
        //---------------------------------------------------------------------

        #region Private Methods

        private void UpdateStgList()
        {
            if (m_bPublic)
                m_publicStgList = null;
            else
                m_privateStgList = null;
            this.GetNameList();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Settings object bank: stores settings files in generic object storage on server
    /// </summary>
    public class SettingsObjectBank : DbObjectBank
    {
        #region Constructors
        public SettingsObjectBank(String userName)
            : base(userName, false)     // always private
        {
            m_description = CBVConstants.COE_GENERIC_OBJECT_SETTINGS;
            Names = GetNameList();
        }
        #endregion

    }
    //---------------------------------------------------------------------
    public class StylesObjectBank : DbObjectBank
    {
        #region Constructors
        public StylesObjectBank()
            : base(string.Empty, true)  // always public
        {
            m_description = CBVConstants.COE_GENERIC_OBJECT_STYLES;
            Names = GetNameList();
        }
        #endregion

    }
    //---------------------------------------------------------------------
}
