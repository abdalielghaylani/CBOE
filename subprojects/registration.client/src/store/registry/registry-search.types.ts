import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';

export interface ISearchRecords {
  rows: any[];
}

export interface IRecordsRecord extends TypedRecord<IRecordsRecord>, ISearchRecords { }

export interface IRegistrySearch {
  hitlist?: IRecordsRecord;
}

export interface IRegistrySearchRecord extends TypedRecord<IRegistrySearchRecord>, IRegistrySearch { }

const INITIAL_RECORDS = makeTypedFactory<ISearchRecords, IRecordsRecord>({
  rows: [],
})();

export const RegistryFactory = makeTypedFactory<IRegistrySearch, IRegistrySearchRecord>({
  hitlist: INITIAL_RECORDS
});

export const INITIAL_STATE = RegistryFactory();
