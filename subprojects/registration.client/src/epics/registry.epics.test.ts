import { fakeAsync, inject, TestBed, } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { HttpModule, XHRBackend, ResponseOptions, Response } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing/mock_backend';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { ActionsObservable } from 'redux-observable';
import { NgRedux, NgReduxModule, DevToolsExtension } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { RegistryActions } from '../actions';
import { RegistryEpics } from './registry.epics';
import { configureTests } from '../tests.configure';

describe('configuration.epics', () => {
  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [HttpModule, RouterModule, NgReduxModule],
        providers: [
          NgReduxRouter,
          { provide: XHRBackend, useClass: MockBackend },
          RegistryEpics
        ]
      });
    };
    configureTests(configure).then(done);
  });

  it('should open and retrieve records', fakeAsync(
    inject([XHRBackend, RegistryEpics], (mockBackend, registryEpics: RegistryEpics) => {
      const payload: any = { temporary: true };
      const data = [{ temporary: true, rows: [{ id: 1, value: 'v1' }, { id: 2, value: 'v2' }], totalCount: 2 }];
      mockBackend.connections.subscribe((connection: MockConnection) => {
        let response = new Response(new ResponseOptions({ body: data }));
        response.url = '';
        connection.mockRespond(response);
      });

      const action$ = new ActionsObservable(Observable.of(RegistryActions.openRecordsAction(payload)));
      registryEpics.handleRegistryActions(action$, null).subscribe(action =>
        expect(action).toEqual(RegistryActions.openRecordsSuccessAction(payload.temporary, data))
      );
    })
  ));

  it('should process a open-records error', fakeAsync(
    inject([XHRBackend, RegistryEpics], (mockBackend, registryEpics: RegistryEpics) => {
      const error = new Error('cannot get records');
      mockBackend.connections.subscribe((connection: MockConnection) => {
        connection.mockError(error);
      });

      const action$ = new ActionsObservable(Observable.of(RegistryActions.openRecordsAction({ temporary: true })));
      registryEpics.handleRegistryActions(action$, null).subscribe(action =>
        expect(action).toEqual(RegistryActions.openRecordsErrorAction(error))
      );
    })
  ));
});
