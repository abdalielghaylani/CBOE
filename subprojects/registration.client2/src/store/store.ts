import { combineReducers } from 'redux';
import { routerReducer } from 'ng2-redux-router';
import * as counter from './counter';
import * as session from './session';
import * as configuration from './configuration';
import * as registry from './registry';


export interface IAppState {
  counter?: counter.ICounter;
  session?: session.ISession;
  configuration?: configuration.IConfiguration;
  records: registry.IRegistry;
  tempRecords: registry.IRegistry;
};

export const rootReducer = combineReducers<IAppState>({
  counter: counter.counterReducer,
  session: session.sessionReducer,
  configuration: configuration.configurationReducer,
  records: registry.registryReducer,
  tempRecords: registry.registryReducer,
  router: routerReducer,
});

export function deimmutify(store) {
  return {
    counter: store.counter.toJS(),
    session: store.session.toJS(),
    router: store.router,
  };
}

export function reimmutify(plain) {
  return {
    counter: counter.CounterFactory(plain.counter),
    session: session.SessionFactory(plain.session),
    router: plain.router,
  };
}
