import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, ConfigurationActions, SessionActions } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';

const BASE_URL = '/api';

@Injectable()
export class SessionEpics {
  constructor(private http: Http) { }

  handleLoginUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return this.http.post(`${BASE_URL}/auth/login`, payload)
          .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
          .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
      });
  }

  handleLoginUserSuccess = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER_SUCCESS)
      .mergeMap<IPayloadAction>(() => {
        return this.http.get(`${BASE_URL}/CustomTables`)
          .map(result => ConfigurationActions.customTablesSuccessAction(result.json()))
          .catch(error => Observable.of(ConfigurationActions.customTablesErrorAction()));
      });
  }
}
