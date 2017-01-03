import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class GridActions {
  static CONFIGURE = 'CONFIGURE';
  static ADD_RECORD = 'ADD_RECORD';
  static DELETE_RECORD = 'DELETE_RECORD';
  static EDIT_RECORD = 'EDIT_RECORD';
  static configureAction = createAction(GridActions.CONFIGURE, tableName => ({ tableName }));
  static addRecordAction = createAction(GridActions.ADD_RECORD, tableName => ({ tableName }));
  static deleteRecordAction = createAction(GridActions.DELETE_RECORD, (tableName, recordId) => ({ tableName, recordId }));
  static editRecordAction = createAction(GridActions.EDIT_RECORD, (tableName, recordId) => ({ tableName, recordId }));

  constructor(private ngRedux: NgRedux<IAppState>) { }

  configure(tableName: string) {
    this.ngRedux.dispatch(GridActions.configureAction(tableName));
  }

  addRecord(tableName: string) {
    this.ngRedux.dispatch(GridActions.addRecordAction(tableName));
  }

  deleteRecord(tableName: string, recordId: string) {
    this.ngRedux.dispatch(GridActions.deleteRecordAction(tableName, recordId));
  }

  editRecord(tableName: string, recordId: string) {
    this.ngRedux.dispatch(GridActions.editRecordAction(tableName, recordId));
  }
}
