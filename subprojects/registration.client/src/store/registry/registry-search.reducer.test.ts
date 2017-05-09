import { Iterable } from 'immutable';
import { registrySearchReducer } from './registry-search.reducer';
import { RegistrySearchActions } from '../../actions';
import { IRegistrySearchRecord, ISearchRecords, INITIAL_STATE } from './registry-search.types';

describe('registry search reducer', () => {
  let initState: IRegistrySearchRecord;

  beforeEach(() => {
    initState = INITIAL_STATE;
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should update hitlist.rows on OPEN_HITLISTS_SUCCESS', () => {
    const rows = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const nextState = registrySearchReducer(
      initState,
      RegistrySearchActions.openHitlistsSuccessAction(rows)
    );
    expect(nextState.hitlist.rows).toEqual(rows);
  });

  it('should update hitlist.currentHitlistInfo on RETRIEVE_HITLIST', () => {
    const data = { id: 0, type: 0 };
    const nextState = registrySearchReducer(
      initState,
      RegistrySearchActions.retrieveHitlistAction(data)
    );
    expect(nextState.hitlist.currentHitlistInfo).toEqual(data);
  });
});
