import { Iterable } from 'immutable';
import { ISessionRecord, ILookupData } from './session.types';
import { sessionReducer } from './session.reducer';
import { INITIAL_STATE } from './session.initial-state';
import { SessionActions, RecordDetailActions, RegistryActions } from '../../actions';

describe('Session Reducer', () => {
  let initState: ISessionRecord;

  beforeEach(() => {
    initState = sessionReducer(undefined, { type: 'TEST_INIT'});
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should set loading to true on LOGIN_USER_PENDING', () => {
    const nextState = sessionReducer(
      initState,
      { type: SessionActions.LOGIN_USER });
    expect(nextState.get('isLoading')).toBeTruthy;
    expect(nextState.get('token')).toEqual(null);
  });

  it('should save the user token on LOGIN_USER_SUCCESS', () => {
    const nextState = sessionReducer(
      initState,
      {
        type: SessionActions.LOGIN_USER_SUCCESS,
        payload: { token: 1234 }
      }
    );
    expect(nextState.get('isLoading')).toBeFalsy;
    expect(nextState.get('hasError')).toBeFalsy;
    expect(nextState.get('token')).toEqual(1234);
  });

  it('should flag an error on LOGIN_USER_ERROR', () => {
    const nextState = sessionReducer(
      initState,
      { type: SessionActions.LOGIN_USER_ERROR });
    expect(nextState.get('isLoading')).toBeFalsy;
    expect(nextState.get('hasError')).toBeTruthy;
  });

  it('should clear user data on LOGOUT_USER', () => {
    const nextState = sessionReducer(
      initState,
      { type: SessionActions.LOGOUT_USER });
    expect(nextState.get('isLoading')).toBeTruthy;
    expect(nextState.get('hasError')).toBeFalsy;
    expect(nextState.get('token')).toEqual(null);
  });

  it('should update lookups on LOAD_LOOKUPS_SUCCESS', () => {
    const data: ILookupData = { users: [{ id: 1, name: 'user one' }] };
    const previousState = initState;
    const nextState = sessionReducer(
      previousState,
      SessionActions.loadLookupsSuccessAction(data));
    expect(nextState.lookups).toEqual(data);
  });

  it ('should clear state on errors with 404 status', () => {
    const error = { status: 404 };
    const previousState = initState;
    let nextState = sessionReducer(
      previousState,
      RegistryActions.openRecordsErrorAction(error));
    expect(nextState).toEqual(INITIAL_STATE);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.retrieveRecordErrorAction(error));
    expect(nextState).toEqual(INITIAL_STATE);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.loadStructureErrorAction(error));
    expect(nextState).toEqual(INITIAL_STATE);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.saveRecordErrorAction(error));
    expect(nextState).toEqual(INITIAL_STATE);
  });

  it ('should do nothing on errors with non-404 error', () => {
    const error = 'error';
    const previousState = initState;
    let nextState = sessionReducer(
      previousState,
      RegistryActions.openRecordsErrorAction(error));
    expect(nextState).toEqual(previousState);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.retrieveRecordErrorAction(error));
    expect(nextState).toEqual(previousState);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.loadStructureErrorAction(error));
    expect(nextState).toEqual(previousState);
    nextState = sessionReducer(
      previousState,
      RecordDetailActions.saveRecordErrorAction(error));
    expect(nextState).toEqual(previousState);
  });
});
