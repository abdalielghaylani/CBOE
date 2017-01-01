import { Action } from 'redux';
import { ConfigurationActions } from './configuration.actions';
import { CounterActions } from './counter.actions';
import { HomeActions } from './home.actions';
import { SessionActions } from './session.actions';

export interface IPayloadAction extends Action {
  payload?: any;
}

export const ACTION_PROVIDERS = [CounterActions, SessionActions];
export { ConfigurationActions, CounterActions, HomeActions, SessionActions };
