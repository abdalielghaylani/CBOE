using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace CambridgeSoft.COE.Framework.Controls.COEDataMapper {
    /// <summary>
    /// <para>
    /// Utilitarian class that allows to Map CEOFramework's binding expressions to objects.
    /// This class uses <see cref="COEDataBinder"/> on its behalf.
    /// </para>
    /// </summary>
    public class COEDataMapper {
        /// <summary>
        /// Maps a 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Map(IDictionary source, object target) {
            COEDataBinder dataBinder = new COEDataBinder(target);

            foreach(string bindingExpression in source.Keys) {
                object prop = dataBinder.RetrieveProperty(bindingExpression);
                if(prop is IList)
                {
                    if(source[bindingExpression] is DataTable)
                        SetCollectionValues((IList) prop, (DataTable) source[bindingExpression]);
                    else
                        new Exception("Types missmatch, got " + source[bindingExpression].GetType() + " when " + prop.GetType() + " was expected");
                }
                else
                {
                    if(COEDataMapper.RemoveIDFromSourceString(bindingExpression) != "this")
                        dataBinder.SetProperty(bindingExpression, source[bindingExpression]);
                }
            }
        }

        private static void SetCollectionValues(IList property, DataTable source) {
            COEDataBinder dataBinder = new COEDataBinder(null);

            for(int i = 0; i < property.Count && i < source.Rows.Count; i++) {
                dataBinder.RootObject = property[i];

                for(int j = 0; j < source.Columns.Count; j++) {
                    dataBinder.SetProperty(source.Columns[j].ColumnName, source.Rows[i][j]);
                }
            }
        }

        /// <summary>
        /// Removed the controlId given a binding expression.
        /// When binding, the formgenerator has added the controlID to the bindingexpresion (needed to have unique keys in the IDictionary)
        /// </summary>
        /// <param name="source">String (bindingexpresion) to remove unnecesary control ID</param>
        /// <returns>Clean string (binding expresion)</returns>
        public static string RemoveIDFromSourceString(string source)
        {
            string splitter = CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.ControlIdsSplitter;
            string retVal = source;
            int start = -1;
            int end = -1;
            start = source.IndexOf(splitter);
            if(start > -1)
                end = source.LastIndexOf(splitter);
            if(start > -1 && end > -1)
                retVal = source.Remove(start, end - start + 1);
            return retVal;
        }
    }
}
