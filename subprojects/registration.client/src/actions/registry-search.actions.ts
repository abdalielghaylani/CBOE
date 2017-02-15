import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IRecordDetail, IAppState } from '../store';

@Injectable()
export class RegistrySearchActions {
  static SEARCH_RECORDS = 'SEARCH_RECORDS';
  static SEARCH_RECORDS_SUCCESS = 'SEARCH_RECORDS_SUCCESS';
  static SEARCH_RECORDS_ERROR = 'SEARCH_RECORDS_ERROR';
  static RETRIEVE_QUERY_LIST = 'RETRIEVE_QUERY_LIST';
  static RETRIEVE_QUERY_LIST_SUCCESS = 'RETRIEVE_QUERY_LIST_SUCCESS';
  static RETRIEVE_QUERY_LIST_ERROR = 'RETRIEVE_QUERY_LIST_ERROR';
  static RETRIEVE_QUERY_FORM = 'RETRIEVE_QUERY_FORM';
  static RETRIEVE_QUERY_FORM_SUCCESS = 'RETRIEVE_QUERY_FORM_SUCCESS';
  static RETRIEVE_QUERY_FORM_ERROR = 'RETRIEVE_QUERY_FORM_ERROR';
  static searchRecordsAction = createAction(RegistrySearchActions.SEARCH_RECORDS,
    (temporary: boolean, history: boolean, id: Number) => (temporary));
  static searchRecordsSuccessAction = createAction(RegistrySearchActions.SEARCH_RECORDS_SUCCESS,
    (temporary: boolean, rows: any[]) => ({ temporary, rows }));
  static searchRecordsErrorAction = createAction(RegistrySearchActions.SEARCH_RECORDS_ERROR);
  static retrieveQueryListAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_LIST,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static retrieveQueryListSuccessAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_LIST_SUCCESS);
  static retrieveQueryListErrorAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_LIST_ERROR);
  static retrieveQueryFormAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static retrieveQueryFormSuccessAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM_SUCCESS);
  static retrieveQueryFormErrorAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  searchRecords(temporary: boolean, history: boolean, id: Number) {
    this.ngRedux.dispatch(RegistrySearchActions.searchRecordsAction(temporary, history, id));
  }

  searchRecordsSuccess(temporary: boolean, rows: any[]) {
    this.ngRedux.dispatch(RegistrySearchActions.searchRecordsSuccessAction(temporary, rows));
  }

  searchRecordsError() {
    this.ngRedux.dispatch(RegistrySearchActions.searchRecordsErrorAction());
  }

  retrieveQueryList(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryListAction(temporary, id));
  }

  retrieveQueryListSuccess(data: any[]) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryListSuccessAction(data));
  }

  retrieveQueryListError() {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryListErrorAction());
  }

  retrieveQueryForm(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormAction(temporary, id));
  }

  retrieveQueryFormSuccess(data: any[]) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormSuccessAction(data));
  }

  retrieveQueryFormError() {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormErrorAction());
  }

}
