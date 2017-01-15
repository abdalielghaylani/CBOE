import { Iterable } from 'immutable';
import { RegistryFactory } from './registry.initial-state';
import { registryReducer } from './registry.reducer';
import { RegistryActions, SessionActions } from '../../actions';
import { IRegistryRecord } from './registry.types';

describe('registry reducer', () => {
  let initState: IRegistryRecord;

  beforeEach(() => {
    initState = RegistryFactory();
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should do nothing on OPEN_RECORDS_ERROR', () => {
    const previousValue = initState.temporary;
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsErrorAction()
    );
    expect(nextState).toEqual(initState);
  });

  it('should send data on OPEN_RECORDS_SUCCESS', () => {
    let data = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const previousValue = initState.temporary;
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsSuccessAction(data)
    );
    expect(nextState.rows).toEqual(data);
  });

  it('should clear rows on OPEN_RECORDS', () => {
    const previousValue = initState.temporary;
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsAction(true)
    );
    expect(nextState.rows).toEqual([]);
  });
});
