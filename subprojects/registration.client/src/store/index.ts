import { IAppState, rootReducer, deimmutify, reimmutify } from './store';
import { ICustomTableData, CCustomTableData, IConfiguration, ConfigurationFactory } from './configuration';
import {
  IRecordDetail, IRecordsData, CRecordsData, IRecords, IRegistry, IRegistryRetrievalQuery, RegistryFactory,
  IRegistrySearch, IHitlistInfo, ISearchRecords
} from './registry';
import { ISession } from './session';

import { dev } from '../configuration';
const createLogger = require('redux-logger');
const persistState = require('redux-localstorage');

export {
  IAppState,
  ICustomTableData, CCustomTableData, IConfiguration, ConfigurationFactory,
  IRecordDetail, IRecordsData, CRecordsData, IRecords, IRegistry, IRegistryRetrievalQuery, RegistryFactory, IHitlistInfo, ISearchRecords,
  ISession,
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
