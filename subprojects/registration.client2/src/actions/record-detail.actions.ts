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
  static LOAD_STRUCTURE = 'LOAD_STRUCTURE';
  static saveAction = createAction(RecordDetailActions.SAVE);
  static saveSuccessAction = createAction(RecordDetailActions.SAVE_SUCCESS);
  static saveErrorAction = createAction(RecordDetailActions.SAVE_ERROR);
  static updateAction = createAction(RecordDetailActions.UPDATE);
  static updateSuccessAction = createAction(RecordDetailActions.UPDATE_SUCCESS);
  static updateErrorAction = createAction(RecordDetailActions.UPDATE_ERROR);
  static registerAction = createAction(RecordDetailActions.REGISTER);
  static registerSuccessAction = createAction(RecordDetailActions.REGISTER_SUCCESS);
  static registerErrorAction = createAction(RecordDetailActions.REGISTER_ERROR);
  static loadStructureAction = createAction(RecordDetailActions.LOAD_STRUCTURE);

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

  loadStructure(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.loadStructureAction(data));
  }
}
