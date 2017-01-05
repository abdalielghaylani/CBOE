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

  it('should open table  on OPEN_TABLE', () => {
    const previousValue = initState.tableId;
    const nextState = configurationReducer(
      initState,
      { type: ConfigurationActions.OPEN_TABLE, payload: { tableId: 'table' } });
    expect(nextState.tableId).toEqual('table');
  });
});
