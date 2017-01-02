using System;
using System.Collections.Generic;
using System.Text;

using Csla;
using Csla.Validation;

using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Registration.Services.Common;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    /// <summary>
    /// Provides a read-only implementation of the Prototype Registration XML.
    /// </summary>
    [Serializable()]
    public class PrototypeRegistryXml : RegistrationReadOnlyBase<PrototypeRegistryXml>
    {
        string _prototype = null;
        /// <summary>
        /// Holds the prototypical Registration object xml as a string
        /// </summary>
        public string Prototype
        {
            get
            {
                CanReadProperty(true);
                return _prototype;
            }
        }

        /// <summary>
        /// CSLA (really, serialization in general) requires a parameterless constructor.
        /// </summary>
        private PrototypeRegistryXml()
        {
        }

        /// <summary>
        /// Public-facing factory method for getting the prototypical Registry object xml.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static PrototypeRegistryXml GetPrototypeRegistryXml(RegistryRecord record)
        {
            PrototypeRegistryXml result = null;
            result = DataPortal.Fetch<PrototypeRegistryXml>(new Criteria(record));
            return result;
        }

        /// <summary>
        /// CSLA server-side DAL-interactive method.
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Fetch(Criteria criteria)
        {
            string xmlText = this.RegDal.GetRegistryRecordTemporary(0);
            if (!string.IsNullOrEmpty(xmlText))
            {
                this._prototype = xmlText;
            }
        }

        /// <summary>
        /// A 'bean' class holding the fetching parameter information.
        /// </summary>
        [Serializable()]
        private class Criteria
        {
            private RegistryRecord _registry;
            public RegistryRecord Registry
            {
                get { return _registry; }
            }

            public Criteria(RegistryRecord record)
            {
                _registry = record;
            }
        }
    }
}
