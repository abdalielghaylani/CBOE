using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

using CambridgeSoft.COE.Framework.Common;

namespace FormDBLib
{
    public class DateCriteriaConverter: ExpandableObjectConverter
    {
        #region Methods
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(SearchCriteria.DateCriteria))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        //--------------------------------------------------------------------------
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                               object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is SearchCriteria.DateCriteria)
            {
                SearchCriteria.DateCriteria sOptions = (SearchCriteria.DateCriteria)value;

                return "Inner text:" + sOptions.InnerText +
                       ", negate: " + sOptions.Negate +
                       ", operator: " + sOptions.Operator +
                       ", value:" + sOptions.Value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        //--------------------------------------------------------------------------
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        //--------------------------------------------------------------------------
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string s = (string)value;
                    int colon = s.IndexOf(':');
                    int comma = s.IndexOf(',');

                    if (colon != -1 && comma != -1)
                    {
                        string innerText = s.Substring(colon + 1,
                                                        (comma - colon - 1));

                        colon = s.IndexOf(':', comma + 1);
                        comma = s.IndexOf(',', comma + 1);

                        string negate = s.Substring(colon + 1,
                                                        (comma - colon - 1));

                        colon = s.IndexOf(':', comma + 1);

                        //This is incomplete. It needs some coding for other attributtes as "Operator" and "Value"

                        SearchCriteria.DateCriteria dc = new SearchCriteria.DateCriteria();

                        dc.InnerText = innerText;
                        dc.Negate = (negate.Equals("Yes") ? SearchCriteria.COEBoolean.Yes : SearchCriteria.COEBoolean.No);
                        //dc.Operator 
                        //dc.Value
                        
                        return dc;
                    }
                }
                catch(Exception ex)
                {
                    StringBuilder message = new StringBuilder("Can not convert '");
                    message.Append((string)value);
                    message.Append("' to type SearchOptions");

                    throw new Exceptions.SearchException(message.ToString(),ex);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        #endregion
    }
}
