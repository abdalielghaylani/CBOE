import {
    fakeAsync,
    inject,
    TestBed,
} from '@angular/core/testing';
import {
    HttpModule,
    XHRBackend,
    RequestOptions,
    ResponseOptions,
    Response
} from '@angular/http';
import { RouterTestingModule } from '@angular/router/testing';
import { NgRedux, NgReduxModule } from '@angular-redux/store';
import { Observable, of } from 'rxjs';

import { SessionEpics } from './session.epics';
import {
    MockBackend,
    MockConnection
} from '@angular/http/testing';
import { TestModule } from '../test';
import { HttpService } from '../services';
import { SessionActions, IAppState } from '../redux';

describe('SessionEpics', () => {
    beforeEach(done => {
        const configure = (testBed: TestBed) => {
            testBed.configureTestingModule({
                imports: [TestModule, HttpModule, NgReduxModule, RouterTestingModule],
                providers: [
                    { provide: XHRBackend, useClass: MockBackend },
                    {
                        provide: HttpService,
                        useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
                        return new HttpService(backend, options, redux);
                        },
                        deps: [XHRBackend, RequestOptions, NgRedux]
                    },
                    SessionEpics
                ]
            });
        };
        TestModule.configureTests(configure).then(done);
    });

    it('should process a successful login', fakeAsync(
        inject([XHRBackend, SessionEpics], (mockBackend, sessionEpics) => {
            const data = { token: '123', user: { fullName: 'John Doe' } };
            mockBackend.connections.subscribe((connection: MockConnection) => {
                let response = new Response(
                    new ResponseOptions({ body: { meta: data } })
                );
                response.url = '';
                connection.mockRespond(response);
            });

            const action$ = of({ type: SessionActions.LOGIN_USER });
            sessionEpics.handleLoginUser(action$).subscribe(action =>
                expect(action).toEqual({ type: SessionActions.LOGIN_USER_SUCCESS, payload: data })
            );
        })
    ));

    it('should process a login error', fakeAsync(
        inject([XHRBackend, SessionEpics], (mockBackend, sessionEpics) => {
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockError(new Error('some error'));
            });

            const action$ = of({ type: SessionActions.LOGIN_USER });
            sessionEpics.handleLoginUser(action$).subscribe(action =>
                expect(action).toEqual({ type: SessionActions.LOGIN_USER_ERROR })
            );
        })
    ));
});
