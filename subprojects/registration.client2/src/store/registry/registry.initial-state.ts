import {
  IRecords, IRecordsRecord,
  IRegistry, IRegistryRecord,
} from './registry.types';
import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';

const INITIAL_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>(
  { temporary: false, rows: [] }
)();

const INITIAL_TEMP_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>(
  { temporary: true, rows: [] }
)();

export const RegistryFactory = makeTypedFactory<IRegistry, IRegistryRecord>({
  records: INITIAL_RECORDS,
  tempRecords: INITIAL_TEMP_RECORDS,
  temporary: true,
  currentId: -1,
  data: null,
  structureData: '',
});

export const INITIAL_STATE = RegistryFactory();
