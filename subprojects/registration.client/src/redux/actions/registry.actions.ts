import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState, IRegistryRetrievalQuery, IRegisterRecordList } from '../store';

@Injectable()
export class RegistryActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';
  static OPEN_RECORDS = 'OPEN_RECORDS';
  static OPEN_RECORDS_SUCCESS = 'OPEN_RECORDS_SUCCESS';
  static OPEN_RECORDS_ERROR = 'OPEN_RECORDS_ERROR';
  static BULK_REGISTER_RECORD = 'BULK_REGISTER_RECORD';
  static BULK_REGISTER_RECORD_SUCCESS = 'BULK_REGISTER_RECORD_SUCCESS';
  static BULK_REGISTER_RECORD_ERROR = 'BULK_REGISTER_RECORD_ERROR';
  static DELETE_RECORD = 'DELETE_RECORD';
  static DELETE_RECORD_SUCCESS = 'DELETE_RECORD_SUCCESS';
  static DELETE_RECORD_ERROR = 'DELETE_RECORD_ERROR';
  static bulkRegisterRecordAction = createAction(RegistryActions.BULK_REGISTER_RECORD,
    (payload: IRegisterRecordList) => (payload));
  static bulkRegisterRecordErrorAction = createAction(RegistryActions.BULK_REGISTER_RECORD_ERROR,
    (payload: any) => (payload));
  static openRecordsAction = createAction(RegistryActions.OPEN_RECORDS,
    (payload: IRegistryRetrievalQuery) => (payload));
  static openRecordsSuccessAction = createAction(RegistryActions.OPEN_RECORDS_SUCCESS,
    (temporary: boolean, data: any) => ({ temporary, data }));
  static openRecordsErrorAction = createAction(RegistryActions.OPEN_RECORDS_ERROR,
    (payload: any) => (payload));
  static deleteRecordAction = createAction(RegistryActions.DELETE_RECORD,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static deleteRecordSuccessAction = createAction(RegistryActions.DELETE_RECORD_SUCCESS);
  static deleteRecordErrorAction = createAction(RegistryActions.DELETE_RECORD_ERROR);

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

  openRecords(payload: IRegistryRetrievalQuery) {
    this.ngRedux.dispatch(RegistryActions.openRecordsAction(payload));
  }

  deleteRecord(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistryActions.deleteRecordAction(temporary, id));
  }

  bulkRegister(payload: IRegisterRecordList) {
    this.ngRedux.dispatch(RegistryActions.bulkRegisterRecordAction(payload));
  }

}
