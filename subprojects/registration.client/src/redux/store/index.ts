import { rootReducer, deimmutify, reimmutify } from './store';
import { dev } from '../../configuration';
const createLogger = require('redux-logger');
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
