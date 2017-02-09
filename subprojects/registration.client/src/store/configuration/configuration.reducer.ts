import { ConfigurationActions } from '../../actions';
import { INITIAL_STATE } from './configuration.initial-state';
import { IConfigurationRecord } from './configuration.types';

export function configurationReducer(
  state: IConfigurationRecord = INITIAL_STATE,
  action: ReduxActions.Action<any>
): IConfigurationRecord {

  switch (action.type) {
    case ConfigurationActions.OPEN_TABLE:
      return state.update('tableId', () => (<ReduxActions.Action<{ tableId: any; }>>action).payload.tableId)
        .update('rows', () => []);

    case ConfigurationActions.OPEN_TABLE_SUCCESS:
      return state.update('rows', () => action.payload);

    default:
      return state;
  }
}
