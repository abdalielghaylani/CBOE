import { combineReducers } from 'redux';
import { routerReducer } from 'ng2-redux-router';
import * as counter from './counter';
import * as session from './session';
import * as configuration from './configuration';


export interface IAppState {
  counter?: counter.ICounter;
  session?: session.ISession;
  configuration?: configuration.IConfiguration;
};

export const rootReducer = combineReducers<IAppState>({
  counter: counter.counterReducer,
  session: session.sessionReducer,
  configuration: configuration.configurationReducer,
  router: routerReducer,
});

export function deimmutify(store) {
  return {
    counter: store.counter.toJS(),
    session: store.session.toJS(),
    configuration: store.configuration.toJS(),
    router: store.router,
  };
}

export function reimmutify(plain) {
  return {
    counter: counter.CounterFactory(plain.counter),
    session: session.SessionFactory(plain.session),
    configuration: configuration.ConfigurationFactory(plain.configuration),
    router: plain.router,
  };
}
