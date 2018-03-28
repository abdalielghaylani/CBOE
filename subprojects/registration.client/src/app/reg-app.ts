import { PrivilegeUtils } from './../common/utils/privilege.utils';
import { Component, ViewEncapsulation } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { DevToolsExtension, NgRedux, select } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { createEpicMiddleware, combineEpics } from 'redux-observable';
import { ConfigurationEpics, RegistryEpics, SessionEpics, RegistrySearchEpics } from '../epics';
import { IAppState, ISession, rootReducer, ILookupData, RegistryActions, SessionActions } from '../redux';
import { middleware, enhancers, reimmutify, IRegistry, RegistryFactory } from '../redux';
import { Subscription } from 'rxjs/Subscription';

import { dev, helpLinkUserGuide, helpLinkAdminGuide, basePath } from '../configuration';
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
  protected ngUnsubscribe: Subject<void> = new Subject<void>();
  public fullScreenEnabled: boolean = false;
  private lookupsSubscription: Subscription;
  private lookups: ILookupData;
  private helpUserGuideLink: string;
  private helpAdminGuideLink: string;
  private isAboutPopupVisible: boolean;
  private aboutContent: any;
  private isLoggedIn: boolean = false;
  private printerFriendly: boolean = false;
  private hasRegAppPrivilege: boolean = false;
  private selectedCommandIndex: number = 0;

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
    this.helpUserGuideLink = basePath + helpLinkUserGuide;
    this.helpAdminGuideLink = basePath + helpLinkAdminGuide;
    this.loggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
    });
    this.routerLink$.subscribe(link => {
      this.printerFriendly = link.indexOf('/print') >= 0;
      this.selectedCommandIndex = link.endsWith('/temp') || link.endsWith('temp/marked') || link.indexOf('temp/hits/') > 0 ? 1
        : !link.endsWith('/temp') && link.indexOf('records/') >= 0 && link.indexOf('/marked') <= 0 && link.indexOf('/hits/') <= 0 ? 2
          : link.endsWith('/records') || link.endsWith('records/marked') || link.indexOf('records/hits/') > 0 ? 3
            : link.indexOf('configuration') > 0 ? 4 : 0;
    });
  }

  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  setVisibility(privilege: string) {
    if (this.lookups) {
      let privileges = this.lookups.homeMenuPrivileges;
      let privilegeItem = privileges.find(p => p.privilegeName === privilege);
      if (privilegeItem != null) {
        return privilegeItem.visibility;
      }
    }
    return false;
  }

  // Trigger data retrieval for the view to show.
  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
    this.aboutContent = this.lookups.systemInformation;
    this.hasRegAppPrivilege = PrivilegeUtils.hasRegAppPrivilege(lookups.userPrivileges);
  }

  openAboutPopup() {
    this.isAboutPopupVisible = true;
  }
};
