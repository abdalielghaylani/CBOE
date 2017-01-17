import { Action } from 'redux';
import { ConfigurationActions, GridActions } from '../../actions';
import { SessionActions } from '../../actions/session.actions';
import { INITIAL_STATE } from './configuration.initial-state';
import { IConfigurationRecord } from './configuration.types';
import { DxDataGridModule, DxDataGridComponent } from 'devextreme-angular';

export function configurationReducer(
  state: IConfigurationRecord = INITIAL_STATE,
  action: ReduxActions.Action<any>
): IConfigurationRecord {

  switch (action.type) {
    case ConfigurationActions.LOAD_LOOKUPS_SUCCESS:
      return state.update('lookups', () => action.payload);
    
    case ConfigurationActions.OPEN_TABLE:
      return state.update('tableId', () => (<ReduxActions.Action<{ tableId: any; }>>action).payload.tableId)
        .update('rows', () => []);

    case ConfigurationActions.OPEN_TABLE_SUCCESS:
      return state.update('rows', () => action.payload);

    default:
      return state;
  }
}
