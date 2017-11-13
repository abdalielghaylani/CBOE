import {
  IRecordDetail, IRecordDetailRecord,
  IRecordsData, CRecordsData,
  IRecords, IRecordsRecord,
  IRegistry, IRegistryRecord,
} from './registry.types';
import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';
import { apiUrlPrefix } from '../../../configuration';

const INITIAL_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>({
  temporary: false,
  data: new CRecordsData(false)
})();

const INITIAL_TEMP_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>({
  temporary: true,
  data: new CRecordsData(true)
})();

export const INITIAL_RECORD_DETAIL = makeTypedFactory<IRecordDetail, IRecordDetailRecord>({
  temporary: true,
  id: -1,
  data: null,
  isLoggedInUserOwner: false,
  isLoggedInUserSuperVisor: false,
  inventoryContainers: null
})();

export const RegistryFactory = makeTypedFactory<IRegistry, IRegistryRecord>({
  records: INITIAL_RECORDS,
  tempRecords: INITIAL_TEMP_RECORDS,
  currentRecord: INITIAL_RECORD_DETAIL,
  previousRecordDetail: INITIAL_RECORD_DETAIL,
  structureData: null,
  duplicateRecords: null,
  bulkRegisterRecords: null,
  saveResponse: null,
  responseData: null
});

export const INITIAL_REGISTRY_STATE = RegistryFactory();
