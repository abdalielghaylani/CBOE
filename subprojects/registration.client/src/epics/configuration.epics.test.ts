import { fakeAsync, inject, TestBed, } from '@angular/core/testing';
import { HttpModule, XHRBackend, RequestOptions, ResponseOptions, Response } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing/mock_backend';
import { NgRedux, NgReduxModule } from '@angular-redux/store';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { ConfigurationActions } from '../actions';
import { ConfigurationEpics } from './configuration.epics';
import { TestModule } from '../test';
import { HttpService } from '../services';
import { IAppState } from '../store';

describe('configuration.epics', () => {
  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [TestModule, HttpModule, NgReduxModule],
        providers: [
          { provide: XHRBackend, useClass: MockBackend },
          {
            provide: HttpService,
            useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
              return new HttpService(backend, options, redux);
            },
            deps: [XHRBackend, RequestOptions, NgRedux]
          },
          ConfigurationEpics
        ]
      });
    };
    TestModule.configureTests(configure).then(done);
  });

  it('should open and retrieve table data', fakeAsync(
    inject([XHRBackend, ConfigurationEpics], (mockBackend, configureEpics) => {
      const data = [{ id: 1, value: 'v1' }, { id: 2, value: 'v2' }];
      mockBackend.connections.subscribe((connection: MockConnection) => {
        let response = new Response(new ResponseOptions({ body: data }));
        response.url = '';
        connection.mockRespond(response);
      });

      const tableId = 'table1';
      const action$ = Observable.of(ConfigurationActions.openTableAction(tableId));
      configureEpics.handleOpenTable(action$).subscribe(action =>
        expect(action).toEqual(ConfigurationActions.openTableSuccessAction({ tableId, data }))
      );
    })
  ));

  it('should process a open-table error', fakeAsync(
    inject([XHRBackend, ConfigurationEpics], (mockBackend, configureEpics) => {
      const error = new Error('cannot get table data');
      mockBackend.connections.subscribe((connection: MockConnection) => {
        connection.mockError(error);
      });

      const tableId = 'table1';
      const action$ = Observable.of(ConfigurationActions.openTableAction(tableId));
      configureEpics.handleOpenTable(action$).subscribe(action =>
        expect(action).toEqual(ConfigurationActions.openTableErrorAction({ tableId, error }))
      );
    })
  ));
});
