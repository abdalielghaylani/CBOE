using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;

using ChemBioViz.NET;
using FormDBLib;
using CBVUtilities;
using CBVControls;

namespace ChemControls
{
    [Designer(typeof(BindableControlDesigner))]
    public class CBVQueryTextBox : TextBox
    {
        #region Variables
        private ToolTip m_toolTip;
        private CBVQueryBoxProps m_queryBoxProps;
        #endregion

        #region Constructors
        public CBVQueryTextBox()
        {
            m_queryBoxProps = new CBVQueryBoxProps();
            m_toolTip = new ToolTip();
        }
        #endregion

        #region Properties
        public String BoundField
        {
            get {
                String s = String.Empty;
                if (this.Tag != null)
                    s = CBVUtil.AfterDelimiter(this.Tag as String, '.');

                else if (this.DataBindings != null && this.DataBindings.Count > 0 &&
                        this.DataBindings[0].BindingMemberInfo != null)
                    s = this.DataBindings[0].BindingMemberInfo.BindingMember;   // NO GOOD for subform

                return s;
            }
        }
        //---------------------------------------------------------------------
        public COEDataView.AbstractTypes BoundFieldType
        {
            get {
                if (this.Parent != null && !String.IsNullOrEmpty(BoundField))
                {
                    FormViewControl fvc = this.Parent.Parent.Parent.Parent as FormViewControl;
                    if (fvc == null && this.Parent is FormViewControl)  // added 1/11
                        fvc = this.Parent as FormViewControl;

                    if (fvc != null)
                    {
                        FormDbMgr formDbMgr = fvc.Form.FormDbMgr;
                        String boundField = BoundField;
                        COEDataView.DataViewTable t = (Query.IsSubformField(boundField))?
                            Query.GetSubformTable(formDbMgr, ref boundField): formDbMgr.SelectedDataViewTable;
                        if (t != null)
                        {
                            COEDataView.Field dvFld = FormDbMgr.FindDVFieldByName(t, boundField);
                            if (dvFld != null)
                            {
                                bool bIsStructure = dvFld.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE;
                                return dvFld.DataType;  // this brings back Text for structure field
                            }
                        }
                    }
                }
                return COEDataView.AbstractTypes.Text;
            }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(UnitsStringConverter))]
        public String Units
        {
            get { return m_queryBoxProps.Units; }
            set { m_queryBoxProps.Units = value; }
        }
        //---------------------------------------------------------------------
        public SearchCriteria.COEOperators Operator
        {
            get { return m_queryBoxProps.Operator; }
            set { m_queryBoxProps.Operator = value; }
        }
        //---------------------------------------------------------------------
        public bool AllowListInput
        {
            get { return m_queryBoxProps.AllowListInput; }
            set { m_queryBoxProps.AllowListInput = value; }
        }
        //---------------------------------------------------------------------
        public String TooltipText
        {
            get { return m_queryBoxProps.TooltipText; }
            set { m_queryBoxProps.TooltipText = value; }
        }
        //---------------------------------------------------------------------
        [TypeConverterAttribute(typeof(CBVTextBox.StdAggregConverter))]
        public String Aggregate
        {
            get { return m_queryBoxProps.Aggregate; }
            set { m_queryBoxProps.Aggregate = value; }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Events
        //---------------------------------------------------------------------
        protected override void OnMouseHover(EventArgs e)
        {
            String msg = String.Empty;
            if (ChemBioViz.NET.Properties.Settings.Default.ShowTooltips && !String.IsNullOrEmpty(TooltipText))
            {
                msg = TooltipText;
                msg = CBVUtil.ReplaceCRs(msg);
            }
            m_toolTip.SetToolTip(this, msg);
            base.OnMouseHover(e);
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(true)]
        [DisplayNameAttribute("(Advanced)")]
        [EditorAttribute(typeof(CBVQueryBoxPropsEditor), typeof(UITypeEditor))]
        public CBVQueryBoxProps QueryBoxProps
        {
            get { return new CBVQueryBoxProps(this); }
            set
            {
                CBVQueryBoxProps props = value as CBVQueryBoxProps;
                props.PropsToBox(this);
            }
        }
        #endregion
    }
    //---------------------------------------------------------------------
}
