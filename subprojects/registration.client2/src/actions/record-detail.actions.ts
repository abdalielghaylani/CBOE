import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';


@Injectable()
export class RecordDetailActions {
  static SAVE = 'SAVE';
  static SAVE_SUCCESS = 'SAVE_SUCCESS';
  static SAVE_ERROR = 'SAVE_ERROR';
  static UPDATE = 'UPDATE';
  static UPDATE_SUCCESS = 'UPDATE_SUCCESS';
  static UPDATE_ERROR = 'UPDATE_ERROR';
  static REGISTER = 'REGISTER';
  static REGISTER_SUCCESS = 'REGISTER_SUCCESS';
  static REGISTER_ERROR = 'REGISTER_ERROR';
  static saveAction = createAction(RecordDetailActions.SAVE);
  static updateAction = createAction(RecordDetailActions.UPDATE);
  static registerAction = createAction(RecordDetailActions.REGISTER);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  save(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.saveAction(data));
  }

  update(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.updateAction(data));
  }

  register(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.registerAction(data));
  }
}
