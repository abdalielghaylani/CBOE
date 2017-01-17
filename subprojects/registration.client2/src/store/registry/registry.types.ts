import { TypedRecord } from 'typed-immutable-record';

export interface IRecords {
  temporary: boolean;
  rows: any[];
  gridColumns: any[];
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, IRecords {
}

export interface IRegistry {
  records: IRecordsRecord;
  tempRecords: IRecordsRecord;
  temporary: boolean;
  currentId: number;
  structureData: string;
  data: string;
}

export interface IRegistryRecord extends TypedRecord<IRegistryRecord>, IRegistry {
}
