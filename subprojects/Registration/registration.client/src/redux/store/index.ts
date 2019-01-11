import { rootReducer, deimmutify, reimmutify } from './store';
import { dev } from '../../configuration';
import { createLogger } from 'redux-logger';
// const createLogger = require('redux-logger').default;
const persistState = require('redux-localstorage');

export * from './configuration';
export * from './registry';
export * from './session';
export * from './store';

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
