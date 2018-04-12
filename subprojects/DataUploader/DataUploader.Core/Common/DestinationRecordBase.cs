using CambridgeSoft.COE.Framework.Services;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Defines the method handler to create a new destination record.
    /// </summary>
    /// <returns></returns>
    public delegate IDestinationRecord DestinationRecordCreationDelegate();

    /// <summary>
    /// Base class for all types of destination records.
    /// </summary>
    public abstract class DestinationRecordBase : IDestinationRecord
    {
        /// <summary>
        /// Factory method to create a new instance of specified destination record type.
        /// </summary>
        /// <typeparam name="T">Type of the destination record</typeparam>
        /// <param name="factoryMethod">Actual creation method</param>
        /// <returns>A new instance of the specified destination record type</returns>
        public static IDestinationRecord CreateNewInstance<T>(DestinationRecordCreationDelegate factoryMethod)
            where T : IDestinationRecord
        {
            return factoryMethod();
        }
    }
}
