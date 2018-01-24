using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml;
using System.Collections;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    class COEFormGeneratorRepeater : CompositeDataBoundControl, ICOEGenerableControl
    {
        #region variables
        private FormGroup.Form _formData;
        #endregion

        #region properties
        public FormGroup.Form FormData
        {
            get
            {
                return _formData;
            }
            set
            {
                _formData = value;

            }
        }
        #endregion

        protected WebControl AddItem(int itemIndex, ListItemType itemType)
        {
            if (itemType == ListItemType.AlternatingItem || itemType == ListItemType.Item)
            {
                COEFormGenerator formGenerator = new COEFormGenerator(_formData);
                formGenerator.ID = string.Format("{0}{1}_{2}", this.ID, _formData.Id, itemIndex);
                this.Controls.Add(formGenerator);
                formGenerator.DataSourceID = this.DataSourceID;
                formGenerator.DataMember = this.DataMember;
                formGenerator.DisplayMode = FormGroup.DisplayMode.View;
                formGenerator.PageIndex = itemIndex;

                return formGenerator;
            }
            return null;
        }

        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            int currentChildIndex = 0;
            IEnumerator enumerator = dataSource.GetEnumerator();
            while (enumerator.MoveNext())
            {
                WebControl currentControl = AddItem(currentChildIndex, ListItemType.Item);
                currentChildIndex++;
            }

            return currentChildIndex;
        }
        #region ICOEGenerableControl Members

        public new object GetData()
        {
            return null;
        }

        public void PutData(object data)
        {
            //this.DataSourceID = _formData.DataSourceId;
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            try
            {
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(xmlDataAsString);

                XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
                manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

                XmlNode form = xmlData.SelectSingleNode("//COE:fieldConfig/COE:coeForm", manager);
                // Coverity Fix CID - 13138
                if(form != null)
                    _formData = FormGroup.Form.GetForm(form.OuterXml);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public string DefaultValue
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        #endregion
    }
}
