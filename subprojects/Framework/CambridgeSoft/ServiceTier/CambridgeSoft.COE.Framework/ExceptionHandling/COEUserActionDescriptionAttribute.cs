using System;

namespace CambridgeSoft.COE.Framework.ExceptionHandling
{
    /// <summary>
    /// Attach this Attribute to a method of Business Objects.
    /// It can also be attatched to the get and set accessors of a property,but not the property itself
    /// It stores the key of a resource entry,which describe what is done in the method or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class COEUserActionDescriptionAttribute : Attribute
    {
        private string _resourceKey;
        public COEUserActionDescriptionAttribute(string key)
        {
            _resourceKey = key;
        }
        public string Key
        {
            get
            {
                return _resourceKey;
            }
        }
    }
}
