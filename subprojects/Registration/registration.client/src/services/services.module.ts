import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Http, XHRBackend, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { NgRedux } from '@angular-redux/store';
import { AuthGuard } from './auth-guard.service';
import { HttpService } from './http.service';
import { IAppState } from '../redux';

export * from './auth-guard.service';
export * from './http.service';

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    AuthGuard, {
      provide: HttpService,
      useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
        return new HttpService(backend, options, redux);
      },
      deps: [XHRBackend, RequestOptions, NgRedux]
    }
  ]
})
export class RegServicesModule { }
