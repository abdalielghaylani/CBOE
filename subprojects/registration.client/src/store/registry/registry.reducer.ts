import { RegistryActions, RecordDetailActions, IPayloadAction } from '../../actions';
import { INITIAL_REGISTRY_STATE, INITIAL_RECORD_DETAIL } from './registry.initial-state';
import { IRegistryRecord, IRecordDetail, IRecords, CRecordsData } from './registry.types';

export function registryReducer(
  state: IRegistryRecord = INITIAL_REGISTRY_STATE,
  action: IPayloadAction
): IRegistryRecord {

  let recordsDataPath = (payload: any): string[] => {
    let temporary: boolean = payload.temporary;
    let records: string = temporary ? 'tempRecords' : 'records';
    return [records, 'data'];
  };

  switch (action.type) {
    case RegistryActions.OPEN_RECORDS_ERROR:
      let a1 = action as ReduxActions.Action<boolean>;
      return state.updateIn(recordsDataPath(a1.payload), () => new CRecordsData(a1.payload));

    case RegistryActions.OPEN_RECORDS_SUCCESS:
      let a2 = action as ReduxActions.Action<IRecords>;
      return state.updateIn(recordsDataPath(a2.payload), () => a2.payload.data);

    case RecordDetailActions.CLEAR_RECORD:
      return state.update('currentRecord', () => INITIAL_RECORD_DETAIL);

    case RecordDetailActions.RETRIEVE_RECORD_SUCCESS:
      let a3 = action as ReduxActions.Action<IRecordDetail>;
      return state.update('currentRecord', () => a3.payload);

    case RecordDetailActions.LOAD_STRUCTURE:
      return state.update('structureData', () => null);

    case RecordDetailActions.LOAD_STRUCTURE_SUCCESS:
      let cdxml = (action.payload as { data }).data;
      return state.update('structureData', () => cdxml);

    default:
      return state;
  }
}
