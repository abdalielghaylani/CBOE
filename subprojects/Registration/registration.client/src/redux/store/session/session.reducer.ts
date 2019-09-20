import { IPayloadAction } from '../../actions';
import { RegistryActions, RecordDetailActions, RegistrySearchActions, SessionActions } from '../../actions';
import { ISessionRecord } from './session.types';
import {
  INITIAL_SESSION_STATE,
  INITIAL_USER_STATE,
  UserFactory,
} from './session.initial-state';
import { notifyError, notifyException } from '../../../common';

function handleError(currentState: ISessionRecord, error): ISessionRecord {
  if (error.url && error.url.toLowerCase().indexOf('index.html?returnurl') > 0) {
    return INITIAL_SESSION_STATE;
  }
  notifyException('The operation failed unexpectedly', error, 5000);
  return currentState;
}

export function sessionReducer(
  state: ISessionRecord = INITIAL_SESSION_STATE,
  action: IPayloadAction): ISessionRecord {

  switch (action.type) {
    case SessionActions.LOGIN_USER:
      return state.merge({
        token: null,
        user: INITIAL_USER_STATE,
        hasError: false,
        isLoading: false,
      });

    case SessionActions.LOGIN_USER_SUCCESS:
      return state.merge({
        token: action.payload.token,
        user: UserFactory(action.payload.profile),
        hasError: false,
        isLoading: false,
      });

    case SessionActions.LOGIN_USER_ERROR:
      return state.merge({
        token: null,
        user: INITIAL_USER_STATE,
        hasError: true,
        isLoading: false,
      });

    case SessionActions.CHECK_LOGIN:
    case SessionActions.LOGOUT_USER:
      return INITIAL_SESSION_STATE;

    case SessionActions.LOAD_LOOKUPS_SUCCESS:
      return state.update('lookups', () => action.payload);

    case RegistryActions.BULK_REGISTER_RECORD_ERROR:
    case RecordDetailActions.LOAD_STRUCTURE_ERROR:
    case RecordDetailActions.UPDATE_RECORD_ERROR:
      return handleError(state, action.payload);


    case RecordDetailActions.RETRIEVE_RECORD_ERROR:
      if (action.payload.status !== 404) {
        return handleError(state, action.payload);
      }
      return state;

    case RecordDetailActions.SAVE_RECORD_ERROR:
      return handleError(state, action.payload.error);

    default:
      return state;
  }
}
