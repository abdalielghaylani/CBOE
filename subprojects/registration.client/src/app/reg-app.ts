import { Component, ViewEncapsulation } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { DevToolsExtension, NgRedux, select } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { createEpicMiddleware, combineEpics } from 'redux-observable';
import { IAppState, ISession, rootReducer } from '../store';
import { RegistryActions, SessionActions } from '../actions';
import { ConfigurationEpics, RegistryEpics, SessionEpics, RegistrySearchEpics } from '../epics';
import { RegAboutPage } from '../pages';
import { middleware, enhancers, reimmutify, IRegistry, RegistryFactory } from '../store';

import {
  RegButton,
  RegNavigator,
  RegNavigatorHeader,
  RegNavigatorItems,
  RegNavigatorItem,
  RegLogo,
  RegLoginModal,
  RegSidebar,
  RegFooter
} from '../components';

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

  public fullScreenEnabled: boolean = false;

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

    this.checkLogin();
  }

  checkLogin() {
    // See if auth token cookie is available.
    console.log('cookie:' + document.cookie);
    let token = document.cookie.split('; ').map(c => c.split('=')).find(c => c[0] === 'CS%5FSEC%5FUserName');
    // If token is available, check if cookie is valid.
    if (token) {
      console.log('user-name:' + token[1]);
      this.sessionActions.checkLogin(token[1]);
    }
  }

};
