using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Defines the set of destination objects/services/etc. which source data can be mapped onto.
    /// </summary>
    public enum DestinationType
    {
        /// <summary>
        /// an CambridgeSoft.COE.Registration.Services.Types.RegistryRecord object
        /// <para>
        /// This will be the only supported destination object type for the first version of the DataUploader
        /// </para>
        /// </summary>
        RegistrationCsla = 1,
        /// <summary>
        /// an XML document which can initialize a RegistryRecord instance via the Registration web service.
        /// <para>
        /// This is not supported for the first version of the DataUploader
        /// </para>
        /// </summary>
        RegistrationWebService = 2,
    }
}
