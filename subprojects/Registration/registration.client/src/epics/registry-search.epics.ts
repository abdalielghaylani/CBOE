import { Injectable } from '@angular/core';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { Action, MiddlewareAPI } from 'redux';
import { createAction } from 'redux-actions';
import { Epic, ActionsObservable, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { apiUrlPrefix } from '../configuration';
import { notify, notifySuccess } from '../common';
import { HttpService } from '../services';
import { IPayloadAction, RegistrySearchActions, RegistryActions, SessionActions, IGridPullAction } from '../redux';
import { IAppState, IHitlistData, IHitlistRetrieveInfo, IQueryData } from '../redux';
import { HitlistType } from '../redux/store/registry/registry-search.types';

@Injectable()
export class RegistrySearchEpics {
  constructor(private http: HttpService) { }

  handleRegistrySearchActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleOpenHitlists,
      this.handleDeleteHitlist,
      this.handleUpdateHitlist
    )(action$, store);
  }

  private handleOpenHitlists: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean }>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.OPEN_HITLISTS)
      .mergeMap(({ payload }) => {
        return this.http.get(`${apiUrlPrefix}hitlists${ payload.temporary ? '?temp=true' : ''}`)
          .map(result => {
            return RegistrySearchActions.openHitlistsSuccessAction(result.json());
          })
          .catch(error => Observable.of(RegistrySearchActions.openHitlistsErrorAction(error)));
      });
  }

  private handleDeleteHitlist: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number }>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.DELETE_HITLIST)
      .mergeMap(({ payload }) => {
        return this.http.delete(`${apiUrlPrefix}hitlists/${payload.id}${ payload.temporary ? '?temp=true' : ''}`)
          .map(result => {
            notifySuccess('The selected hitlist was deleted successfully!', 5000);
            return RegistrySearchActions.openHitlistsAction(payload.temporary);
          })
          .catch(error => Observable.of(RegistrySearchActions.deleteHitlistErrorAction(error)));
      });
  }

  private handleUpdateHitlist: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, data: IHitlistData }>>) => {
    return action$.filter(({ type }) => type === RegistrySearchActions.UPDATE_HITLIST)
      .mergeMap(({ payload }) => {
        return this.http.put(`${apiUrlPrefix}hitlists/${payload.data.hitlistId}${ payload.temporary ? '?temp=true' : ''}`, payload.data)
          .map(result => {
            notifySuccess(`The selected hitlist was ${payload.data.hitlistType === HitlistType.SAVED ? 'saved' : 'updated'} successfully!`, 5000);
            return RegistrySearchActions.openHitlistsAction(payload.temporary);
          })
          .catch(error => Observable.of(RegistrySearchActions.updateHitlistErrorAction()));
      });
  }

}
