using System;
using System.Xml;
using System.Collections.Generic;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEPickListPickerService;

using Csla;
using Csla.Data;
using CambridgeSoft.COE.Registration.Core.Properties;

namespace CambridgeSoft.COE.Registration
{
    /// <summary>
    /// Allows the generation of name/value lists for the purposes of generating drop-downs for UI
    /// as well as for validating data (like a Foriegn Key reference).
    /// </summary>
    [Serializable()]
    public class Picklist : RegistrationBusinessBase<Picklist>
    {
        private int _includeValue = -1;
        private Dictionary<int, string> _pickList = new Dictionary<int, string>();
        /// <summary>
        /// The integer-based dictionary of value-member/display-member pairs embodied
        /// by any given Picklist object.
        /// </summary>
        public Dictionary<int, string> PickList
        {
            get
            {
                return _pickList;
            }
            protected set
            {
                _pickList = value;
                return;
            }
        }

        /// <summary>
        /// Include value in picklist list, communicates with DB if include value is available and Is Active = false inserts in to picklist Dictionary.
        /// </summary>
        public int IncludeValue
        {
            set
            {
                _includeValue = value;
                if (!this.PickList.ContainsKey(_includeValue))
                  UpdatePicklist(_includeValue);
                return;
            }
        }

        /// <summary>
        /// Given a display-member, return the matching value-member (the KeyValuePair.Key).
        /// </summary>
        /// <remarks>
        /// The 'ContainsValue' mechanism is not sued in order to provide case-insensitive values.
        /// </remarks>
        /// <param name="value">the string equivalent of the item's Key</param>
        /// <returns>-1 if no such display-member was found, otherwise a positive integer</returns>
        public int GetListItemIdByValue(string value)
        {
            int id = -1;
            foreach (KeyValuePair<int, string> kvp in PickList)
            {
                if (string.Equals(kvp.Value, value, StringComparison.OrdinalIgnoreCase))
                {
                    id = kvp.Key;
                    break;
                }
            }
            return id;
        }

        /// <summary>
        /// Given a value-member, return the matching display-member (the KeyValuePair.Value).
        /// </summary>
        /// <param name="value">the item's Key (which *might* not exist)</param>
        /// <returns>an emmty string if there is no matching picklist item ID</returns>
        public string GetListItemValueById(int value)
        {
            string display = string.Empty;
            if (this.PickList.ContainsKey(value))
                display = this.PickList[value];
            return display;
        }

        #region Factory methods

        /// <summary>
        /// Create a new Picklist instance from the given xml fragment
        /// </summary>
        /// <param name="xml">the xml representation of a Picklist</param>
        /// <returns>The Picklist instance represented by the given xml fragment</returns>
        [COEUserActionDescription("NewPicklist")]
        public static Picklist NewPicklist(string xml)
        {
            try
            {
                Picklist picklist = new Picklist();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.SelectSingleNode("Picklist");
                picklist.ID = Convert.ToInt32(root.Attributes["id"].Value);
                int itemsCount = int.Parse(root.Attributes["count"].Value);
                XmlNodeList picklistItems = doc.SelectNodes("Picklist/PicklistItem");
                foreach (XmlNode item in picklistItems)
                {
                    int id;
                    if (Int32.TryParse(item.Attributes["ID"].Value, out id))
                    {
                        picklist.PickList.Add(id, item.InnerText);
                    }
                }
                if (itemsCount != picklist.PickList.Count)
                {
                    throw new COEBusinessLayerException(Resources.Picklist_InvalidXmlInput);
                }
                return picklist;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Constructs a pick-list given a SQL statement. This mechanism should be considered legacy.
        /// </summary>
        /// <param name="vSql">the actual sql statement used to fetch name/value pairs</param>
        /// <returns></returns>
        [COEUserActionDescription("GetPicklist")]
        public static Picklist GetPicklist(string vSql)
        {
            Picklist picklist = new Picklist();
            PickListNameValueList nameValueList = null;

            try
            {
                nameValueList = PickListNameValueList.GetPickListNameValueList(vSql);
                picklist = Picklist.ConvertFromPickListNameValueList(0, nameValueList);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
            return picklist;
        }

        /// <summary>
        /// Adds the picklist id if available in DB, Mostly inactive pickilist id is included.
        /// </summary>
        /// <param name="includeValue">Picklist id to be validated and added in picklist list</param>
        /// <returns></returns>
        [COEUserActionDescription("GetPicklist")]
        private void UpdatePicklist(int includeValue)
        {
            PickListNameValueList nameValueList = null;
            try
            {
                nameValueList = PickListNameValueList.GetPickListNameValueList(this.ID, true, includeValue);
                foreach (Csla.NameValueListBase<string, string>.NameValuePair pair in nameValueList)
                {
                    if (string.Equals(pair.Key, Convert.ToString(includeValue), StringComparison.OrdinalIgnoreCase))
                    {
                        this.PickList.Add(Convert.ToInt32(pair.Key), pair.Value);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }

        }

        /// <summary>
        /// Constructs a pick-list given a specific picklist domain.
        /// </summary>
        /// <param name="vPicklistDomain">the object representing the picklist domain information</param>
        /// <returns>an initialized instance of this object</returns>
        [COEUserActionDescription("GetPicklist")]
        public static Picklist GetPicklist(PicklistDomain vPicklistDomain)
        {
            Picklist picklist = new Picklist();
            PickListNameValueList nameValueList = null;

            try
            {
                if (!string.IsNullOrEmpty(vPicklistDomain.PickListDomainSql))
                    nameValueList = PickListNameValueList.GetPickListNameValueList(vPicklistDomain.PickListDomainSql);
                else
                    nameValueList = PickListNameValueList.GetPickListNameValueList(Constants.REGDB, vPicklistDomain.Identifier);
                picklist = Picklist.ConvertFromPickListNameValueList(vPicklistDomain.Identifier, nameValueList);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }

            return picklist;
        }

        /// <summary>
        /// Constructs a picklist given a specific picklist description.
        /// </summary>
        /// <param name="description">the description of the picklist domain</param>
        /// <returns>an initialized instance of this object</returns>
        [COEUserActionDescription("GetPicklist")]
        public static Picklist GetPicklistByDescription(string description)
        {
            Picklist picklist = new Picklist();
            PickListNameValueList nameValueList = null;

            try
            {
                PicklistDomain pickListDomain = PicklistDomain.GetPicklistDomain(description);
                nameValueList = PickListNameValueList.GetPickListNameValueList(Constants.REGDB, pickListDomain.Identifier);
                picklist = Picklist.ConvertFromPickListNameValueList(pickListDomain.Identifier, nameValueList);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
            return picklist;
        }

        private static Picklist ConvertFromPickListNameValueList(int picklistIdentifier, PickListNameValueList nameValueList)
        {
            Picklist pl = new Picklist();
            if (nameValueList != null)
            {
                pl.ID = picklistIdentifier;
                foreach (Csla.NameValueListBase<string, string>.NameValuePair pair in nameValueList)
                    pl.PickList.Add(Convert.ToInt32(pair.Key), pair.Value);
            }
            return pl;
        }

        #endregion
    }
}
