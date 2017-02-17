import { RegistryActions, RecordDetailActions, IPayloadAction } from '../../actions';
import { INITIAL_STATE, INITIAL_RECORD_DETAIL } from './registry.initial-state';
import { IRegistryRecord, IRecordDetail, IRecords } from './registry.types';

export function registryReducer(
  state: IRegistryRecord = INITIAL_STATE,
  action: IPayloadAction
): IRegistryRecord {

  let recordsPath = (temporary: boolean): any[] => {
    let records: string = temporary ? 'tempRecords' : 'records';
    return [records, 'rows'];
  };

  switch (action.type) {
    case RegistryActions.OPEN_RECORDS:
      let a1 = action as ReduxActions.Action<boolean>;
      return state.updateIn(recordsPath(a1.payload), () => []);

    case RegistryActions.OPEN_RECORDS_SUCCESS:
      let a2 = action as ReduxActions.Action<IRecords>;
      return state.updateIn(recordsPath(a2.payload.temporary), () => a2.payload.rows);

    case RecordDetailActions.CLEAR_RECORD:
      return state.update('currentRecord', () => INITIAL_RECORD_DETAIL);

    case RecordDetailActions.RETRIEVE_RECORD_SUCCESS:
      let a3 = action as ReduxActions.Action<IRecordDetail>;
      return state.update('currentRecord', () => a3.payload);

    case RecordDetailActions.LOAD_STRUCTURE:
      return state.update('structureData', () => null );

    case RecordDetailActions.LOAD_STRUCTURE_SUCCESS:
      let cdxml = (action.payload as { data }).data;
      return state.update('structureData', () => cdxml);

    default:
      return state;
  }
}
