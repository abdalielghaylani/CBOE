import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { Http } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { IPayloadAction, RegistrySearchActions, SessionActions, IGridPullAction } from '../actions';
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

const BASE_URL = `${basePath}api`;

@Injectable()
export class RegistrySearchEpics {
  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleRegistrySearchActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleopenHitlists,
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
        // return RegistrySearchActions.openHitlistsSuccessAction([{id:1,Name:'Temp'},{id:1,Name:'Temp2'}]);
      });
  }



}
