import { Action } from 'redux';
import { ConfigurationActions } from '../../actions/configuration.actions';
import { SessionActions } from '../../actions/session.actions';
import { INITIAL_STATE } from './configuration.initial-state';
import { IConfigurationRecord } from './configuration.types';

export function configurationReducer(
  state: IConfigurationRecord = INITIAL_STATE,
  action: ReduxActions.Action<{ tableId: any; }>
): IConfigurationRecord {

  switch (action.type) {
    case ConfigurationActions.OPEN_TABLE:
      return state.update('tableId', (value) => action.payload.tableId);

    default:
      return state;
  }
}
