using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// The top-level abstraction for a batch of a registry record, allowing
    /// for re-use by multiple batches. Contains the batch-independent information
    /// describing one BaseFragment (a.k.a. ParentFragment).
    /// </summary>
    [Serializable()]
    public class Component : BusinessBase<Component>
    {

        #region [ Factory Methods ]

        [COEUserActionDescription("CreateNewComponent")]
        public static Component NewComponent()
        {
            try
            {
                using (RegistryRecord record = RegistryRecord.NewRegistryRecord())
                {
                    Component component = record.ComponentList[0];
                    component.MarkNew();
                    component.MarkClean();

                    return component;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateNewComponent")]
        public static Component NewComponent(bool isNew)
        {
            try
            {
                using (RegistryRecord record = RegistryRecord.NewRegistryRecord())
                {
                    Component component = record.ComponentList[0];

                    if (!isNew)
                        component.MarkOld();

                    component.MarkClean();

                    return component;
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateNewComponent")]
        public static Component NewComponent(string xml, bool isNew, bool isClean)
        {
            try
            {
                Component component = new Component(xml, isNew, isClean);

                if (isNew)
                    component.MarkNew();
                else
                    component.MarkOld();

                if (isClean)
                    component.MarkClean();
                else
                    component.MarkDirty();

                component.ValidationRules.CheckRules();
                return component;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private Component()
        {
            this.MarkAsChild();
        }

        private Component(string xml, bool isNew, bool isClean)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Component/ID");
            xIterator.MoveNext();
            int.TryParse(xIterator.Current.Value, out _id);

            xIterator = xNavigator.Select("Component/ComponentIndex");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    this._componentIndex = int.Parse(xIterator.Current.Value);

            xIterator = xNavigator.Select("Component/Compound");
            xIterator.MoveNext();
            _compound = Compound.NewCompound(xIterator.Current.OuterXml, isNew, isClean);
        }

        #endregion

        #region [ Authorization and Validation Rules ]

        //TODO: Determine the roles necessary for these logic-checks

        public static bool CanAddObject() { return true; }
        public static bool CanGetObject() { return true; }
        public static bool CanDeleteObject() { return true; }
        public static bool CanEditObject() { return true; }
        
        #endregion

        #region [ Properties and Fields ]

        private List<string> _changedProperties = new List<string>();

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

        private Compound _compound;
        public Compound Compound
        {
            get
            {
                CanReadProperty(true);
                return _compound;
            }
            set
            {
                CanWriteProperty(true);
                if (_compound != value)
                {
                    _compound = value;
                    PropertyHasChanged();
                }
            }
        }

        private int _componentIndex;
        public int ComponentIndex
        {
            get { return _componentIndex; }
            set
            {
                CanWriteProperty(true);
                if (_componentIndex != value)
                {
                    _componentIndex = value;
                }
            }
        }

        private double _percentage;
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                CanWriteProperty(true);
                if (_percentage != value)
                {
                    _percentage = value;
                    PropertyHasChanged();
                }
            }
        }

        public override bool IsValid
        {
            get { return base.IsValid && _compound.IsValid; }
        }

        public override bool IsDirty
        {
            get { return base.IsDirty || (this.Compound == null ? false : this.Compound.IsDirty); }
        }

        [NonSerialized()]
        public string InstanceXml;

        #endregion

        [COEUserActionDescription("GetComponentBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                if (this.BrokenRulesCollection != null && this.BrokenRulesCollection.Count > 0)
                {
                    brokenRules.Add(new BrokenRuleDescription(this, this.BrokenRulesCollection.ToArray()));
                }

                this._compound.GetBrokenRulesDescription(brokenRules);
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

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        /// <summary>
        /// Custom xml-based 'serialization' of this object's current state
        /// </summary>
        /// <param name="addCRUDattributes">true to record state-changes</param>
        /// <returns>an xml string representing this object and its children</returns>
        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            string state = string.Empty;
            if (addCRUDattributes && this.IsDeleted)
                state = "delete=\"yes\"";

            else if (addCRUDattributes && this.IsNew)
                state = "insert=\"yes\"";

            builder.AppendFormat("<Component {0}>", state);

            builder.Append("<ID>" + this._id + "</ID>");
            
            builder.Append("<ComponentIndex");
            if (addCRUDattributes && _changedProperties.Contains("ComponentIndex"))
                    builder.Append(" update=\"yes\"");

            builder.Append(">" + this._componentIndex + "</ComponentIndex>");

            builder.Append("<Percentage>" + this._id + "</Percentage>");

            builder.Append(this._compound.UpdateSelf(addCRUDattributes && !this.IsDeleted));

            builder.Append("</Component>");

            string sxml = builder.ToString();
            this.InstanceXml = sxml;
            return sxml;
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            //update Percentage
            XmlNode PercentageNode = parentNode.SelectSingleNode("//Component/Percentage");
            if (PercentageNode != null )
            {
                this.Percentage = double.Parse(PercentageNode.InnerText);
            }

            //Update compound
            XmlNode compoundNode = parentNode.SelectSingleNode("//Component/Compound");
            this.Compound.UpdateFromXml(compoundNode);
        }
        internal void UpdateUserPreference(XmlNode parentNode)
        {
            //update Percentage
            //XmlNode PercentageNode = parentNode.SelectSingleNode("//Component/Percentage");
            //if (PercentageNode != null )
            //{
            //    this.Percentage = double.Parse(PercentageNode.InnerText);
            //}

            //Update compound
            XmlNode compoundNode = parentNode.SelectSingleNode("//Component/Compound");
            this.Compound.UpdateUserPreference(compoundNode);
        }
    }
}
