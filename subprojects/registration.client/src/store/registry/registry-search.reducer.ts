import { RegistrySearchActions, IPayloadAction } from '../../actions';
import { IRegistrySearchRecord, ISearchRecords, IHitlistRetrieveInfo, INITIAL_STATE } from './registry-search.types';
import { notify, notifyError, notifySuccess } from '../../common';

export function registrySearchReducer(
  state: IRegistrySearchRecord = INITIAL_STATE,
  action: IPayloadAction
): IRegistrySearchRecord {
  switch (action.type) {
    case RegistrySearchActions.OPEN_HITLISTS_SUCCESS:
      let a1 = action as ReduxActions.Action<ISearchRecords>;
      return state.updateIn(['hitlist', 'rows'], () => a1.payload);
    case RegistrySearchActions.RETRIEVE_HITLIST:
      let a2 = action as ReduxActions.Action<IHitlistRetrieveInfo>;
      return state.updateIn(['hitlist', 'currentHitlistId'], () => a2.payload.id);
    case RegistrySearchActions.RETRIEVE_HITLIST_ERROR:
      notifyError(action.payload);
      return state;
    default:
      return state;
  }
}
