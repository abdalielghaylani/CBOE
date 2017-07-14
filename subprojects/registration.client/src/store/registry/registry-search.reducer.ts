import { RegistrySearchActions, IPayloadAction } from '../../actions';
import { IRegistrySearchRecord, ISearchRecords, IHitlistRetrieveInfo, INITIAL_SEARCH_STATE } from './registry-search.types';
import { notify, notifyError, notifySuccess } from '../../common';

export function registrySearchReducer(
  state: IRegistrySearchRecord = INITIAL_SEARCH_STATE,
  action: IPayloadAction
): IRegistrySearchRecord {
  switch (action.type) {
    case RegistrySearchActions.OPEN_HITLISTS_SUCCESS:
      let a1 = action as ReduxActions.Action<ISearchRecords>;
      return state.updateIn(['hitlist', 'rows'], () => a1.payload);
    case RegistrySearchActions.RETRIEVE_HITLIST_ERROR:
      notifyError(action.payload);
      return state;
    default:
      return state;
  }
}
