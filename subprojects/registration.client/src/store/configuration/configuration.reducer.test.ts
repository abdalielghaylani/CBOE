import { Iterable } from 'immutable';
import { ConfigurationFactory } from './configuration.initial-state';
import { configurationReducer } from './configuration.reducer';
import { ConfigurationActions, SessionActions } from '../../actions';
import { IConfigurationRecord } from './configuration.types';

describe('configuration reducer', () => {
  let initState: IConfigurationRecord;

  beforeEach(() => {
    initState = ConfigurationFactory();
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should update table and clear rows on OPEN_TABLE', () => {
    const data = 'table';
    const previousState = initState;
    const nextState = configurationReducer(
      previousState,
      ConfigurationActions.openTableAction(data));
    expect(nextState.tableId).toEqual(data);
    expect(nextState.rows).toEqual([]);
  });

  it('should update rows on OPEN_TABLE_SUCCESS', () => {
    const data = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const previousState = initState;
    const nextState = configurationReducer(
      previousState,
      ConfigurationActions.openTableSuccessAction(data));
    expect(nextState.rows).toEqual(data);
  });

  it('should ignore OPEN_TABLE_ERROR', () => {
    const nextState = configurationReducer(
      initState,
      ConfigurationActions.openTableErrorAction('error')
    );
    expect(nextState).toEqual(initState);
  });  
});
