using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CambridgeSoft.COE.Framework.Common.Messaging;
using System.Xml.Serialization;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.COEDisplayDataBrokerService
{
    public static class COEDisplayDataBroker
    {
        public static object GetDisplayData(string assemblyName, string className, string methodName, object[] paramValues, Type[] paramTypes)
        {
            Type type = null;
            object result = null;

            if(paramValues != null && paramValues.Length > 0)
            {
                if(paramValues.Length != paramTypes.Length)
                    throw new Exception("Paramater values count does not match with parameter types count.");
                for(int i = 0; i < paramValues.Length; i++)
                    paramValues[i] = EnsureValueType(paramValues[i], paramTypes[i]);
            }

            if(!string.IsNullOrEmpty(assemblyName))
            {
                System.Reflection.Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName.Trim());
                type = assembly.GetType(className);
            }
            else
            {
                type = Type.GetType(className);
            }
            if(type == null)
                throw new Exception(string.Format("There is no class ({0}) in assembly {1}.", className, assemblyName));

            if(!string.IsNullOrEmpty(methodName))
            {
                MethodInfo method = type.GetMethod(methodName, paramTypes);

                if(method == null || !method.IsStatic)
                    throw new Exception(string.Format("There is no static method ({0}) defined in {1}.", methodName, className));

                result = method.Invoke(null, paramValues);
            }
            else
            {
                ConstructorInfo constructor = null;
                constructor = type.GetConstructor(paramTypes);

                if(constructor == null)
                    throw new Exception("There is no public constructor with the given parameters.");

                result = constructor.Invoke(paramValues);
            }
            return result;
        }
        
        public static object GetDisplayData(string assemblyName, string className, string methodName, object[] paramValues, string[] paramTypes)
        {
            Type[] types = Type.EmptyTypes;
            if(paramTypes != null && paramTypes.Length > 0)
            {
                types = new Type[paramTypes.Length];
                for(int i = 0; i < paramTypes.Length; i++)
                {
                    types[i] = Type.GetType(paramTypes[i]);
                }
            }
            return GetDisplayData(assemblyName, className, methodName, paramValues, types);
        }
        
        public static object GetDisplayData(string assemblyName, string className, string methodName, object[] paramValues)
        {
            Type[] types = new Type[paramValues.Length];
            for(int i = 0; i < paramValues.Length; i++)
            {
                types[i] = paramValues[i].GetType();
            }
            return GetDisplayData(assemblyName, className, methodName, paramValues, types);
        }

        public static object GetDisplayData(string assemblyName, string className, string methodName)
        {
            return GetDisplayData(assemblyName, className, methodName, null, Type.EmptyTypes);
        }

        public static object GetDisplayData(string className, string methodName)
        {
            return GetDisplayData(null, className, methodName, null, Type.EmptyTypes);
        }

        public static object GetDisplayData(FormGroup.DisplayData displayData)
        {
            string assemblyName = displayData.Assembly != null && !string.IsNullOrEmpty(displayData.Assembly.Name) ? displayData.Assembly.Name : null;
            string className = displayData.Class != null && !string.IsNullOrEmpty(displayData.Class.Name) ? displayData.Class.Name : null;
            string methodName = displayData.Method != null && !string.IsNullOrEmpty(displayData.Method.Name) ? displayData.Method.Name : null;
            int paramCount = displayData.Method != null && displayData.Method.ParamList != null ? displayData.Method.ParamList.Length : 0;
            string[] paramTypes = new string[paramCount];
            object[] paramValues = new object[paramCount];

            for(int i = 0; i < paramCount; i++)
            {
                paramTypes[i] = displayData.Method.ParamList[i].Type;
                paramValues[i] = displayData.Method.ParamList[i].Value;
            }

            return GetDisplayData(assemblyName, className, methodName, paramValues, paramTypes);
        }

        public static object GetDisplayData(string displayDataXml)
        {
            FormGroup.DisplayData displayData = Utilities.XmlDeserialize <FormGroup.DisplayData>(displayDataXml);
            return GetDisplayData(displayData);
        }

        private static object EnsureValueType(object value, Type targetType)
        {
            if(targetType.Equals(typeof(Guid)))
                return new Guid(value.ToString());
            else if(!targetType.IsInstanceOfType(value))
                return Convert.ChangeType(value, targetType);

            return value;
        }
    }
}
