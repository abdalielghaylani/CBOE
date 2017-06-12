import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';

export enum HitlistType {
  TEMP,
  SAVED,
  MARKED,
  ALL
}

export enum SearchCriteriaType {
  TEMP,
  SAVED
}

export interface IHitlistData {
  id?: number;
  hitlistId?: number;
  hitlistType: HitlistType;
  numberOfHits?: number;
  isPublic: boolean;
  searchCriteriaId?: number;
  searchCriteriaType?: number;
  name: string;
  description?: string;
  dateCreated?: Date;
  markedHitIds?: number[];
}

export interface IHitlistInfo {
  id: number;
  type: number;
}

export interface IHitlistRetrieveInfo {
  type: string;
  temporary: boolean;
  id: number;
  refresh?: boolean;
  data?: any;
}

export interface ISearchRecords {
  rows?: IHitlistData[];
  currentHitlistId?: number;
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, ISearchRecords { }

export interface IRegistrySearch {
  hitlist?: IRecordsRecord;
}

export interface IRegistrySearchRecord extends TypedRecord<IRegistrySearchRecord>, IRegistrySearch { }

const INITIAL_RECORDS = makeTypedFactory<ISearchRecords, IRecordsRecord>({
  rows: [],
  currentHitlistId: 0
})();

export const RegistryFactory = makeTypedFactory<IRegistrySearch, IRegistrySearchRecord>({
  hitlist: INITIAL_RECORDS
});

export const INITIAL_STATE = RegistryFactory();
