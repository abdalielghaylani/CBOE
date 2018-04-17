import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { Action } from 'redux';
import { IAppState } from '../store';

export interface IGridBaseAction extends Action {
  payload: { tableId: string };
}

export interface IGridRecordAction extends Action {
  payload: { tableId: string, recordId: string };
}

export interface IGridPullAction extends Action {
  payload: { tableId: string, startIndex?: number, recordCount?: number };
}

@Injectable()
export class GridActions {
  static CONFIGURE = 'CONFIGURE';
  static ADD_RECORD = 'ADD_RECORD';
  static DELETE_RECORD = 'DELETE_RECORD';
  static EDIT_RECORD = 'EDIT_RECORD';
  static GET_RECORDS = 'GET_RECORDS';
  static GET_ALL_RECORDS = 'GET_ALL_RECORDS';
  static configureAction = createAction(GridActions.CONFIGURE,
    (tableName: string) => ({ tableName }));
  static addRecordAction = createAction(GridActions.ADD_RECORD,
    (tableName: string) => ({ tableName }));
  static deleteRecordAction = createAction(GridActions.DELETE_RECORD,
    (tableName: string, recordId: string) => ({ tableName, recordId }));
  static editRecordAction = createAction(GridActions.EDIT_RECORD,
    (tableName: string, recordId: string) => ({ tableName, recordId }));
  static getRecordsAction = createAction(GridActions.GET_RECORDS,
    (tableName: string, startIndex: number, recordCount: number) => ({ tableName, startIndex, recordCount }));
  static getAllRecordsAction = createAction(GridActions.GET_ALL_RECORDS,
    (tableName: string) => ({ tableName }));

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

  getRecords(tableName: string, startIndex: number, recordCount: number) {
    this.ngRedux.dispatch(GridActions.getRecordsAction(tableName, startIndex, recordCount));
  }

  getAllRecords(tableName: string) {
    this.ngRedux.dispatch(GridActions.getAllRecordsAction(tableName));
  }
}
