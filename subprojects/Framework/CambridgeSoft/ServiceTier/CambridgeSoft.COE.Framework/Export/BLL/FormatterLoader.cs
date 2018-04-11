using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEExportService
{
    internal class FormatterLoader
    {
    

        internal  IFormatter InstantiateFormatter(string assemblyName, string className)
        {
            IFormatter formatter;
           
            Assembly assembly = null;

          
            assembly = System.Reflection.Assembly.Load(assemblyName);

            formatter = (IFormatter)assembly.CreateInstance(className.Trim());
          
            
            return formatter;
        }

        //
        internal IFormatterAdvanced InstantiateFormatterAdvanced(string assemblyName, string className)
        {
            IFormatterAdvanced formatterAdvanced;
            Assembly assembly = null;
            assembly = System.Reflection.Assembly.Load(assemblyName);
            formatterAdvanced = (IFormatterAdvanced)assembly.CreateInstance(className.Trim());
            return formatterAdvanced;
        }
        //

        // Advance Export - function for IFormInterchangeFormatter
        internal IFormInterchangeFormatter InstantiateFormInterchangeFormatter(string assemblyName, string className)
        {
            IFormInterchangeFormatter formInterchangeFormatter;
            Assembly assembly = null;
            assembly = System.Reflection.Assembly.Load(assemblyName);
            formInterchangeFormatter = (IFormInterchangeFormatter)assembly.CreateInstance(className.Trim());
            return formInterchangeFormatter;
        }
    }
}
