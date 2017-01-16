import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class RegistryActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';
  static OPEN_RECORDS = 'OPEN_RECORDS';
  static OPEN_RECORDS_SUCCESS = 'OPEN_RECORDS_SUCCESS';
  static OPEN_RECORDS_ERROR = 'OPEN_RECORDS_ERROR';
  static RETRIEVE_RECORD_SUCCESS = 'RETRIEVE_RECORD_SUCCESS';
  static RETRIEVE_RECORD_ERROR = 'RETRIEVE_RECORD_ERROR';
  static openRecordsAction = createAction(RegistryActions.OPEN_RECORDS,
    (temporary: boolean) => (temporary));
  static openRecordsSuccessAction = createAction(RegistryActions.OPEN_RECORDS_SUCCESS,
    (temporary: boolean, rows: any[]) => ({ temporary, rows }));
  static openRecordsErrorAction = createAction(RegistryActions.OPEN_RECORDS_ERROR);
  static retrieveRecordSuccessAction = createAction(RegistryActions.RETRIEVE_RECORD_SUCCESS,
    (temporary: boolean, data: string) => ({ temporary, data }));
  static retrieveRecordErrorAction = createAction(RegistryActions.RETRIEVE_RECORD_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  create() {
    this.ngRedux.dispatch({ type: RegistryActions.OPEN_CREATE });
  }

  edit() {
    this.ngRedux.dispatch({ type: RegistryActions.OPEN_EDIT });
  }

  search() {
    this.ngRedux.dispatch({ type: RegistryActions.SEARCH });
  }

  openRecords(temporary: boolean) {
    this.ngRedux.dispatch(RegistryActions.openRecordsAction(temporary));
  }

  retrieveRecordSuccess(temporary: boolean, data: string) {
    this.ngRedux.dispatch(RegistryActions.retrieveRecordSuccessAction(temporary, data));
  }

  retrieveRecordError() {
    this.ngRedux.dispatch(RegistryActions.retrieveRecordErrorAction());
  }
}
