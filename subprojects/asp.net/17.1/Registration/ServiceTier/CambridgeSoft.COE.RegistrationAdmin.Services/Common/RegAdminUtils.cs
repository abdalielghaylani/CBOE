using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

using CambridgeSoft.COE.RegistrationAdmin.Services.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace CambridgeSoft.COE.RegistrationAdmin.Services.Common
{
    /// <summary>
    /// General purpose helper calls
    /// </summary>
    public class RegAdminUtils
    {
        /// <summary>
        /// Returns the Registry prefix (e.g. RE_)
        /// </summary>
        /// <returns>Reg prefix</returns>
        public static string GetRegistryPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the Component Prefix
        /// </summary>
        /// <returns>Component Prefix</returns>
        public static string GetComponentPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the Batch Prefix
        /// </summary>
        /// <returns></returns>
        public static string GetBatchPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the Batch Component Prefix
        /// </summary>
        /// <returns></returns>
        public static string GetBatchComponentsPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the Structure Prefix
        /// </summary>
        /// <returns></returns>
        public static string GetStructurePrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns css class for Registry level properties form elements
        /// </summary>
        /// <returns>Registry CSSClass</returns>
        public static string GetRegistryCSSClass()
        {
            return "RegPropertyListFormElement";
        }

        /// <summary>
        /// Returns css class for Component level properties form elements
        /// </summary>
        /// <returns>Component CSSClass</returns>
        public static string GetComponentCSSClass()
        {
            return "ComponentPropertyListFormElement";
        }

        /// <summary>
        /// Returns css class for Batch level properties form elements
        /// </summary>
        /// <returns>Batch CSSClass</returns>
        public static string GetBatchCSSClass()
        {
            return "BatchPropertyListFormElement";
        }

        /// <summary>
        /// Returns css class for Batch Component level properties form elements
        /// </summary>
        /// <returns>Batch Component CSSClass</returns>
        public static string GetBatchCompCSSClass()
        {
            return "BatchCompPropertyListFormElement";
        }

        [COEUserActionDescription("ConvertPrecision")]
        public static string ConvertPrecision(string precision, bool toDataBase)
        {

            if (!string.IsNullOrEmpty(precision))
            {
                try
                {
                    char[] delElement ={ '.' };

                    string[] presSplit = precision.Split(delElement);

                    long pres;

                    if (toDataBase)
                    {
                        pres = Convert.ToInt64(presSplit[0]) + Convert.ToInt64(presSplit[1]);
                    }
                    else
                    {
                        pres = Convert.ToInt64(presSplit[0]) - Convert.ToInt64(presSplit[1]);
                    }

                    string presString = pres + "." + presSplit[1];

                    return presString;
                }
                catch (Exception ex)
                {
                    COEExceptionDispatcher.HandleBLLException(ex);
                    return null;
                }
            }
            else
            {
                return string.Empty;
            }

        }

        [COEUserActionDescription("CanEditPrecision")]
        public static Boolean CanEditPrecision(string oldPrecision, string newPrecision)
        {
            bool allowEdit = false;
            char[] delElement ={ '.' };

            if (!string.IsNullOrEmpty(oldPrecision) && !string.IsNullOrEmpty(newPrecision))
            {
                try
                    {
                        if (!oldPrecision.Contains(delElement[0].ToString()) && !newPrecision.Contains(delElement[0].ToString()))
                        {
                            allowEdit = (oldPrecision.ToUpper() == newPrecision.ToUpper() ? false : true);
                            return allowEdit;
                        }
                        else
                        {
                            string[] oldSplit = oldPrecision.Split(delElement);
                            Int64 oldLeft = Convert.ToInt64(oldSplit[0]);
                            Int64 oldRight = Convert.ToInt64(oldSplit[1]);
                            string[] newSplit = newPrecision.Split(delElement);
                            Int64 newLeft = Convert.ToInt64(newSplit[0]);
                            Int64 newRight = Convert.ToInt64(newSplit[1]);
                            if ((newLeft >= oldLeft) && (newRight >= oldRight))
                            {
                                allowEdit = true;
                            }
                            else
                            {
                                allowEdit = false;
                            }
                            return allowEdit;
                        }
                }
                catch (Exception ex)
                    {
                        COEExceptionDispatcher.HandleBLLException(ex);
                        return allowEdit;
                    }  
            }
            else
            {
                return allowEdit;
            }

        }

        [COEUserActionDescription("GetRegCustomFormGroupsIds")]
        public static string[] GetRegCustomFormGroupsIds()
        {
            try
            {
                string[] values = FrameworkUtils.GetAppConfigSetting("REGISTRATION", "REGADMIN", "CustomRegFormGroupsIds").Split('|');
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim();
                return values;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        [COEUserActionDescription("GetCustomPropertyStyles")]
        public static string[] GetCustomPropertyStyles()
        {
            try
            {
                string[] values = FrameworkUtils.GetAppConfigSetting("REGISTRATION", "MISC", "CustomPropertyStyles").Split('|');
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim();
                return values;
            }
            catch (Exception ex)
            {
                COEExceptionDispatcher.HandleBLLException(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns dafault formElement CSSClass
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static string GetFormElementCSSClass(string controlType)
        {
            string cssclass = string.Empty;
            switch (controlType)
            {
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly":
                    cssclass = "Std100x80";
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly":
                    cssclass = "Std20x40";
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker":
                    cssclass = "CalenderClass";
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly":
                    cssclass = "Std20x40";
                    break;
                default:
                    cssclass = "Std25x40";
                    break;
            }
            return cssclass;
        }

        /// <summary>
        /// Returns default control cssclass
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns>CSSClass</returns>
        public static string GetDefaultControlStyle(string controlType, FormGroup.DisplayMode mode)
        {
            string cssclass = string.Empty;
            switch (controlType)
            {
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextArea":
                    switch (mode)
                    {
                        case FormGroup.DisplayMode.View:
                            cssclass = "FETextAreaViewMode";
                            break;
                        default:
                            cssclass = "FETextArea";
                            break;
                    }
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextAreaReadOnly":
                    cssclass = "FETextAreaViewMode";
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COENumericTextBox":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBox":
                    switch (mode)
                    {
                        case FormGroup.DisplayMode.View:
                            cssclass = "FETextBoxViewMode";
                            break;
                        default:
                            cssclass = "FETextBox";
                            break;
                    }
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly":
                    cssclass = "FETextBoxViewMode";
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownList":
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePicker":
                    switch (mode)
                    {
                        case FormGroup.DisplayMode.View:
                            cssclass = "FEDropDownListViewMode";
                            break;
                        default:
                            cssclass = "FEDropDownList";
                            break;
                    }
                    break;
                case "CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDatePickerReadOnly":
                    cssclass = "FEDropDownListViewMode";
                    break;
                default:
                    cssclass = string.Empty;
                    break;
            }
            return cssclass;
        }

        /// <summary>
        /// Returns COELink target .
        /// </summary>
        /// <returns>Target</returns>
        public static string GetLinkTarget
        { 
           get
           {
               return "_blank";
           } 
 
        }


         #region ENUM
        /// <summary>
        /// Oracle error codes .
        /// ORA30625 = method dispatch on NULL SELF argument is disallowed.
        /// ORA20001 = The field 'Column_Name' not exist in the table 'Table_Name'.
        /// ORA01403 = No data found
        /// </summary>
        public enum ConfigurationErrorCodes
        {
            ORA30625 = 30625,
            ORA20001 = 20001,
            ORA01403 = 1403
        }
        #endregion

    }
}
