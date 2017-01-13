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
    case ConfigurationActions.CUSTOM_TABLES_SUCCESS:
      return state.update('customTables', () => action.payload);
    
    case ConfigurationActions.OPEN_TABLE:
      return state.update('tableId', () => (<ReduxActions.Action<{ tableId: any; }>>action).payload.tableId);

    case ConfigurationActions.OPEN_TABLE_SUCCESS:
      return state.update('rows', () => action.payload);

    case ConfigurationActions.OPEN_TABLE_ERROR:
      return state.update('rows', () => []);

    default:
      return state;
  }
}
