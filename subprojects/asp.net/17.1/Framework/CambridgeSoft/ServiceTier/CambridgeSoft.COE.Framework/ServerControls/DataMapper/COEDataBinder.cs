using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using System.ComponentModel;

namespace CambridgeSoft.COE.Framework.Controls.COEDataMapper
{
    /// <summary>
    /// <para>
    /// Utilitarian class that allows to retrieve and set property values using COEFramework's binding expressions and using Reflection.
    /// </para>
    /// </summary>
    public class COEDataBinder
    {
        #region Variables
        object _rootObject;
        string[] separators = { ".", "[", "]", "'" };
        #endregion

        #region Properties
        public object RootObject
        {
            get
            {
                return _rootObject;
            }
            set
            {
                _rootObject = value;
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public COEDataBinder(object parentObject)
        {
            this._rootObject = parentObject;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Sets the value of the property specified by the binding expression.
        /// </summary>
        /// <param name="rootObject">The object that contains the property at some depth level</param>
        /// <param name="bindingExpression">The expression that specifies the property to look for, relative to root object</param>
        /// <param name="value">The new value of the property</param>
        /// <remarks>
        /// IMPORTANT: we don't currently support setting value for 'this' expression, which would
        /// need by reference use of the root object.
        /// </remarks>
        /// <summary>
        /// Sets the value of the property specified by the binding expression.
        /// </summary>
        /// <param name="rootObject">The object that contains the property at some depth level</param>
        /// <param name="bindingExpression">The expression that specifies the property to look for, relative to root object</param>
        /// <param name="value">The new value of the property</param>
        /// <remarks>
        /// IMPORTANT: we don't currently support setting value for 'this' expression, which would
        /// need by reference use of the root object.
        /// </remarks>
        public void SetProperty(string bindingExpression, object value)
        {
            if (COEDataMapper.RemoveIDFromSourceString(bindingExpression) != "this")
            {
                try
                {
                    object localParentObject = null;
                    PropertyInfo propertyInfo = null;
                    object[] index = null;

                    ResolvePropertyInfo(new XmlTranslation.Tokenizer(this.NormalizeBindingExpression(COEDataMapper.RemoveIDFromSourceString(bindingExpression)), separators),
                                                        ref  localParentObject,
                                                        ref propertyInfo,
                                                        ref index);

                    if (localParentObject.GetType().IsArray)
                    {
                        int[] indices = new int[index.Length];

                        for (int i = 0; i < indices.Length; i++)
                        {
                            indices[i] = (int)index[i];
                        }

                        ((Array)localParentObject).SetValue(value, indices);
                    }
                    else if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        value = EnsureValueType(value, propertyInfo.PropertyType);
                        propertyInfo.SetValue(localParentObject, value, index);
                    }
                    else
                        localParentObject = value;
                }
                catch (Exception exception)
                {
                    if (exception.InnerException == null || string.IsNullOrEmpty(exception.InnerException.Message))
                    {
                        string errorMessage = string.Format("Cannot set property {0} from object {1} {2}: {3}",
                                                                          bindingExpression,
                                                                          this._rootObject != null ? this._rootObject.GetType().Name : "null",
                                                                          this._rootObject != null ? this._rootObject.ToString() : "null",
                                                                          exception.Message);
                        throw new Exception(errorMessage/*, exception*/);
                    }
                    else
                        throw;
                }
            }

        }

        public object RetrieveProperty(string bindingExpression)
        {
            return RetrieveProperty(new XmlTranslation.Tokenizer(COEDataMapper.RemoveIDFromSourceString(bindingExpression), separators));
        }

        //Compound.PropertyList.Property[@name="CAS"|Value]
        /// <summary>
        /// Resolves the binding Expression and retrieves the corresponding property.
        /// </summary>
        /// <param name="parent">The object on wich the search for the property will be performed.</param>
        /// <param name="bindingExpression">The expression that describes the location of the property relative to the parent object</param>
        /// <returns>The value of the property</returns>
        public object RetrieveProperty(XmlTranslation.Tokenizer tokenizer)
        {
            try
            {
                object localParentObject = null;
                PropertyInfo propertyInfo = null;
                object[] index = null;


                tokenizer.Text = this.NormalizeBindingExpression(tokenizer.GetRemainingText());

                ResolvePropertyInfo(tokenizer,
                                                    ref  localParentObject,
                                                    ref propertyInfo,
                                                    ref index);

                if (localParentObject.GetType().IsArray)
                {
                    int[] indices = new int[index.Length];

                    for (int i = 0; i < indices.Length; i++)
                    {
                        indices[i] = (int)index[i];
                    }

                    return ((Array)localParentObject).GetValue(indices);
                }
                else if (propertyInfo != null && propertyInfo.CanRead)
                {
                    return propertyInfo.GetValue(localParentObject, index);
                }
                else
                    return localParentObject;
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format("Cannot retrieve property {0} from object {1} {2}: {3}",
                                                                 tokenizer.ToString(false),
                                                                 this._rootObject != null ? this._rootObject.GetType().Name : "null",
                                                                 this._rootObject != null ? this._rootObject.ToString() : "null",
                                                                 exception.Message);
                throw new Exception(errorMessage/*, exception*/);
            }
        }

        /// <summary>
        /// Dynamically locates a method in a class and invokes it with passed parameter values.
        /// For now, we only support method overloads with different parameter types. Other parts of 
        /// method overloads are not used and therefore not supported.
        /// </summary>
        /// <param name="methodBindingPath">The binding expression to identify the method.</param>
        /// <param name="isStatic">Specifies whether or not the method is a static one.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="methodParamTypes">The parameter types to help identify different method overloads, if any.</param>
        /// <param name="methodParamValues">The parameter values used to invoke the method.</param>
        public void InvokeMethod(string methodBindingPath, bool isStatic, string methodName, Type[] methodParamTypes, object[] methodParamValues)
        {
            object parentObject = null;
            MethodInfo methodInfo = null;

            if ((parentObject = this.RetrieveProperty(methodBindingPath)) != null)
            {
                methodInfo = parentObject.GetType().GetMethod(methodName,
                    BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance),
                    null,
                    methodParamTypes,
                    null);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(parentObject, methodParamValues);
                }
            }
        }

        public bool ContainsProperty(string bindingExpression)
        {
            try
            {
                object localParentObject = null;
                PropertyInfo propertyInfo = null;
                object[] index = null;

                ResolvePropertyInfo(new XmlTranslation.Tokenizer(this.NormalizeBindingExpression(COEDataMapper.RemoveIDFromSourceString(bindingExpression)), separators),
                                                    ref  localParentObject,
                                                    ref propertyInfo,
                                                    ref index);

                if (propertyInfo != null && propertyInfo.CanWrite)
                    ;/*propertyInfo.SetValue(localParentObject, value, index);*/
                else
                    ;/* localParentObject = value;*/

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        #endregion

        #region Private Methods
        private object EnsureValueType(object value, Type targetType)
        {
            if (targetType.Equals(typeof(Guid)))
                return new Guid(value.ToString());
            else if (!targetType.IsInstanceOfType(value))
            {
                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                    if (converter != null && value != null && converter.CanConvertFrom(value.GetType()))
                    {
                        return converter.ConvertFrom(value);
                    }
                    return Convert.ChangeType(value, targetType);
                }
                catch
                {
                    return Activator.CreateInstance(targetType);
                }
            }

            return value;
        }
        /// <summary>
        /// Sets the value of the property specified by the binding expression.
        /// </summary>
        /// <param name="rootObject">The object that contains the property at some depth level</param>
        /// <param name="bindingExpression">The expression that specifies the property to look for, relative to root object</param>
        /// <param name="value">The new value of the property</param>

        private string NormalizeBindingExpression(string bindingExpression)
        {
            bindingExpression = bindingExpression.Trim();

            if (!string.IsNullOrEmpty(bindingExpression))
            {
                if (!bindingExpression.StartsWith("this"))
                {
                    return "this" + ((string.IsNullOrEmpty(bindingExpression) || bindingExpression[0] == '[') ? bindingExpression : ("." + bindingExpression));
                }
                else
                    return bindingExpression;
            }
            return "this";
        }

        private void ResolvePropertyInfo(XmlTranslation.Tokenizer tokenizer, ref object localParentObject, ref PropertyInfo propertyInfo, ref object[] index)
        {
            string currentToken = string.Empty;

            while (!string.IsNullOrEmpty(currentToken = tokenizer.ViewNextToken()))
            {
                switch (currentToken)
                {
                    case "this":
                        tokenizer.GetNextToken();
                        localParentObject = this._rootObject;
                        propertyInfo = null;
                        index = null;
                        break;
                    case ".":
                        tokenizer.GetNextToken();
                        if (propertyInfo != null)
                        {
                            localParentObject = propertyInfo.GetValue(localParentObject, index);
                            propertyInfo = null;
                            index = null;
                        }
                        break;
                    case "]":
                        return;
                        break;
                    case "[":
                        this.parseIndexer(tokenizer, ref localParentObject, ref propertyInfo, ref index);
                        break;
                    default:
                        tokenizer.GetNextToken();
                        propertyInfo = GetPropertyInfo(localParentObject, currentToken);
                        break;
                }
            }
        }

        private void parseIndexer(XmlTranslation.Tokenizer tokenizer, ref object localParentObject, ref PropertyInfo propertyInfo, ref object[] index)
        {
            if (tokenizer.GetNextToken() == "[")
            {
                string currentToken = string.Empty;

                List<object> indexers = new List<object>();

                while ((currentToken = tokenizer.ViewNextToken()) != "]")
                {
                    switch (currentToken)
                    {
                        case null:
                        case "":
                            throw new Exception("Indexer or ] expected");
                            break;

                        default:
                            double doubleIndexer = 0;
                            int intIndexer = 0;
                            string stringIndexer = string.Empty;

                            //for customLists
                            string keyPropertyName = string.Empty;
                            string keyPropertyValue = string.Empty;
                            string valuePropertyName = string.Empty;

                            // E.g. array[0]
                            if (int.TryParse(currentToken, out intIndexer))
                            {
                                indexers.Add(intIndexer);
                                tokenizer.GetNextToken();
                            }
                            // E.g. dic[1.2]
                            else if (double.TryParse(currentToken, out doubleIndexer))
                            {
                                indexers.Add(doubleIndexer);
                                tokenizer.GetNextToken();
                            }
                            // E.g. array["Item1"]
                            else if (TryParseString(tokenizer, out stringIndexer))
                                indexers.Add(stringIndexer);
                            // E.g. ?
                            else if (TryParseCustomList(tokenizer, out keyPropertyName, out keyPropertyValue, out valuePropertyName))
                                GetPropertyInfoFromCustomList(keyPropertyName, keyPropertyValue, valuePropertyName, ref localParentObject, ref propertyInfo, ref index);
                            // E.g. ?
                            else //subexpression.
                            {
                                COEDataBinder subDataBinder = new COEDataBinder(this.RootObject);
                                object subExpressionObject = subDataBinder.RetrieveProperty(tokenizer);

                                if (subExpressionObject == null)
                                    throw new Exception("Invalid Index");

                                indexers.Add(subExpressionObject);
                            }

                            break;
                    }
                }
                tokenizer.GetNextToken();

                //Once we got the indexes, get the property and local parent object
                if (indexers.Count > 0)
                {
                    object collection = (propertyInfo == null ? localParentObject : propertyInfo.GetValue(localParentObject, index));

                    if (collection.GetType().IsArray)
                    {
                        int[] arrayIndexes = new int[indexers.Count];

                        for (int currentIndex = 0; currentIndex < indexers.Count; currentIndex++)
                            arrayIndexes[currentIndex] = (int)indexers[currentIndex];

                        localParentObject = collection;
                        index = indexers.ToArray();
                    }
                    else
                    {
                        Type[] indexesTypes = new Type[indexers.Count];

                        for (int currentIndex = 0; currentIndex < indexers.Count; currentIndex++)
                            indexesTypes[currentIndex] = indexers[currentIndex].GetType();

                        localParentObject = collection;
                        propertyInfo = collection.GetType().GetProperty("Item", indexesTypes);
                        index = indexers.ToArray();
                    }
                }
            }
        }

        private PropertyInfo GetPropertyInfo(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new Exception(string.Format("Property {0} not found or not accesible", propertyName));

            return propertyInfo;
        }

        private void GetPropertyInfoFromCustomList(string keyPropertyName, string keyPropertyValue, string valuePropertyName, ref object parentObject, ref PropertyInfo propertyInfo, ref object[] index)
        {
            index = null;
            object localParentObject = null;
            PropertyInfo localPropertyInfo = null;

            object collection = propertyInfo.GetValue(parentObject, null);
            if (collection == null)
                throw new Exception(string.Format("Unable to get value of property {0}", propertyInfo.Name));

            if (collection is IEnumerable)
            {
                foreach (object currentCollectionItem in ((IEnumerable)collection))
                {

                    localPropertyInfo = GetPropertyInfo(currentCollectionItem, keyPropertyName);
                    object value = localPropertyInfo.GetValue(currentCollectionItem, null);

                    if (value == null)
                        throw new Exception(string.Format("Unable to get value of property {0}", localPropertyInfo.Name));

                    if (value.ToString() == keyPropertyValue)
                    {
                        parentObject = currentCollectionItem;
                        propertyInfo = GetPropertyInfo(currentCollectionItem, valuePropertyName);
                        return;
                    }
                }
                throw new Exception(string.Format("Property {0} not found", keyPropertyValue));
            }
            else
                throw new Exception(string.Format("Property {0} is not a collection", propertyInfo.Name));
        }

        private bool TryParseCustomList(XmlTranslation.Tokenizer tokenizer, out string keyPropertyName, out string keyPropertyValue, out string valuePropertyName)
        {
            tokenizer.PushPosition();

            keyPropertyName = string.Empty;
            keyPropertyValue = string.Empty;
            valuePropertyName = string.Empty;

            string propertyName = tokenizer.GetNextToken().Trim();

            if (propertyName.StartsWith("@") && propertyName.EndsWith("="))
            {
                keyPropertyName = propertyName.Substring(1, propertyName.Length - 2);

                //propertyName = tokenizer.GetNextToken();
                propertyName = tokenizer.GetString();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    keyPropertyValue = propertyName;

                    propertyName = tokenizer.GetNextToken().Trim();

                    if (propertyName.StartsWith("|"))
                    {
                        valuePropertyName = propertyName.Substring(1).Trim();
                        tokenizer.RemoveTopPosition();
                        return true;
                    }
                }
            }

            tokenizer.PopPosition();

            return false;
        }

        private bool TryParseString(XmlTranslation.Tokenizer tokenizer, out string stringIndexer)
        {
            stringIndexer = string.Empty;

            tokenizer.PushPosition();

            if (tokenizer.GetNextToken() == "'")
            {
                string token = string.Empty;

                while ((token = tokenizer.GetNextToken()) != "'")
                {
                    switch (token)
                    {
                        case null:
                        case "":
                            tokenizer.PopPosition();
                            return false;
                            break;

                        default:
                            stringIndexer += token;
                            break;
                    }
                }
                tokenizer.RemoveTopPosition();
                return true;
            }
            tokenizer.PopPosition();
            return false;
        }
        #endregion
        #endregion
    }
}
