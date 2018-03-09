using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Base Configuration Element. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public class COEConfigurationElement : ConfigurationElement
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }

    /// <summary>
    /// Base Configuration Element Collection. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public abstract class COEConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }

    /// <summary>
    /// Base Configuration Section. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public class COEConfigurationSection : ConfigurationSection
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }

    /// <summary>
    /// Base Named Configuration Element. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public class COENamedConfigurationElement : NamedConfigurationElement
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }

    /// <summary>
    /// Base Named Configuration Element Collection. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public class COENamedElementCollection<T> : NamedElementCollection<T> where T : Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NamedConfigurationElement, new()
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }

    /// <summary>
    /// Base Configuration Section. Differs from the base in the fact that exceptions are not thrown when unrecognized attributes are present
    /// </summary>
    public class COESerializableConfigurationSection : SerializableConfigurationSection
    {
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            try
            {
                base.OnDeserializeUnrecognizedAttribute(name, value);
            }
            catch { }
            return true;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            try
            {
                base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            catch { }
            return true;
        }
    }
}
