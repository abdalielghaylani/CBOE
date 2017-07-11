import { Component, ViewEncapsulation } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { DevToolsExtension, NgRedux, select } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { createEpicMiddleware, combineEpics } from 'redux-observable';
import { IAppState, ISession, rootReducer, ILookupData } from '../store';
import { RegistryActions, SessionActions } from '../actions';
import { ConfigurationEpics, RegistryEpics, SessionEpics, RegistrySearchEpics } from '../epics';
import { middleware, enhancers, reimmutify, IRegistry, RegistryFactory } from '../store';
import { Subscription } from 'rxjs/Subscription';

import { dev } from '../configuration';
import 'bootstrap/dist/js/bootstrap.min.js';

@Component({
  selector: 'reg-app',
  // Allow app to define global styles.
  encapsulation: ViewEncapsulation.None,
  styles: [
    require('./reg-app.css'),
    require('../styles/index.css'),
    require('../styles/colors.css'),
    require('../styles/pe.css'),
    require('bootstrap/dist/css/bootstrap.min.css'),
    require('font-awesome/css/font-awesome.min.css'),
    require('devextreme/dist/css/dx.common.css'),
    require('devextreme/dist/css/dx.light.compact.css')
  ],
  template: require('./reg-app.html')
})
export class RegApp {
  @select(['session', 'hasError']) hasError$: Observable<boolean>;
  @select(['session', 'isLoading']) isLoading$: Observable<boolean>;
  @select(s => s.session.user.fullName) fullName$: Observable<string>;
  @select(s => !!s.session.token) loggedIn$: Observable<boolean>;
  @select(s => !s.session.token) loggedOut$: Observable<boolean>;
  @select(s => s.router) routerLink$: Observable<string>;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;

  public fullScreenEnabled: boolean = false;
  private lookupsSubscription: Subscription;
  private lookups: ILookupData;

  constructor(
    private devTools: DevToolsExtension,
    private ngRedux: NgRedux<IAppState>,
    private ngReduxRouter: NgReduxRouter,
    private sessionActions: SessionActions,
    private registryActions: RegistryActions,
    private configEpics: ConfigurationEpics,
    private registryEpics: RegistryEpics,
    private registrySearchEpics: RegistrySearchEpics,
    private sessionEpics: SessionEpics) {

    middleware.push(createEpicMiddleware(combineEpics(
      configEpics.handleOpenTable,
      registryEpics.handleRegistryActions,
      registrySearchEpics.handleRegistrySearchActions,
      sessionEpics.handleLoginUser,
      sessionEpics.handleLoginUserSuccess,
      sessionEpics.handleLogoutUser,
      sessionEpics.handleCheckLogin
    )));

    ngRedux.configureStore(
      rootReducer,
      {},
      middleware,
      devTools.isEnabled() ?
        [...enhancers, devTools.enhancer()] :
        enhancers);

    ngReduxRouter.initialize();
  }

  ngOnInit() {
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
  }

  setVisibility(privilage: string) {
    if (this.lookups) {
      let privilages = this.lookups.homeMenuPrivileges;
      let privilageItem = privilages.find(p => p.privilegeName === privilage);
      if (privilageItem !== undefined) {
        return privilageItem.visibility;
      }
    }
    return false;
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
  }
};
