import { combineReducers } from 'redux';
import { routerReducer } from '@angular-redux/router';
import * as session from './session';
import * as configuration from './configuration';
import * as registry from './registry';


export interface IAppState {
  session?: session.ISession;
  configuration?: configuration.IConfiguration;
  registry?: registry.IRegistry;
};

export const rootReducer = combineReducers<IAppState>({
  session: session.sessionReducer,
  configuration: configuration.configurationReducer,
  registry: registry.registryReducer,
  router: routerReducer,
});

export function deimmutify(store) {
  return {
    session: store.session.toJS(),
    router: store.router,
  };
}

export function reimmutify(plain) {
  return {
    session: session.SessionFactory(plain.session),
    router: plain.router,
  };
}
