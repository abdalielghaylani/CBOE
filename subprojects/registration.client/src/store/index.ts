import { IAppState, rootReducer, deimmutify, reimmutify } from './store';
import { ICustomTableData, CCustomTableData, IConfiguration, ConfigurationFactory, INITIAL_STATE } from './configuration';
import {
  IRecordDetail, IRecordsData, CRecordsData, IRecords, IRegistry, IRegistryRetrievalQuery, IQueryData, RegistryFactory,
  HitlistType, SearchCriteriaType, IHitlistData, IRegistrySearch, IHitlistInfo, IHitlistRetrieveInfo, ISearchRecords
} from './registry';

import { dev } from '../configuration';
const createLogger = require('redux-logger');
const persistState = require('redux-localstorage');

export * from './session';
export {
  IAppState,
  ICustomTableData, CCustomTableData, IConfiguration, ConfigurationFactory, INITIAL_STATE,
  IRecordDetail, IRecordsData, CRecordsData, IRecords, IRegistry, IRegistryRetrievalQuery, IQueryData, RegistryFactory,
  HitlistType, SearchCriteriaType, IHitlistData, IRegistrySearch, IHitlistInfo, IHitlistRetrieveInfo, ISearchRecords,
  rootReducer,
  reimmutify
};

export let middleware = [];
export let enhancers = [
  persistState(
    '', {
      key: 'angular2-redux-seed',
      serialize: store => JSON.stringify(deimmutify(store)),
      deserialize: state => reimmutify(JSON.parse(state))
    }
  )
];

if (dev) {
  middleware.push(
    createLogger({
      level: 'info',
      collapsed: true,
      stateTransformer: deimmutify
    })
  );
}
