import { Action } from 'redux';
import { RegistryActions, GridActions, IPayloadAction } from '../../actions';
import { SessionActions } from '../../actions/session.actions';
import { INITIAL_STATE } from './registry.initial-state';
import { IRegistryRecord, IRecords } from './registry.types';
import { DxDataGridModule, DxDataGridComponent } from 'devextreme-angular';

function recordsPath(temporary: boolean): any[] {
  let records: string = temporary ? 'tempRecords' : 'records';
  return [ records, 'rows' ];
}

export function registryReducer(
  state: IRegistryRecord = INITIAL_STATE,
  action: IPayloadAction
): IRegistryRecord {

  switch (action.type) {
    case RegistryActions.OPEN_RECORDS:
      let a1: ReduxActions.Action<boolean> = action as ReduxActions.Action<boolean>;
      return state.updateIn(recordsPath(a1.payload), () => []);

    case RegistryActions.OPEN_RECORDS_SUCCESS:
      let a2: ReduxActions.Action<IRecords> = action as ReduxActions.Action<IRecords>;
      return state.updateIn(recordsPath(a2.payload.temporary), () => a2.payload.rows);

    default:
      return state;
  }
}
