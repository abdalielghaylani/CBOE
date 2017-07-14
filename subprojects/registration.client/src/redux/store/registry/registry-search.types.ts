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
  id: number;
  refresh?: boolean;
  data?: { id1: number, id2: number, op: string };
}

export interface ISearchRecords {
  rows?: IHitlistData[];
}

export interface ISearchRecordsRecord extends TypedRecord<ISearchRecordsRecord>, ISearchRecords { }

export interface IRegistrySearch {
  hitlist?: ISearchRecordsRecord;
}

export interface IRegistrySearchRecord extends TypedRecord<IRegistrySearchRecord>, IRegistrySearch { }

const INITIAL_RECORDS = makeTypedFactory<ISearchRecords, ISearchRecordsRecord>({
  rows: []
})();

export const SearchFactory = makeTypedFactory<IRegistrySearch, IRegistrySearchRecord>({
  hitlist: INITIAL_RECORDS
});

export const INITIAL_SEARCH_STATE = SearchFactory();
