using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COEDataLoader.Core
{
    /// <summary>
    /// This class can be used to maintain a list of key/value pairs in such a way as
    /// to allow for the prediction of a data-type for each entry as well as each field
    /// from the source data as a whole.
    /// </summary>
    public class SourceDictionary
    {
        private Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();
        /// <summary>
        /// This dictionary uses the field names as keys, and type information for each field as values.
        /// </summary>
        public Dictionary<string, Type> FieldTypes
        {
            get { return _fieldTypes; }
            set { _fieldTypes = value; }
        }

        private Dictionary<string, string> _fieldValues = new Dictionary<string, string>();
        /// <summary>
        /// This dictionary uses the field names as keys, and values for each field as values.
        /// </summary>
        public Dictionary<string, string> FieldValues
        {
            get { return _fieldValues; }
            set { _fieldValues = value; }
        }

        /// <summary>
        /// Gets the type associated with the specified field.
        /// </summary>
        /// <param name="fieldName">Name of the field to get type information of</param>
        /// <returns>Type information of the specified field</returns>
        public Type TryGetFieldType(string fieldName)
        {
            Type fieldType = null;
            _fieldTypes.TryGetValue(fieldName, out fieldType);

            return fieldType;
        }

        /// <summary>
        /// Gets the value of the specified field.
        /// </summary>
        /// <param name="fieldName">Name of the field to get value of</param>
        /// <returns>Value of the specified field</returns>
        public string TryGetFieldValue(string fieldName)
        {
            string fieldValue = string.Empty;
            _fieldValues.TryGetValue(fieldName, out fieldValue);

            return fieldValue;
        }

        // TODO: Check whether we need the AddKeyDefinition method
        /// <summary>
        /// Sets type information for the specified field.
        /// </summary>
        /// <param name="fieldName">Name of the field to set type information for</param>
        /// <param name="fieldType">Type of the specified field</param>
        public void SetFieldType(string fieldName, Type fieldType)
        {
            if (_fieldTypes.ContainsKey(fieldName))
                _fieldTypes[fieldName] = fieldType;
            else
                _fieldTypes.Add(fieldName, fieldType);
        }

        //public void Set

        /// <summary>
        /// The collection of all fields.
        /// </summary>
        public Dictionary<string, Type>.KeyCollection Fields
        {
            get { return _fieldTypes.Keys; }
        }

        // TODO: Hereafter is all old code to be refactored.
        private Dictionary<string, DataPair> _instanceEntries = new Dictionary<string, DataPair>();
        /// <summary>
        /// This Dictionary uses the field (or column) names as keys, and DataPair instances
        /// as values. The DataPair can be constructed with nothing but:
        /// <list type="bullet">
        /// <item>the raw value of the field/column data-point for the current logical record,</item>
        /// <item>the same value, and culture information for deducing its the data type, or</item>
        /// <item>the same value (as an object) and its known data type</item>
        /// </list>
        /// </summary>
        public Dictionary<string, DataPair> InstanceEntries
        {
            get { return _instanceEntries; }
            set { _instanceEntries = value; }
        }

        /// <summary>
        /// If the dictionary lacks an entry for this key, return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataPair TryGetValue(string key)
        {
            DataPair val = null;
            _instanceEntries.TryGetValue(key, out val);
            return val;
        }

        /// <summary>
        /// Adds or overwrites a dictionary entry.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, DataPair pair)
        {
            if (!_instanceEntries.ContainsKey(key))
            {
                _instanceEntries.Add(key, pair);
                SourceDictionary.AddKeyDefinition(key, pair.DataType);
            }
            else
                _instanceEntries[key] = pair;
        }

        /// <summary>
        /// Eliminates a dictionary entry.
        /// </summary>
        /// <param name="key"></param>
        public void DropValue(string key)
        {
            if (_instanceEntries.ContainsKey(key))
            {
                _instanceEntries.Remove(key);
                SourceDictionary.RemoveKeyDefinition(key);
            }
        }

        #region > Static members <

        /// <summary>
        /// Complete set of keys (and their data-types) derived from all the SourceDictionary
        /// instances.
        /// </summary>
        public static Dictionary<string, Type> GlobalEntries;

        /// <summary>
        /// Called by the KeyedEntity instance "SetValue" method to accumulate keys (fields).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectType"></param>
        private static void AddKeyDefinition(string key, Type objectType)
        {
            //create dictionary instance as necessary
            if (GlobalEntries == null)
                GlobalEntries = new Dictionary<string, Type>();

            //re-evaluate key's TYPE
            if (!GlobalEntries.ContainsKey(key))
                GlobalEntries.Add(key, objectType);
            else
            {
                //the existing entry's Type
                bool performTypeReplacement = false;
                Type globalType = GlobalEntries[key];

                //replace the entry's Type as necessary
                if ( objectType != null && (globalType != objectType) )
                {
                    if (globalType == null)
                        performTypeReplacement = true;
                    else
                    {
                        //string trumps everything
                        if (objectType == typeof(string))
                            performTypeReplacement = true;

                        //int can be converted to double
                        if (globalType == typeof(int) && objectType == typeof(double))
                            performTypeReplacement = true;

                        //incompatible combinations automatically become string
                        if (
                            globalType == typeof(DateTime) && (objectType == typeof(int) || objectType == typeof(double))
                            || objectType == typeof(DateTime) && (globalType == typeof(int) || globalType == typeof(double))
                        )
                        {
                            GlobalEntries[key] = typeof(string);
                            return;
                        }
                    }
                }

                // finally, replace the existing type if warranted
                if (performTypeReplacement)
                    GlobalEntries[key] = objectType;
            }
        }

        /// <summary>
        /// Called by the KeyedEntity instance "DropValue" method to eliminate keys (fields).
        /// </summary>
        /// <param name="key"></param>
        private static void RemoveKeyDefinition(string key)
        {
            if (SourceDictionary.GlobalEntries == null)
                SourceDictionary.GlobalEntries = new Dictionary<string, Type>();
            else
                if (SourceDictionary.GlobalEntries.ContainsKey(key))
                    SourceDictionary.GlobalEntries.Remove(key);
        }

        #endregion
    }
}
