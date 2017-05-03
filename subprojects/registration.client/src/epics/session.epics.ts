import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { Http } from '@angular/http';
import { createAction } from 'redux-actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { IPayloadAction, SessionActions, RegActions } from '../actions';
import { apiUrlPrefix } from '../configuration';

@Injectable()
export class SessionEpics {
  constructor(private http: Http, private router: Router) { }

  handleLoginUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER)
      .mergeMap(({ payload }) => {
        return this.http.post(`${apiUrlPrefix}auth/login`, payload)
          .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
          .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
      });
  }

  handleLoginUserSuccess = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER_SUCCESS)
      .mergeMap(() => {
        this.router.navigate(['records/temp']);
        return this.http.get(`${apiUrlPrefix}ViewConfig/Lookups`)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : SessionActions.loadLookupsSuccessAction(result.json());
          })
          .catch(error => Observable.of(SessionActions.loadLookupsErrorAction()));
      });
  }

  handleLogoutUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGOUT_USER)
      .mergeMap(() => {
        return this.http.get(`${apiUrlPrefix}auth/logout`)
          .map(result => createAction(UPDATE_LOCATION)('login'))
          .catch(error => Observable.of(RegActions.ignoreAction()));
      });
  }

  handleCheckLogin = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.CHECK_LOGIN)
      .mergeMap(({ payload }) => {
        return !payload ? Observable.of(RegActions.ignoreAction()) :
          this.http.get(`${apiUrlPrefix}auth/validate/${payload}`)
          // .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
          // .catch(error => Observable.of(SessionActions.loginUserErrorAction()));
          .map(result => {
            let validationData = result.json();
            return validationData.isValid ?
              SessionActions.loginUserSuccessAction(validationData.meta) :
              RegActions.ignoreAction();
          })
          .catch(error => Observable.of(RegActions.ignoreAction()));
      });
  }
}
