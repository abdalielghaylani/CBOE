using CambridgeSoft.COE.Framework.Services;

namespace CambridgeSoft.COE.DataLoader.Core.DataMapping
{
    public static class DestinationRecordFactory
    {
        public static IDestinationRecord CreateDestinationRecord(Mappings.DestinationRecordTypeEnum type)
        {
            switch (type)
            {
                case Mappings.DestinationRecordTypeEnum.RegistryRecord:
                    return RegistryRecordFactory.GetNascentRegistryRecord();

                    break;
                case Mappings.DestinationRecordTypeEnum.MockRegistryRecord:
                    return new MockRegistryRecord();
                default:
                    return null;
            }
        }
    }

}
