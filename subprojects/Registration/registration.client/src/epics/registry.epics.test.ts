import { fakeAsync, inject, TestBed, } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { HttpModule, XHRBackend, RequestOptions, ResponseOptions, Response } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing/mock_backend';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { ActionsObservable } from 'redux-observable';
import { NgRedux, NgReduxModule, DevToolsExtension } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { RegistryEpics } from './registry.epics';
import { TestModule } from '../test';
import { HttpService } from '../services';
import { RegistryActions, IAppState } from '../redux';

describe('configuration : epics', () => {
  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, HttpModule, RouterModule, NgReduxModule],
        providers: [
          NgReduxRouter,
          { provide: XHRBackend, useClass: MockBackend },
          {
            provide: HttpService,
            useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
              return new HttpService(backend, options, redux);
            },
            deps: [XHRBackend, RequestOptions, NgRedux]
          },
          RegistryEpics
        ]
      });
    };
    TestModule.configureTests(configure).then(done);
  });
});
