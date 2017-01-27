import { fakeAsync, inject, TestBed, } from '@angular/core/testing';
import { HttpModule, XHRBackend, ResponseOptions, Response } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing/mock_backend';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { ConfigurationActions } from '../actions';
import { ConfigurationEpics } from './configuration.epics';
import { configureTests } from '../tests.configure';

describe('configuration.epics', () => {
  beforeEach(done => {
    const configure = (testBed: TestBed) => {
      testBed.configureTestingModule({
        imports: [HttpModule],
        providers: [
          { provide: XHRBackend, useClass: MockBackend },
          ConfigurationEpics
        ]
      });
    };
    configureTests(configure).then(done);
  });

  it('should open and retrieve table data', fakeAsync(
    inject([XHRBackend, ConfigurationEpics], (mockBackend, configureEpics) => {
      const data = [{ id: 1, value: 'v1' }, { id: 2, value: 'v2' }];
      mockBackend.connections.subscribe((connection: MockConnection) => {
        let response = new Response(new ResponseOptions({ body: data }));
        response.url = '';
        connection.mockRespond(response);
      });

      const action$ = Observable.of(ConfigurationActions.openTableAction('table'));
      configureEpics.handleOpenTable(action$).subscribe(action =>
        expect(action).toEqual(ConfigurationActions.openTableSuccessAction(data))
      );
    })
  ));

  it('should process a open-table error', fakeAsync(
    inject([XHRBackend, ConfigurationEpics], (mockBackend, configureEpics) => {
      mockBackend.connections.subscribe((connection: MockConnection) => {
        connection.mockError(new Error('cannot get table data'));
      });

      const action$ = Observable.of(ConfigurationActions.openTableAction('table'));
      configureEpics.handleOpenTable(action$).subscribe(action =>
        expect(action).toEqual({ type: ConfigurationActions.OPEN_TABLE_ERROR })
      );
    })
  ));
});
