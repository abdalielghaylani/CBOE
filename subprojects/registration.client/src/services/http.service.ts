import { Injectable } from '@angular/core';
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { NgRedux } from '@angular-redux/store';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { SessionActions } from '../actions';
import { IAppState } from '../store';
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
    return super.request(url, options).map(this.preProcessRequest(this)).catch(this.catchAuthError(this));
  }

  private checkRedirect(self: HttpService, res: Response) {
      if (res.url && res.url.indexOf('index.html') > 0) {
        self.ngRedux.dispatch(SessionActions.logoutUserAction());
        throw res;
      }
  }

  private preProcessRequest(self: HttpService) {
    return (res: Response) => {
      this.checkRedirect(self, res);
      return res;
    };
  }

  private catchAuthError(self: HttpService) {
    // we have to pass HttpService's own instance here as `self`
    return (err: Response, caught) => {
      // console.log(res);
      this.checkRedirect(self, err);
      if (self.tokenPath) {
        if (err.status === 401 || err.status === 403) {
          // if not authenticated
          // console.log(res);
        }
      }
      throw err;
    };
  }
}
