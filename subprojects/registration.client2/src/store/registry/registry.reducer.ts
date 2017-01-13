import { Action } from 'redux';
import { RegistryActions, GridActions } from '../../actions';
import { SessionActions } from '../../actions/session.actions';
import { INITIAL_STATE } from './registry.initial-state';
import { IRegistryRecord } from './registry.types';
import { DxDataGridModule, DxDataGridComponent } from 'devextreme-angular';

export function registryReducer(
  state: IRegistryRecord = INITIAL_STATE,
  action: ReduxActions.Action<any>
): IRegistryRecord {

  switch (action.type) {
    case RegistryActions.OPEN_RECORDS_SUCCESS:
      return state.update('rows', () => action.payload);

    case RegistryActions.OPEN_RECORDS_ERROR:
      return state.update('rows', () => []);

    default:
      return state;
  }
}
