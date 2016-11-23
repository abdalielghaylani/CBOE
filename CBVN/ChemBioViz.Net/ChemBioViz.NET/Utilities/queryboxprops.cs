using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.ComponentModel;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;

using ChemBioViz.NET;
using CBVUtilities;
using Utilities;
using ChemControls;

namespace CBVControls
{
    public class CBVQueryBoxProps
    {
        #region Variables
        private SearchCriteria.COEOperators m_operator;
        private bool m_bAllowListInput;
        private String m_nativeUnits;
        private String m_tooltipText;
        private String m_aggreg;
        #endregion

        #region Properties
        public SearchCriteria.COEOperators Operator
        {
            get { return m_operator; }
            set { m_operator = value; }
        }
        //---------------------------------------------------------------------
        public bool AllowListInput
        {
            get { return m_bAllowListInput; }
            set { m_bAllowListInput = value; }
        }
        //---------------------------------------------------------------------
        public String TooltipText
        {
            get { return m_tooltipText; }
            set { m_tooltipText = value; }
        }
        //---------------------------------------------------------------------
        public String Aggregate
        {
            get { return m_aggreg; }
            set { m_aggreg = value; }
        }
        //---------------------------------------------------------------------
        public String Units
        {
            get { return m_nativeUnits; }
            set
            {
                String sUnits = value;
                if (CBVUtil.StartsWith(sUnits, "["))
                {
                    int endBrack = sUnits.IndexOf("]");
                    if (endBrack > 0)
                        sUnits = sUnits.Substring(endBrack + 3);    // skip ] and : and space
                }
                m_nativeUnits = sUnits;
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public CBVQueryBoxProps()
        {
            m_operator = SearchCriteria.COEOperators.EQUAL;
            m_bAllowListInput = false;
            m_nativeUnits = String.Empty;
            m_tooltipText = String.Empty;
            m_aggreg = String.Empty;
        }
        //---------------------------------------------------------------------
        public CBVQueryBoxProps(CBVQueryTextBox queryBox)
        {
            BoxToProps(queryBox);
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Empty;  // prevent showing class name in props grid
        }
        //---------------------------------------------------------------------
        public void BoxToProps(CBVQueryTextBox qbox)
        {
            m_operator = qbox.Operator;
            m_bAllowListInput = qbox.AllowListInput;
            m_nativeUnits = qbox.Units;
            m_tooltipText = qbox.TooltipText;
        }
        //---------------------------------------------------------------------
        public void PropsToBox(CBVQueryTextBox qbox)
        {
            qbox.Operator = m_operator;
            qbox.AllowListInput = m_bAllowListInput = qbox.AllowListInput;
            qbox.Units = m_nativeUnits;
            qbox.TooltipText = m_tooltipText;
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVQueryBoxPropsEditor : UITypeEditor
    {
        #region Constructors
        public CBVQueryBoxPropsEditor()
        {
        }
        #endregion

        #region Methods
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        //---------------------------------------------------------------------
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context,
                                        System.IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(CBVQueryBoxProps))
                return value;

            CBVQueryTextBox queryBox = context.Instance as CBVQueryTextBox;
            CBVQueryBoxProps props = value as CBVQueryBoxProps;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                QueryTextPropsDialog dialog = new QueryTextPropsDialog(queryBox);
                DialogResult result = edSvc.ShowDialog(dialog);
                if (result == DialogResult.OK)
                    value = queryBox.QueryBoxProps;
            }
            return value;
        }
        #endregion
    }
}
