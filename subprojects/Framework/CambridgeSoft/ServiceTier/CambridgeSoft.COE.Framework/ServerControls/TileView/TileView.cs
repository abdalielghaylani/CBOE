using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

namespace CambridgeSoft.COE.Framework.ServerControls.TileView
{
    public class TileView : System.Web.UI.WebControls.CompositeControl, ICOEHitMarker
    {
        #region Variables
        private DataList dList;
        private XmlDocument document;
        object dataSource;
        #endregion

        #region Properties
        public object DataSource
        {
            get
            {
                object o = dataSource;
                return (o == null) ? (object) this.Page.Session["DataSource"] : (object) o;
            }
            set
            {
                dataSource = value;
                dList.DataSource = value;
                this.Page.Session.Add("DataSource", value);
            }
        }
        #endregion

        #region Constructors
        public TileView()
        {
            document = new XmlDocument();
            dList = new DataList();
        }
        #endregion

        #region Overriden (Life Cycle) Events
        protected override void CreateChildControls()
        {
            Controls.Clear();
            Controls.Add(dList);
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
            manager.AddNamespace("COE", document.DocumentElement.NamespaceURI);

            XmlNode headerForm = document.SelectSingleNode("//COE:headerTemplate/COE:coeForm", manager);
            XmlNode itemForm = document.SelectSingleNode("//COE:itemTemplate/COE:coeForm", manager);
            XmlNode footerForm = document.SelectSingleNode("//COE:footerTemplate/COE:coeForm", manager);

            if(headerForm != null)
                dList.HeaderTemplate = new FormGeneratorTemplate(FormGroup.Form.GetForm(headerForm.OuterXml), this.DataSource, FormGenerator_MarkingHit);
            if(itemForm != null)
                dList.ItemTemplate = new FormGeneratorTemplate(FormGroup.Form.GetForm(itemForm.OuterXml), this.DataSource, FormGenerator_MarkingHit);
            if(footerForm != null)
                dList.FooterTemplate = new FormGeneratorTemplate(FormGroup.Form.GetForm(footerForm.OuterXml), this.DataSource, FormGenerator_MarkingHit);

            dList.RepeatColumns = 3;
            dList.RepeatLayout = RepeatLayout.Table;
            dList.ItemDataBound += new DataListItemEventHandler(dList_ItemDataBound);
            ChildControlsCreated = true;
        }

        void dList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ((COEFormGenerator) e.Item.Controls[0]).PageIndex = e.Item.ItemIndex;
            ((COEFormGenerator) e.Item.Controls[0]).DataBind();
            foreach(Control ctrlDiv in ((COEFormGenerator) e.Item.Controls[0]).Controls[0].Controls)
            {
                if(ctrlDiv.Controls[0] is ICOEDisplayModeChanger)
                {
                    ((ICOEDisplayModeChanger) ctrlDiv.Controls[0]).CurrentIndex = e.Item.ItemIndex;
                }
            }
        }

        void FormGenerator_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if(MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            dList.DataBind();
        }
        #endregion

        #region Business Methods
        public void LoadFromXml(string xmlDataAsString)
        {
            // <HeaderTemplate>
            //     <coeForm />
            // </HeaderTemplate>
            // <ItemTemplate>
            //     <coeForm />
            // </ItemTemplate>
            // <FooterTemplate>
            //     <coeForm />
            // </FooterTemplate>
            document = new XmlDocument();
            document.LoadXml(xmlDataAsString);
        }
        #endregion

        #region ICOEHitMarker Members

        public event MarkingHitHandler MarkingHit;

        public string ColumnIDValue
        {
            get
            {
                return string.Empty;
            }
            set { }
        }

        public string ColumnIDBindingExpression
        {
            get { return string.Empty; }
            set { }
        }

        #endregion
    }

    public class FormGeneratorTemplate : ITemplate
    {
        FormGroup.Form _formData;
        object _dataSource;
        private COEFormGenerator _formGenerator;
        private MarkingHitHandler _markingHit;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        public object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
            }
        }

        public FormGeneratorTemplate(FormGroup.Form formData, object dataSource, MarkingHitHandler markingHit)
        {
            _formData = formData;
            _dataSource = dataSource;
            _markingHit = markingHit;
        }

        public void InstantiateIn(Control control)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _formGenerator = new COEFormGenerator(_formData);
            control.Controls.Add(_formGenerator);
            _formGenerator.DisplayMode = FormGroup.DisplayMode.View;
            _formGenerator.DataSource = _dataSource;
            if(_markingHit != null)
                _formGenerator.DataBinding += new EventHandler(_formGenerator_DataBinding);
            _coeLog.LogEnd(methodSignature);
        }

        void _formGenerator_DataBinding(object sender, EventArgs e)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            if(_formGenerator.Controls.Count > 0)
            {
                foreach(Control control in _formGenerator.Controls[0].Controls)
                {
                    if(control.Controls[0] is ICOEHitMarker)
                        ((ICOEHitMarker) control.Controls[0]).MarkingHit += _markingHit;
                }
            }
            _coeLog.LogEnd(methodSignature);
        }
    }
}

