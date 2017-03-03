import { RegistrySearchActions, IPayloadAction } from '../../actions';
import { IRegistrySearchRecord, ISearchRecords, INITIAL_STATE } from './registry-search.types';

export function registrySearchReducer(
  state: IRegistrySearchRecord = INITIAL_STATE,
  action: IPayloadAction
): IRegistrySearchRecord {


  switch (action.type) {
    case RegistrySearchActions.OPEN_HITLISTS_SUCCESS:
      let a2 = action as ReduxActions.Action<ISearchRecords>;
      return state.updateIn(['hitlist', 'rows'], () => a2.payload);
    default:
      return state;
  }
}
