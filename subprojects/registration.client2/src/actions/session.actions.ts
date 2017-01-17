import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class SessionActions {
  static CHECK_LOGIN = 'CHECK_LOGIN';
  static LOGIN_USER = 'LOGIN_USER';
  static LOGIN_USER_SUCCESS = 'LOGIN_USER_SUCCESS';
  static LOGIN_USER_ERROR = 'LOGIN_USER_ERROR';
  static LOGOUT_USER = 'LOGOUT_USER';
  static checkLoginAction = createAction(SessionActions.CHECK_LOGIN);
  static loginUserAction = createAction(SessionActions.LOGIN_USER);
  static loginUserSuccessAction = createAction(SessionActions.LOGIN_USER_SUCCESS);
  static logoutUserAction = createAction(SessionActions.LOGOUT_USER);
  static loginUserErrorAction = createAction(SessionActions.LOGIN_USER_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  checkLogin() {
    this.ngRedux.dispatch(SessionActions.checkLoginAction());
  }

  loginUser(credentials) {
    this.ngRedux.dispatch(SessionActions.loginUserAction(credentials));
  };

  logoutUser() {
    this.ngRedux.dispatch(SessionActions.logoutUserAction());
  };
} 
