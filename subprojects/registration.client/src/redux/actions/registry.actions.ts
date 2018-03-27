import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState, IRegistryRetrievalQuery, IRegisterRecordList } from '../store';

@Injectable()
export class RegistryActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';
  static BULK_REGISTER_RECORD = 'BULK_REGISTER_RECORD';
  static CLEAR_BULK_REGISTER_RECORD = 'CLEAR_BULK_REGISTER_RECORD';
  static BULK_REGISTER_RECORD_SUCCESS = 'BULK_REGISTER_RECORD_SUCCESS';
  static BULK_REGISTER_RECORD_ERROR = 'BULK_REGISTER_RECORD_ERROR';
  static CLEAR_RESPONSE = 'CLEAR_RESPONSE';
  static UPDATE_LIST_DATA = 'UPDATE_LIST_DATA';
  static clearResponseAction = createAction(RegistryActions.CLEAR_RESPONSE);
  static clearBulkRegisterRecordAction = createAction(RegistryActions.CLEAR_BULK_REGISTER_RECORD);
  static bulkRegisterRecordAction = createAction(RegistryActions.BULK_REGISTER_RECORD,
    (payload: IRegisterRecordList) => (payload));
  static bulkRegisterRecordSuccessAction = createAction(RegistryActions.BULK_REGISTER_RECORD_SUCCESS,
    (payload: any) => (payload));
  static bulkRegisterRecordErrorAction = createAction(RegistryActions.BULK_REGISTER_RECORD_ERROR,
    (payload: any) => (payload));

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

  bulkRegister(payload: IRegisterRecordList) {
    this.ngRedux.dispatch(RegistryActions.bulkRegisterRecordAction(payload));
  }

  clearBulkRrgisterStatus() {
    this.ngRedux.dispatch(RegistryActions.clearBulkRegisterRecordAction());
  }

  clearResponse() {
    this.ngRedux.dispatch(RegistryActions.clearResponseAction());
  }
}
