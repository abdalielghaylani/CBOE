import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, SessionActions } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { basePath } from '../configuration';

const BASE_URL = `${basePath}api`;

@Injectable()
export class SessionEpics {
  constructor(private http: Http) { }

  handleLoginUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER)
      .mergeMap(({ payload }) => {
        return this.http.post(`${BASE_URL}/auth/login`, payload)
          .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
          .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
      });
  }

  handleLoginUserSuccess = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER_SUCCESS)
      .mergeMap(() => {
        return this.http.get(`${BASE_URL}/ViewConfig/Lookups`)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : SessionActions.loadLookupsSuccessAction(result.json());
          })
          .catch(error => Observable.of(SessionActions.loadLookupsErrorAction()));
      });
  }

  handleCheckLogin = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.CHECK_LOGIN)
      .mergeMap(({ payload }) => {
        return this.http.get(`${BASE_URL}/auth/validate/${payload}`)
          // .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
          // .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
          .map(result => {
            let validationData = result.json();
            return validationData.isValid ?
              SessionActions.loginUserSuccessAction(validationData.meta) :
              SessionActions.loginUserErrorAction();
          })
          .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
      });
  }
}
