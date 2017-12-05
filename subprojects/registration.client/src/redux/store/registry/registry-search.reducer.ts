import { RegistrySearchActions, SessionActions, IPayloadAction } from '../../actions';
import { IRegistrySearchRecord, ISearchRecords, IHitlistRetrieveInfo, INITIAL_SEARCH_STATE } from './registry-search.types';
import { notify, notifyError, notifySuccess } from '../../../common';

export function registrySearchReducer(
  state: IRegistrySearchRecord = INITIAL_SEARCH_STATE,
  action: IPayloadAction
): IRegistrySearchRecord {
  switch (action.type) {
    case RegistrySearchActions.OPEN_HITLISTS_SUCCESS:
      let a1 = action as ReduxActions.Action<ISearchRecords>;
      return state.updateIn(['hitlist', 'rows'], () => a1.payload);
    case RegistrySearchActions.SEARCH_OPTION_CHANGED:
      let a2 = action as ReduxActions.Action<ISearchRecords>;
      return state.update('highLightSubstructure', () => a2.payload);
    case SessionActions.LOGOUT_USER:
      return state.update('highLightSubstructure', () => false);
    case RegistrySearchActions.RETRIEVE_HITLIST_ERROR:
      notifyError(action.payload);
      return state;
    default:
      return state;
  }
}
