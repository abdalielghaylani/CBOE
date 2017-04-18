import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { Http } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { IPayloadAction, RegistrySearchActions, RegistryActions, SessionActions, IGridPullAction } from '../actions';
import { Action, MiddlewareAPI } from 'redux';
import { createAction } from 'redux-actions';
import { Epic, ActionsObservable, combineEpics } from 'redux-observable';
import { IAppState } from '../store';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { basePath } from '../configuration';
import { notify, notifySuccess } from '../common';

const BASE_URL = `${basePath}api`;

@Injectable()
export class RegistrySearchEpics {
  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleRegistrySearchActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleopenHitlists,
      this.handledeleteHitlists,
      this.handleEditHitlist,
      this.handleSaveHitlist,
      this.handleRetrieveHitlist
    )(action$, store);
  }

  private handleopenHitlists: Epic = (action$: Observable<ReduxActions.Action<any>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.OPEN_HITLISTS)
      .mergeMap(() => {
        return this.http.get(`${BASE_URL}/search/hitlists`)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RegistrySearchActions.openHitlistsSuccessAction(result.json());
          })
          .catch(error => Observable.of(RegistrySearchActions.openHitlistsErrorAction(error)));
      });
  }

  private handledeleteHitlists: Epic = (action$: Observable<ReduxActions.Action<{ type: number, id: number }>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.DELETE_HITLISTS)
      .mergeMap(({ payload }) => {
        return this.http.delete(`${BASE_URL}/search/hitlists/` + payload.id + `/` + payload.type)
          .map(result => {
            if (result.url.indexOf('index.html') > 0) {
              SessionActions.logoutUserAction();
            } else {
              notifySuccess('The selected hitlist deleted successfully!', 5000);
              return RegistrySearchActions.openHitlistsAction();
            }
          })
          .catch(error => Observable.of(RegistrySearchActions.deleteHitlistsErrorAction(error)));
      });
  }

  handleEditHitlist = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.EDIT_HITLISTS)
      .mergeMap(({ payload }) => {
        return this.http.put(`${BASE_URL}/search/hitlists`, payload)
          .map(result => {
            notifySuccess('The selected hitlist updated successfully!', 5000);
            return RegistrySearchActions.openHitlistsAction();
          })
          .catch(error => Observable.of(RegistrySearchActions.editHitlistsErrorAction()));
      });
  }

  handleSaveHitlist = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.SAVE_HITLISTS)
      .mergeMap(({ payload }) => {
        return this.http.post(`${BASE_URL}/search/hitlists`, payload)
          .map(result => {
            notifySuccess('The selected hitlist saved successfully!', 5000);
            return RegistrySearchActions.openHitlistsAction();
          })
          .catch(error => Observable.of(RegistrySearchActions.saveHitlistsErrorAction()));
      });
  }

  private handleRetrieveHitlist: Epic = (action$: Observable<ReduxActions.Action<any>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.RETRIEVE_QUERY_LIST)
      .mergeMap(({ payload }) => {
        // restoreType 0 -: Restore hitlist
        // restoreType 1 -: Restore by union/intersect/substract
        if (payload.type === 0) {
          return this.http.get(`${BASE_URL}/search/restorehitlists/` + payload.HitlistID + `/` + payload.HitlistType)
            .map(result => {
              return result.url.indexOf('index.html') > 0
                ? SessionActions.logoutUserAction()
                : RegistryActions.openRecordsSuccessAction(result.json());
            })
            .catch(error => Observable.of(RegistrySearchActions.retrieveQueryListErrorAction(error)));
        }
        if (payload.type === 1) {
          return this.http.post(`${BASE_URL}/search/restorehitlistsactions`, payload.data)
            .map(result => {
              return result.url.indexOf('index.html') > 0
                ? SessionActions.logoutUserAction()
                : RegistryActions.openRecordsSuccessAction(result.json());
            })
            .catch(error => Observable.of(RegistrySearchActions.retrieveQueryListErrorAction(error)));
        }
      });
  }

}
