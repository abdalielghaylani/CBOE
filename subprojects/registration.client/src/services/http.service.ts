import { Injectable } from '@angular/core';
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { Observable, ObservableInput } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { SessionActions, IAppState } from '../redux';
import { notify, notifyError, notifySuccess } from '../common';

@Injectable()
export class HttpService extends Http {
  private ngRedux: NgRedux<IAppState>;
  private tokenPath?: string;

  constructor(backend: XHRBackend, options: RequestOptions, ngRedux: NgRedux<IAppState>, tokenPath?: string) {
    if (tokenPath) {
      let token = localStorage.getItem(tokenPath);
      options.headers.set('Authorization', `Bearer ${token}`);
    }
    super(backend, options);
    this.ngRedux = ngRedux;
    this.tokenPath = tokenPath;
  }

  request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
    if (this.tokenPath) {
      let token = localStorage.getItem(this.tokenPath);
      if (typeof url === 'string') { // meaning we have to add the token to the options, not in url
        if (!options) {
          // let's make option object
          options = { headers: new Headers() };
        }
        options.headers.set('Authorization', `Bearer ${token}`);
      } else {
        // we have to add the token to the url object
        url.headers.set('Authorization', `Bearer ${token}`);
      }
    }
    return super.request(url, options)
      .map((res: Response) => this.preProcessRequest(res))
      .catch((err: any, caught: Observable<Response>) => this.catchAuthError(err, caught));
  }

  private checkRedirect(res: Response): Response {
    if (res.url && res.url.indexOf('index.html') > 0) {
      this.ngRedux.dispatch(SessionActions.logoutUserAction());
      throw res;
    } else if (res.url && res.url.indexOf('records/') > 0 && res.status === 404 ) {
      this.ngRedux.dispatch(createAction(UPDATE_LOCATION)(`records${res.url.indexOf('temp') > 0 ? '/temp' : ''}`));
    }
    return res;
  }

  private preProcessRequest(res: Response): Response {
    return this.checkRedirect(res);
  }

  private catchAuthError(res: any, caught: Observable<Response>): ObservableInput<any> {
    // we have to pass HttpService's own instance here as `self`
    if (res.url) {
      this.checkRedirect(res);
    }
    if (this.tokenPath) {
      if (res.status === 401 || res.status === 403) {
        // if not authenticated
        // console.log(res);
      }
    }
    throw res;
  }
}
