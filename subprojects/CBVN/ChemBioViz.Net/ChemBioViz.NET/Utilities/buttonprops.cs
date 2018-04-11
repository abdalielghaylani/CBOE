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

using ChemBioViz.NET;
using CBVUtilities;
using Utilities;

namespace CBVControls
{
    public class CBVButtonProps
    {
        #region Variables
        private CBVButton.ActionType m_actionType;
        private String m_actionArgs;
        private String m_displayLabel;
        private String m_tooltipText;
        #endregion

        #region Properties
        public CBVButton.ActionType ActionType
        {
            get { return m_actionType; }
            set { m_actionType = value; }
        }
        public String ActionArgs
        {
            get { return m_actionArgs; }
            set { m_actionArgs = value; }
        }
        public String DisplayLabel
        {
            get { return m_displayLabel; }
            set { m_displayLabel = value; }
        }
        public String TooltipText
        {
            get { return m_tooltipText; }
            set { m_tooltipText = value; }
        }
        #endregion

        #region Constructors
        public CBVButtonProps(CBVButton button)
        {
            m_actionType = button.Action;
            m_actionArgs = button.Arguments;
            m_displayLabel = button.DisplayString;
            m_tooltipText = button.TooltipText;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Empty;  // prevent showing class name in props grid
        }
        #endregion
    }
    //---------------------------------------------------------------------
    public class CBVButtonPropsEditor : UITypeEditor
    {
        #region Constructors
        public CBVButtonPropsEditor()
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
            if (value.GetType() != typeof(CBVButtonProps))
                return value;

            CBVButton button = context.Instance as CBVButton;
            CBVButtonProps props = value as CBVButtonProps;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                ButtonPropsDialog dialog = new ButtonPropsDialog(props, button);
                DialogResult result = edSvc.ShowDialog(dialog);
                if (result == DialogResult.OK)
                {
                    button.ButtonProps = props;
                    button.Text = props.DisplayLabel;
                    button.Form.Modified = true;
                    return props;
                }
            }
            return value;
        }
        #endregion
    }
}
