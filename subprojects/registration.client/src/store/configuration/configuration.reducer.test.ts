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
    const tableId = 'table1';
    const previousState = initState;
    const nextState = configurationReducer(
      previousState,
      ConfigurationActions.openTableAction(tableId));
    expect(nextState.customTables[tableId]).toEqual({ rows: [] });
  });

  it('should update rows on OPEN_TABLE_SUCCESS', () => {
    const tableId = 'table1';
    const data = { rows: [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }] };
    const previousState = initState;
    const nextState = configurationReducer(
      previousState,
      ConfigurationActions.openTableSuccessAction({ tableId, data }));
    expect(nextState.customTables[tableId]).toEqual(data);
  });

  it('should ignore OPEN_TABLE_ERROR', () => {
    const tableId = 'table1';
    const error = 'opening table failed';    
    const nextState = configurationReducer(
      initState,
      ConfigurationActions.openTableErrorAction({ tableId, error })
    );
    expect(nextState).toEqual(initState);
  });  
});
