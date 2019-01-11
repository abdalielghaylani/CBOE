import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState, IHitlistData, IHitlistRetrieveInfo, IQueryData } from '../store';

@Injectable()
export class RegistrySearchActions {
  static RETRIEVE_QUERY_FORM = 'RETRIEVE_QUERY_FORM';
  static RETRIEVE_QUERY_FORM_SUCCESS = 'RETRIEVE_QUERY_FORM_SUCCESS';
  static RETRIEVE_QUERY_FORM_ERROR = 'RETRIEVE_QUERY_FORM_ERROR';
  static OPEN_HITLISTS = 'OPEN_HITLISTS';
  static OPEN_HITLISTS_SUCCESS = 'OPEN_HITLISTS_SUCCESS';
  static OPEN_HITLISTS_ERROR = 'OPEN_HITLISTS_ERROR';
  static DELETE_HITLIST = 'DELETE_HITLIST';
  static DELETE_HITLIST_ERROR = 'DELETE_HITLIST_ERROR';
  static UPDATE_HITLIST = 'UPDATE_HITLIST';
  static UPDATE_HITLIST_ERROR = 'UPDATE_HITLIST_ERROR';
  static SAVE_HITLISTS = 'SAVE_HITLISTS';
  static SAVE_HITLISTS_ERROR = 'SAVE_HITLISTS_ERROR';
  static SEARCH_OPTION_CHANGED = 'SEARCH_OPTION_CHANGED';

  static searchOptionChangedAction = createAction(RegistrySearchActions.SEARCH_OPTION_CHANGED,
    (highLight: boolean) => ( highLight ));
  static openHitlistsAction = createAction(RegistrySearchActions.OPEN_HITLISTS,
    (temporary: boolean) => ({ temporary }));
  static openHitlistsSuccessAction = createAction(RegistrySearchActions.OPEN_HITLISTS_SUCCESS);
  static openHitlistsErrorAction = createAction(RegistrySearchActions.OPEN_HITLISTS_ERROR);
  static updateHitlistAction = createAction(RegistrySearchActions.UPDATE_HITLIST,
    (temporary: boolean, data: IHitlistData) => ({ temporary, data }));
  static updateHitlistErrorAction = createAction(RegistrySearchActions.UPDATE_HITLIST_ERROR);
  static deleteHitlistAction = createAction(RegistrySearchActions.DELETE_HITLIST,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static deleteHitlistErrorAction = createAction(RegistrySearchActions.DELETE_HITLIST_ERROR);
  static retrieveQueryFormAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static retrieveQueryFormSuccessAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM_SUCCESS);
  static retrieveQueryFormErrorAction = createAction(RegistrySearchActions.RETRIEVE_QUERY_FORM_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  retrieveQueryForm(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormAction(temporary, id));
  }

  retrieveQueryFormSuccess(data: any[]) {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormSuccessAction(data));
  }

  retrieveQueryFormError() {
    this.ngRedux.dispatch(RegistrySearchActions.retrieveQueryFormErrorAction());
  }

  openHitlists(temporary: boolean) {
    this.ngRedux.dispatch(RegistrySearchActions.openHitlistsAction(temporary));
  }

  seachOptionChanged(highLight: boolean) {
    this.ngRedux.dispatch(RegistrySearchActions.searchOptionChangedAction(highLight));
  }

  deleteHitlist(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RegistrySearchActions.deleteHitlistAction(temporary, id));
  }

  updateHitlist(temporary: boolean, data: IHitlistData) {
    this.ngRedux.dispatch(RegistrySearchActions.updateHitlistAction(temporary, data));
  }
}
