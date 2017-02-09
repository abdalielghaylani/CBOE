import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class RegActions {
  static IGNORE_ACTION = 'IGNORE_ACTION';
  static ignoreAction = createAction(RegActions.IGNORE_ACTION);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  ignore() {
    this.ngRedux.dispatch(RegActions.ignoreAction());
  }
}
