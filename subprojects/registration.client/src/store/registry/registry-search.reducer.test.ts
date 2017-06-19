import { Iterable } from 'immutable';
import { registrySearchReducer } from './registry-search.reducer';
import { RegistrySearchActions } from '../../actions';
import { IRegistrySearchRecord, HitlistType, IHitlistData, IHitlistInfo, IHitlistRetrieveInfo, ISearchRecords, INITIAL_STATE } from './registry-search.types';

describe('registry search reducer', () => {
  let initState: IRegistrySearchRecord;

  beforeEach(() => {
    initState = INITIAL_STATE;
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should update hitlist.rows on OPEN_HITLISTS_SUCCESS', () => {
    const rows: IHitlistData[] = [
      { hitlistType: HitlistType.TEMP, name: 'temp1', isPublic: false },
      { hitlistType: HitlistType.TEMP, name: 'temp2', isPublic: false }];
    const nextState = registrySearchReducer(
      initState,
      RegistrySearchActions.openHitlistsSuccessAction(rows)
    );
    expect(nextState.hitlist.rows).toEqual(rows);
  });

  it('should update hitlist.currentHitlistId on RETRIEVE_HITLIST', () => {
    const data: IHitlistRetrieveInfo = { id: 0, type: 'Retrieve' };
    const nextState = registrySearchReducer(
      initState,
      RegistrySearchActions.retrieveHitlistAction(false, data)
    );
    expect(nextState.hitlist.currentHitlistId).toEqual(data.id);
  });
});
