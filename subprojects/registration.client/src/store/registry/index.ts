import { IRecordDetail, IRecords, IRegistry } from './registry.types';
import { IRegistrySearch, ISearchRecords } from './registry-search.types';
import { registryReducer } from './registry.reducer';
import { RegistryFactory } from './registry.initial-state';
import { registrySearchReducer } from './registry-search.reducer';

export {
  IRecordDetail,
  IRecords,
  ISearchRecords,
  IRegistry,
  IRegistrySearch,
  registryReducer,
  registrySearchReducer,
  RegistryFactory,
}
