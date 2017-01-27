import { TypedRecord } from 'typed-immutable-record';

export interface IRecordDetail {
  temporary: boolean;
  id: number;
  data: string;
}

export interface IRecordDetailRecord extends TypedRecord<IRecordDetailRecord>, IRecordDetail { }

export interface IRecords {
  temporary: boolean;
  rows: any[];
  gridColumns: any[];
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, IRecords { }

export interface IRegistry {
  records: IRecordsRecord;
  tempRecords: IRecordsRecord;
  currentRecord: IRecordDetail;
  structureData: string;
}

export interface IRegistryRecord extends TypedRecord<IRegistryRecord>, IRegistry { }
