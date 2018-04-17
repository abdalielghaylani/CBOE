import {
  IRecordDetail, IRecordDetailRecord,
  IRecordsData, CRecordsData,
  IRegistry, IRegistryRecord,
} from './registry.types';
import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';
import { apiUrlPrefix } from '../../../configuration';

export const INITIAL_RECORD_DETAIL = makeTypedFactory<IRecordDetail, IRecordDetailRecord>({
  temporary: true,
  id: -1,
  data: null,
  isLoggedInUserOwner: false,
  isLoggedInUserSuperVisor: false,
  inventoryContainers: null
})();

export const RegistryFactory = makeTypedFactory<IRegistry, IRegistryRecord>({
  currentRecord: INITIAL_RECORD_DETAIL,
  previousRecordDetail: INITIAL_RECORD_DETAIL,
  structureData: null,
  duplicateRecords: null,
  bulkRegisterRecords: null,
  saveResponse: null,
  responseData: null,
  isLoading: false
});

export const INITIAL_REGISTRY_STATE = RegistryFactory();
