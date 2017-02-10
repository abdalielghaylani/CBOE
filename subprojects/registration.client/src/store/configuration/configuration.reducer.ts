import { ConfigurationActions } from '../../actions';
import { INITIAL_STATE } from './configuration.initial-state';
import { IConfigurationRecord } from './configuration.types';
import { FormGroupType, copyObjectAndSet, convertToFormGroup } from '../../common';

export function configurationReducer(
  state: IConfigurationRecord = INITIAL_STATE,
  action: ReduxActions.Action<any>
): IConfigurationRecord {

  switch (action.type) {
    case ConfigurationActions.OPEN_TABLE:
      let a1 = action as ReduxActions.Action<string>;
      let tableId = a1.payload;
      return state.update('customTables', () => copyObjectAndSet(state.customTables, tableId, []));

    case ConfigurationActions.OPEN_TABLE_SUCCESS:
      let a2 = action as ReduxActions.Action<{ tableId, data }>;
      return state.update('customTables', () => copyObjectAndSet(state.customTables, a2.payload.tableId, a2.payload.data));

    case ConfigurationActions.LOAD_FORMGROUP:
      let a3 = action as ReduxActions.Action<{ type: FormGroupType, data: string }>;
      return state.update('formGroups', () => copyObjectAndSet(state.formGroups, FormGroupType[a3.payload.type], convertToFormGroup(a3.payload.data)));

    default:
      return state;
  }
}
