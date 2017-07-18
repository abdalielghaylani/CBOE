import { TypedRecord } from 'typed-immutable-record';

export interface IRecordDetail {
  temporary: boolean;
  id: number;
  data: string;
  isLoggedInUserOwner: boolean;
  isLoggedInUserSuperVisor: boolean;
}

export interface IRecordDetailRecord extends TypedRecord<IRecordDetailRecord>, IRecordDetail { }

export interface IRecordsData {
  temporary: boolean;
  startIndex: number;
  totalCount: number;
  hitlistId?: number;
  rows: any[] | any;
}

export class CRecordsData implements IRecordsData {
  temporary: boolean = false;
  startIndex: number = 0;
  totalCount: number = 0;
  histlistId?: number = 0;
  rows: any[] | Function = [];
  constructor(temporary: boolean, rows: any[] = undefined) {
    this.temporary = temporary;
    if (rows) {
      this.rows = rows;
      this.totalCount = rows.length;
    }
  }
}

export interface IRecords {
  temporary: boolean;
  data: IRecordsData;
  gridColumns: any[];
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, IRecords { }

export interface IRegistry {
  records: IRecordsRecord;
  tempRecords: IRecordsRecord;
  currentRecord: IRecordDetail;
  structureData: string;
  previousRecordDetail: IRecordDetail;
}

export interface IRegistryRecord extends TypedRecord<IRegistryRecord>, IRegistry { }

export interface IRegistryRetrievalQuery {
  temporary: boolean;
  hitlistId?: number;
  skip?: number;
  take?: number;
  sort?: string;
}

export interface IQueryData {
  temporary: boolean;
  searchCriteria: string;
}

export interface IRecordSaveData {
  temporary: boolean;
  id: number;
  recordDoc: Document;
  saveToPermanent: boolean;
  checkDuplicate: boolean;
}
