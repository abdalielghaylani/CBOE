import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class SessionActions {
  static LOGIN_USER = 'LOGIN_USER';
  static LOGIN_USER_SUCCESS = 'LOGIN_USER_SUCCESS';
  static LOGIN_USER_ERROR = 'LOGIN_USER_ERROR';
  static LOGOUT_USER = 'LOGOUT_USER';
  static loginUserAction = createAction(SessionActions.LOGIN_USER);
  static loginUserSuccessAction = createAction(SessionActions.LOGIN_USER_SUCCESS);
  static loginUserErrorAction = createAction(SessionActions.LOGIN_USER_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) {}

  loginUser(credentials) {
    this.ngRedux.dispatch({
      type: SessionActions.LOGIN_USER,
      payload: credentials,
    });
  };

  logoutUser() {
    this.ngRedux.dispatch({ type: SessionActions.LOGOUT_USER });
  };
} 
