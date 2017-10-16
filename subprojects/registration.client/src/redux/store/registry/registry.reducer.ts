import { RegistryActions, RecordDetailActions, SessionActions, IPayloadAction } from '../../actions';
import { INITIAL_REGISTRY_STATE, INITIAL_RECORD_DETAIL } from './registry.initial-state';
import { IRegistryRecord, IRecordDetail, IRecords, CRecordsData, ISaveResponseData } from './registry.types';

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

    case RecordDetailActions.CLEAR_DUPLICATE_RECORD:
      return state.update('duplicateRecords', () => null);

    case RecordDetailActions.LOAD_DUPLICATE_RECORD_SUCCESS:
      return state.update('duplicateRecords', () => action.payload);

    case RegistryActions.BULK_REGISTER_RECORD_SUCCESS:
      return state.update('bulkRegisterRecords', () => action.payload);

    case RegistryActions.CLEAR_BULK_REGISTER_RECORD:
      return state.update('bulkRegisterRecords', () => null);

    case RecordDetailActions.SAVE_RECORD:
      let a4 = action as ReduxActions.Action<any>;
      return state.update('previousRecordDetail', () => a4.payload);

    case RegistryActions.DELETE_RECORD_SUCCESS:
      return state.update('responseData', () => action.payload);

    case RegistryActions.CLEAR_RESPONSE:
      return state.update('responseData', () => null);

    case RecordDetailActions.SAVE_RECORD_SUCCESS:
    case RecordDetailActions.SAVE_RECORD_ERROR:
      let a5 = action as ReduxActions.Action<ISaveResponseData>;
      return state.update('saveResponse', () => a5.payload);

    case RecordDetailActions.RETRIEVE_RECORD_SUCCESS:
      let a3 = action as ReduxActions.Action<IRecordDetail>;
      return state.update('currentRecord', () => a3.payload);

    case RecordDetailActions.LOAD_STRUCTURE:
      return state.update('structureData', () => null);

    case RecordDetailActions.LOAD_STRUCTURE_SUCCESS:
      let cdxml = (action.payload as { data }).data;
      return state.update('structureData', () => cdxml);

    case RecordDetailActions.CLEAR_SAVE_RESPONSE:
      return state.update('saveResponse', () => null);

    case SessionActions.LOGOUT_USER:
      return INITIAL_REGISTRY_STATE;

    default:
      return state;
  }
}
