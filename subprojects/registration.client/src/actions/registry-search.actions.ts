import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

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
  static OPEN_HITLISTS = 'OPEN_HITLISTS';
  static OPEN_HITLISTS_SUCCESS = 'OPEN_HITLISTS_SUCCESS';
  static OPEN_HITLISTS_ERROR = 'OPEN_HITLISTS_ERROR';
  static DELETE_HITLISTS = 'DELETE_HITLISTS';
  static DELETE_HITLISTS_ERROR = 'DELETE_HITLISTS_ERROR';
  static EDIT_HITLISTS = 'EDIT_HITLISTS';
  static EDIT_HITLISTS_ERROR = 'EDIT_HITLISTS_ERROR';
  static SAVE_HITLISTS = 'SAVE_HITLISTS';
  static SAVE_HITLISTS_ERROR = 'SAVE_HITLISTS_ERROR';
  static openHitlistsAction = createAction(RegistrySearchActions.OPEN_HITLISTS);
  static openHitlistsSuccessAction = createAction(RegistrySearchActions.OPEN_HITLISTS_SUCCESS);
  static openHitlistsErrorAction = createAction(RegistrySearchActions.OPEN_HITLISTS_ERROR);
  static editHitlistsAction = createAction(RegistrySearchActions.EDIT_HITLISTS);
  static editHitlistsErrorAction = createAction(RegistrySearchActions.EDIT_HITLISTS_ERROR);
  static saveHitlistsAction = createAction(RegistrySearchActions.SAVE_HITLISTS);
  static saveHitlistsErrorAction = createAction(RegistrySearchActions.SAVE_HITLISTS_ERROR);
  static deleteHitlistsAction = createAction(RegistrySearchActions.DELETE_HITLISTS,
    (type: number, id: number) => ({ type, id }));
  static deleteHitlistsErrorAction = createAction(RegistrySearchActions.DELETE_HITLISTS_ERROR);
  static searchRecordsAction = createAction(RegistrySearchActions.SEARCH_RECORDS,
    (temporary: boolean, history: boolean, id: Number) => ({ temporary, history, id }));
  static searchRecordsSuccessAction = createAction(RegistrySearchActions.SEARCH_RECORDS_SUCCESS,
    (temporary: boolean, rows: any[]) => ({ temporary, rows }));
  static searchRecordsErrorAction = createAction(RegistrySearchActions.SEARCH_RECORDS_ERROR);
  static retrieveQueryListAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_LIST);
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

  retrieveQueryForm(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormAction(temporary, id));
  }

  retrieveQueryFormSuccess(data: any[]) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormSuccessAction(data));
  }

  retrieveQueryFormError() {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormErrorAction());
  }

  openHitlists() {
    this.ngRedux.dispatch(RegistrySearchActions.openHitlistsAction());
  }

  deleteHitlists(type: number, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.deleteHitlistsAction(type, id));
  }

  editHitlists(data) {
    this.ngRedux.dispatch(RegistrySearchActions.editHitlistsAction(data));
  };

  saveHitlists(data) {
    this.ngRedux.dispatch(RegistrySearchActions.saveHitlistsAction(data));
  };

  retrieveHitlist(data) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryListAction(data));
  };
}
