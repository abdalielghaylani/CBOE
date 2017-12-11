import { TypedRecord } from 'typed-immutable-record';
import { IResponseData } from '../../../components/registry/registry.types';

export interface IRecordDetail {
  temporary: boolean;
  id: number;
  data: string;
  isLoggedInUserOwner: boolean;
  isLoggedInUserSuperVisor: boolean;
  inventoryContainers : IInventoryContainerList;
}

export interface IRecordDetailRecord extends TypedRecord<IRecordDetailRecord>, IRecordDetail { }

export interface IRecordsData {
  temporary: boolean;
  startIndex: number;
  totalCount: number;
  hitlistId?: number;
  rows: any[];
}

export class CRecordsData implements IRecordsData {
  temporary: boolean = false;
  startIndex: number = 0;
  totalCount: number = 0;
  hitlistId?: number = 0;
  rows: any[] = [];
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
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, IRecords { }

export interface IRegistry {
  records: IRecordsRecord;
  tempRecords: IRecordsRecord;
  currentRecord: IRecordDetail;
  structureData: string;
  previousRecordDetail: IRecordDetail;
  duplicateRecords: any[] | any;
  bulkRegisterRecords: any[] | any;
  saveResponse: ISaveResponseData;
  responseData: IResponseData;
}

export interface IRegistryRecord extends TypedRecord<IRegistryRecord>, IRegistry { }

export interface IRegistryRetrievalQuery {
  temporary: boolean;
  hitlistId?: number;
  skip?: number;
  take?: number;
  sort?: string;
  hitListId?: number;
  highlightSubStructures?: boolean;
}

export interface IRegisterRecordList {
  duplicateAction: String;
  records: string[];
  description: string;
}

export interface IInventoryContainerList { 
  containers: any[];
  batchContainers: any[];
}

export interface IQueryData {
  temporary: boolean;
  searchCriteria: string;
  highlightSubStructures?: boolean;
}

export interface IRecordSaveData {
  temporary: boolean;
  id: number;
  recordDoc: Document;
  saveToPermanent: boolean;
  recordData?: IRecordData;
}

export interface IRecordData {
  data?: string;
  duplicateCheckOption?: string;
  action?: string; 
}

export class CSaveResponseData {
  constructor(public id: number, public temporary: boolean, public error?: any) {
  }
}

export interface ISaveResponseData extends CSaveResponseData {
}
