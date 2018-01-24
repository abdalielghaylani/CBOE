using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDisplayDataBrokerService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Common;

using Csla;
using Csla.Data;
using Csla.Validation;

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Provides support for fetching differently-scoped 'identifer' types
    /// defined at the registration mixture, component and batch level.
    /// </summary>
    [Serializable()]
    [TypeConverter(typeof(IdentifierListConverter))]
    public class IdentifierList :
        RegistrationBusinessListBase<IdentifierList, Identifier>, IKeyValueListHolder
    {
        private const string IDENTIFIERLIST_MANAGERNAME = "Default Cache Manager";
        private const string IDENTIFIERLIST_CACHEKEY = "IdentifiersByType.{0}";
        private const CacheItemPriority CACHE_PRIORITY = CacheItemPriority.Normal;
        private static SlidingTime CacheExpiry = new SlidingTime(TimeSpan.FromSeconds(60));

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="identifier">Identifier to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetIdentifierIndex")]
        public int GetIndex(Identifier identifier)
        {
            int retVal = 0;
            try
            {
                if(this.Contains(identifier))
                {
                    foreach(Identifier currentIdentifier in this)
                    {
                        retVal++;
                        if(currentIdentifier.UniqueID == identifier.UniqueID)
                            break;
                    }
                }
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return retVal;
            }
        }

        [COEUserActionDescription("CreateIdentifierList")]
        public static IdentifierList NewIdentifierList(string xml, bool isNew, bool isClean)
        {
            try
            {
                return new IdentifierList(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateIdentifierList")]
        public static IdentifierList NewIdentifierList()
        {
            try
            {
                return new IdentifierList();
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private IdentifierList()
        {
            //MarkAsChild();
        }

        private IdentifierList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("IdentifierList/Identifier");

            if(xIterator.MoveNext())
            {
                do
                {
                    this.Add(Identifier.NewIdentifier(xIterator.Current.OuterXml, isNew, isClean));
                } while(xIterator.Current.MoveToNext());
            }
        }

        [COEUserActionDescription("GetIdentifierList")]
        public static IdentifierList GetIdentifierList()
        {
            try
            {
                return DataPortal.Fetch<IdentifierList>(new Criteria());
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }
               
        /// <summary>
        /// Retrieves the list of available identifiers based on the 'type' or scope of their usage.
        /// </summary>
        /// <param name="type">a member of the IdentifierTypeEnum enumeration</param>
        /// <returns>an IdentifierList containing Identifiers of only the type specified</returns>
        [COEUserActionDescription("GetIdentifierListByType")]
        public static IdentifierList GetIdentifierListByType(IdentifierTypeEnum type)
        {
            try
            {
                return DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Helps you to fetch all the identifier lists ignoring identifier active inactive status. 
        /// </summary>
        /// <param name="type">a member of the IdentifierTypeEnum enumeration</param>
        /// <returns>a Csla data-reader</returns>
        [COEUserActionDescription("GetAllIdentifierListByType")]
        public static IdentifierList GetAllIdentifierListByType(IdentifierTypeEnum type)
        {
            try
            {
                return DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type, true));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the list of available identifiers based on the 'type' or scope of their usage.
        /// </summary>
        /// <param name="type">a member of the IdentifierTypeEnum enumeration</param>
        /// <param name="useCache">use false to explicitly force a refresh from the repository</param>
        /// <returns>an IdentifierList containing Identifiers of only the type specified</returns>
        [COEUserActionDescription("GetIdentifierListByType")]
        public static IdentifierList GetIdentifierListByType(IdentifierTypeEnum type, bool useCache)
        {
            IdentifierList identifiers = null;
            string cacheKey = string.Format(IDENTIFIERLIST_CACHEKEY, type.ToString());
            try
            {
                if (useCache)
                {
                    CacheManager manager = CacheFactory.GetCacheManager(IDENTIFIERLIST_MANAGERNAME);
                    identifiers = (IdentifierList)manager.GetData(cacheKey);
                    if (identifiers == null)
                    {
                        identifiers = DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type));
                        manager.Add(cacheKey, identifiers, CACHE_PRIORITY, null, CacheExpiry);
                    }

                }
                else
                    identifiers = DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }

            return identifiers;
        }

        /// <summary>
        /// Retrieves the list of available identifiers based on the 'type' or scope of their usage.
        /// Optionally includes the 'All' scope in the list returned.
        /// </summary>
        /// <param name="type">a member of the IdentifierTypeEnum enumeration</param>
        /// <param name="useCache">use false to explicitly force a refresh from the repository</param>
        /// <param name="includeSharedIdentifiers">
        /// set to true to merge the scoped list with the 'All' (unscoped) list, giving a potentially
        /// broader set of options
        /// </param>
        /// <returns>an IdentiferList containing identifiers of one or more types</returns>
        [COEUserActionDescription("GetIdentifierListByType")]
        public static IdentifierList GetIdentifierListByType(
            IdentifierTypeEnum type, bool useCache, bool includeSharedIdentifiers)
        {
            IdentifierList identifiers = null;
            string cacheKey = string.Format(IDENTIFIERLIST_CACHEKEY, type.ToString());
            try
            {
                if (useCache)
                {
                    CacheManager manager = CacheFactory.GetCacheManager(IDENTIFIERLIST_MANAGERNAME);
                    identifiers = (IdentifierList)manager.GetData(cacheKey);
                    if (identifiers == null)
                    {
                        identifiers = DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type));
                        manager.Add(cacheKey, identifiers, CACHE_PRIORITY, null, CacheExpiry);

                        //in this case, also include the 'all-scope' identifiers
                        IdentifierList sharedIdentifiers = DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria( IdentifierTypeEnum.A));
                        foreach(Identifier idf in sharedIdentifiers)
                        {
                            if (!identifiers.Contains(idf))
                                identifiers.Add(idf);
                        }

                    }
                }
                else
                    identifiers = DataPortal.Fetch<IdentifierList>(new IdentifierTypeCriteria(type));
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }

            return identifiers;
        }

        [COEUserActionDescription("GetIdentifierListByType")]
        public static IdentifierList GetIdentifierListByType(
            string type, bool useCache, bool includeSharedIdentifiers)
        {
            IdentifierList identifiers = null;
            try
            {
                IdentifierTypeEnum translatedType = (IdentifierTypeEnum)Enum.Parse(typeof(IdentifierTypeEnum), type);
                identifiers = GetIdentifierListByType(translatedType, useCache, includeSharedIdentifiers);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
            return identifiers;
        }


        [Serializable()]
        protected class Criteria
        {
            public Criteria() { }
        }

        [Serializable()]
        protected class IdentifierTypeCriteria
        {

            private bool _ignoreStatus = false;
            private IdentifierTypeEnum _type;
            public IdentifierTypeEnum Type { get { return _type; } }
            public bool IgnoreStatus { get { return _ignoreStatus; } }

            public IdentifierTypeCriteria(IdentifierTypeEnum type) { _type = type; }
            public IdentifierTypeCriteria(IdentifierTypeEnum type, bool ignoreStatus) { _type = type; _ignoreStatus = ignoreStatus; }
        }

        protected void DataPortal_Fetch(Criteria criteria)
        {
            using(SafeDataReader dr = this.RegDal.GetIdentifierList())
            {
                this.Fetch(dr);
            }
        }

        protected void DataPortal_Fetch(IdentifierTypeCriteria criteria)
        {
            using (SafeDataReader dr = this.RegDal.GetIdentifierListByType(Char.Parse(criteria.Type.ToString()), criteria.IgnoreStatus))
            {
                this.Fetch(dr);
            }
        }

        private void Fetch(SafeDataReader reader)
        {
            while(reader.Read())
            {
                this.Add(Identifier.NewIdentifier(0,
                                                reader.GetInt32("ID"),
                                                reader.GetString("NAME"),
                                                reader.GetString("DESCRIPTION"),
                                                reader.GetString("ACTIVE") == "T" ? true : false
                                                ));
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            bool showUpdateTag = false;

            foreach(Identifier currentIdentifier in this)
                if(currentIdentifier.IsDirty && !currentIdentifier.IsNew && !currentIdentifier.IsDeleted)
                    showUpdateTag = true;


            StringBuilder builder = new StringBuilder("");
            builder.Append(string.Format("<IdentifierList{0}>", (addCRUDattributes && this.IsDirty && showUpdateTag ? " update=\"yes\"" : string.Empty)));
            /*if(this.IsDirty)
                builder.Append(" update=\"yes\"");
            builder.Append(">");*/
            
            foreach(Identifier currentIdentifier in this)
                builder.Append(currentIdentifier.UpdateSelf(addCRUDattributes));

            foreach(Identifier currentDeletedIdentifier in this.DeletedList)
                //CSBR:143968 for the case where the record has never been committed to the database, but has identifiers that are
                //deleted such as loading a template, simply remove them from the xml
                if (!currentDeletedIdentifier.IsNew)
                {
                    builder.Append(currentDeletedIdentifier.UpdateSelf(true));
                }

            builder.Append("</IdentifierList>");
            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode incomingNode)
        {
            if (incomingNode != null)
            {
                XmlNodeList nodes = incomingNode.SelectNodes("Identifier");
                foreach (XmlNode node in nodes)
                {
                    Identifier identifier = Identifier.NewIdentifier(node.OuterXml, true, true);
                    //look at the unique id for the identifier entry and see if there is a match. If there is set the data to the new
                    //data otherwise assume it is new and add it
                    bool identifierFound = false;
                    foreach (Identifier idCurrent in this)
                    {
                        if (idCurrent.UniqueID == identifier.UniqueID && !string.IsNullOrEmpty(identifier.InputText))
                        {
                            idCurrent.InputText = identifier.InputText;
                            identifierFound = true;
                        }

                    }
                    if (!identifierFound)
                    {
                        this.Add(identifier);
                    }
                }
            }
        }

        internal void UpdateUserPreference(XmlNode incomingNode)
        {
            if (incomingNode != null)
            {
                XmlNodeList nodes = incomingNode.SelectNodes("Identifier");
                foreach (XmlNode node in nodes)
                {
                    Identifier identifier = Identifier.NewIdentifier(node.OuterXml, true, true);
                    bool identifierFound = false;
                    foreach (Identifier idCurrent in this)
                    {
                        if (idCurrent.IdentifierID == identifier.IdentifierID && !string.IsNullOrEmpty(identifier.InputText) && identifier.InputText == idCurrent.InputText) // Need to check IdentifierID and Identifier Text for matching
                        {
                           identifierFound = true;
                           break;
                        }

                    }
                    if (!identifierFound)
                    {
                        this.Add(identifier);
                    }
                }
            }
        }
        //public enum IdentifierTypeEnum 
        //{
        //    /// <summary>
        //    /// All project type
        //    /// </summary>
        //    A,
        //    /// <summary>
        //    /// Registry identifier type
        //    /// </summary>
        //    R,
        //    /// <summary>
        //    /// Compound identifier type
        //    /// </summary>
        //    C,
        //    /// <summary>
        //    /// Batch identifier type
        //    /// </summary>
        //    B,
        //    /// <summary>
        //    /// Structure identifier type
        //    /// </summary>
        //    S
        //}

        #region IKeyValueListHolder Members

        /// <summary>
        /// Brief list of identifiers
        /// </summary>
        public System.Collections.IDictionary KeyValueList
        {
            get
            {
                System.Collections.Specialized.HybridDictionary dictionary = new System.Collections.Specialized.HybridDictionary(this.Count);
                foreach (Identifier identifier in this)
                    dictionary.Add(identifier.IdentifierID, identifier.Name);
                return dictionary;
            }
        }

        #endregion

    }
}
