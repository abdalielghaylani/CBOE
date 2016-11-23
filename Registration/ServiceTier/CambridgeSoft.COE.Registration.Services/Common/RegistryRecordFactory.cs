using CambridgeSoft.COE.Registration.Services.Types;

//TODO: Use a true caching mechanism instead of a static to allows expiration.

namespace CambridgeSoft.COE.DataLoader.Core
{
    public class RegistryRecordFactory
    {
        private static RegistryRecord _nascentRegistryRecord = null;

        /// <summary>
        /// The purpose of this method is to produce a nascent instance of RegistryRecord upon
        /// request.
        /// First we get a prototype instance, then we add some initialization and make it serve as 
        /// the real template for cloning. The method will return a deep copy of that template eventually.
        /// </summary>
        /// <returns></returns>
        public static RegistryRecord GetNascentRegistryRecord()
        {
            if (_nascentRegistryRecord == null)
            {
                _nascentRegistryRecord = RegistryRecord.NewRegistryRecord();
                _nascentRegistryRecord.DataStrategy = RegistryRecord.DataAccessStrategy.BulkLoader;
            }

            return _nascentRegistryRecord.Clone();
        }
    }
}
