using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Csla;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// The BaseFragment class contains the Structure class.
    /// </summary>
    /// <remarks>
    /// Aside from containing the Structure, there isn't much reason for this class to exist.
    /// The Component could contain a BaseFragment class which itself derives from Structure.
    /// </remarks>
    [Serializable()]
    public class BaseFragment : BusinessBase<BaseFragment>
    {
        #region [ Factory Methods ]

        [COEUserActionDescription("NewBaseFragment")]
        public static BaseFragment NewBaseFragment(string xml, bool isNew, bool isClean)
        {
            try
            {
                BaseFragment baseFragment = new BaseFragment(xml, isNew, isClean);
                baseFragment.ValidationRules.CheckRules();

                return baseFragment;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private BaseFragment(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("BaseFragment/Structure");
            xIterator.MoveNext();

            this._structure = Structure.NewStructure(xIterator.Current.OuterXml, isNew, isClean);

            if (isClean)
                MarkClean();

            if (!isNew)
                MarkOld();
        }

        #endregion

        #region [ Authorization and Validation Rules ]

        public static bool CanAddObject() { return true; }
        public static bool CanGetObject() { return true; }

        #endregion

        #region [ Properties and members ]

        private int _id;
        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
        }

        private Structure _structure;
        public Structure Structure
        {
            get
            {
                return _structure;
            }
            set
            {
                CanWriteProperty(true);
                if (_structure != value)
                {
                    _structure = value;
                    PropertyHasChanged();
                }
            }
        }

        public override bool IsValid
        {
            get { return base.IsValid && _structure.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty || _structure.IsDirty; }
        }

        #endregion

        [COEUserActionDescription("GetBaseFragmentBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
                }

                this._structure.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        /// <summary>
        /// Csla requirement for the current version of that framework.
        /// </summary>
        /// <returns>the local ID value</returns>
        protected override object GetIdValue() { return _id; }

        /// <summary>
        /// Replaces the structure 
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="isNew"></param>
        public void ReplaceStructure(Structure structure, bool isNew)
        {
            this.Structure = structure;
            if (!isNew)
                this.Structure.MarkLinked();
        }

        /// <summary>
        /// Custom xml-based 'serialization' of this object's current state
        /// </summary>
        /// <param name="addCRUDattributes">true to record state-changes</param>
        /// <returns>an xml string representing this object and its children</returns>
        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");


            builder.Append("<BaseFragment>");
            builder.Append("<ID>" + this._id + "</ID>");
            if(_structure != null)
                builder.Append(this._structure.UpdateSelf(addCRUDattributes));

            builder.Append("</BaseFragment>");

            return builder.ToString();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            XmlNode structureNode = parentNode.SelectSingleNode("//BaseFragment/Structure");
            this.Structure.UpdateFromXml(structureNode);
        }
        /// <summary>
        /// updateuserpreference method for basefragment class
        /// </summary>
        /// <param name="parentNode"></param>
        internal void UpdateUserPreference(XmlNode parentNode)
        {
            XmlNode structureNode = parentNode.SelectSingleNode("//BaseFragment/Structure");
            this.Structure.UpdateUserPreference(structureNode);
        }

    }
}
