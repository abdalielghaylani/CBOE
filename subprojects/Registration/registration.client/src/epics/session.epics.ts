import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { createAction } from 'redux-actions';
import { Observable, of } from 'rxjs';
import { map, filter } from 'rxjs/operators';



import { apiUrlPrefix } from '../configuration';
import { HttpService } from '../services';
import { IPayloadAction, SessionActions, RegActions, ILookupData } from '../redux';

@Injectable()
export class SessionEpics {
  constructor(private http: HttpService, private router: Router, private route: ActivatedRoute) { }

  handleLoginUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER)
      .mergeMap(({ payload }) => {
        return this.http.post(`${apiUrlPrefix}auth/login`, payload)
          .map(result => {
            if (result.json().meta.token === 'INVALID_USER') {
              return SessionActions.loginUserErrorAction();
            }
            return SessionActions.loginUserSuccessAction(result.json().meta);
          })
          .catch(error => of(SessionActions.loginUserErrorAction()));
      });
  }

  handleLoginUserSuccess = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGIN_USER_SUCCESS)
      .mergeMap(() => {
        return this.http.get(`${apiUrlPrefix}ViewConfig/Lookups?${new Date().getTime()}`)
          .map(result => {
            this.navigateToHomePage(result.json());
            return SessionActions.loadLookupsSuccessAction(result.json());
          })
          .catch(error => of(SessionActions.loginUserErrorAction()));
      });
  }

  navigateToHomePage(lookupData: ILookupData) {
    let returnUrlParam = 'returnUrl';
    let returnUrl = this.route.snapshot.queryParams[returnUrlParam];
    if (returnUrl) {
      this.router.navigate([returnUrl]);
    } else {
      let homeMenuPrivileges = lookupData.homeMenuPrivileges;
      if (this.getPrivilege(homeMenuPrivileges, 'SEARCH_TEMP')) {
        this.router.navigate(['records/temp']);
      } else if (this.getPrivilege(homeMenuPrivileges, 'SEARCH_REG')) {
        this.router.navigate(['records']);
      } else if (this.getPrivilege(homeMenuPrivileges, 'ADD_COMPOUND_TEMP')) {
        this.router.navigate(['records/new']);
      } else if (this.getPrivilege(homeMenuPrivileges, 'CONFIG_REG')) {
        this.router.navigate(['configuration']);
      } else {
        this.router.navigate(['unauthorized']);
      }
    }
  }

  getPrivilege(homeMenuPrivileges, privilage) {
    let privilageItem = homeMenuPrivileges.find(p => p.privilegeName === privilage);
    if (privilageItem === undefined) {
      return false;
    }
    return privilageItem.visibility ? true : false;
  }

  handleLogoutUser = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.LOGOUT_USER)
      .mergeMap(() => {
        return this.http.get(`${apiUrlPrefix}auth/logout`)
          .map(result => createAction(UPDATE_LOCATION)('logout'))
          .catch(error => of(RegActions.ignoreAction()));
      });
  }

  handleCheckLogin = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === SessionActions.CHECK_LOGIN)
      .mergeMap(({ payload }) => {
        return !payload ? of(RegActions.ignoreAction()) :
          this.http.get(`${apiUrlPrefix}auth/validate/${payload}`)
            // .map(result => SessionActions.loginUserSuccessAction(result.json().meta))
            // .catch(error => of(SessionActions.loginUserErrorAction()));
            .map(result => {
              let validationData = result.json();
              return validationData.isValid ?
                SessionActions.loginUserSuccessAction(validationData.meta) :
                RegActions.ignoreAction();
            })
            .catch(error => of(RegActions.ignoreAction()));
      });
  }
}
