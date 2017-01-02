using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Web.UI.WebControls;

namespace CambridgeSoft.COE.Framework.ServerControls.Utilities
{
    /// <summary>
    /// <para>
    /// This helper class implements a static method that is used by <see cref="ICOERequireable"/> implementations.
    /// </para>
    /// </summary>
    class LabelStylesParser
    {
        /// <summary>
        /// Parses the value string and sets the control's style
        /// <param name="value">Contains a comma separated list of CSS sentences</param>
        /// <param name="control">Control that will receive the css styles parsed</param>
        /// </summary>
        private static Dictionary<string, string> ParseStyleString(string value)
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();
            string[] styles = value.Split(new char[1] { ';' });
            for (int i = 0; i < styles.Length; i++)
            {
                if (styles[i].Length > 0)
                {
                    string[] styleDef = styles[i].Split(new char[1] { ':' });
                    string styleId = styleDef[0].Trim();
                    string styleValue = styleDef[1].Trim();
                    returnValue.Add(styleId, styleValue);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Parses the value string and sets the control's style
        /// <param name="value">Contains a comma separated list of CSS sentences</param>
        /// <param name="control">Control that will receive the css styles parsed</param>
        /// </summary>
        public static void SetRequiredControlStyles(string value, WebControl control)
        {
            Dictionary<string, string> styles = LabelStylesParser.ParseStyleString(value);
            foreach (KeyValuePair<string, string> currentValue in styles)
                control.Style.Add(currentValue.Key, currentValue.Value);
        }

        /// <summary>
        /// Parses the value string and sets the control's style
        /// <param name="value">Contains a comma separated list of CSS sentences</param>
        /// <param name="control">ICOELabelable Control that will receive the css styles parsed</param>
        /// </summary>
        public static void SetLabelStyles(string value, ICOELabelable control)
        {
            if(!string.IsNullOrEmpty(value) && control != null)
                control.LabelStyles = LabelStylesParser.ParseStyleString(value);
        }

        /// <summary>
        /// Adds the styles to be applied to the label.
        /// </summary>
        /// <param name="control">ICOELabelable Control that will receive the css styles parsed</param>
        /// <param name="lbl">Label control to apply the given styles</param>
        private static void AddLabelStyles(ICOELabelable control, Label lbl)
        {
            if (control != null && lbl != null)
            {
                if (control.LabelStyles != null)
                {
                    System.Collections.IEnumerator styles = control.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        lbl.Style.Add((string)styles.Current, (string)control.LabelStyles[(string)styles.Current]);
                    }
                }
            }
        }

        /// <summary>
        /// Configures (parses, sets and adds) the styles to the label control. 
        /// </summary>
        /// <param name="value">Contains a comma separated list of CSS sentences</param>
        /// <param name="control">ICOELabelable Control that will receive the css styles parsed</param>
        /// <param name="lbl">Label control to apply the given styles</param>
        /// <remarks>This method reuses the calls SetLabelStyles and AddLabelStyles</remarks>
        public static void ConfigureLabelStyles(string value, ICOELabelable control, Label lbl)
        {
            SetLabelStyles(value, control);
            AddLabelStyles(control, lbl);
        }

    }
}
