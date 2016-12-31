import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import {
  NgRedux,
  NgReduxModule,
  DevToolsExtension,
} from 'ng2-redux';
import { NgReduxRouter } from 'ng2-redux-router';
import {
  routing,
  appRoutingProviders
} from './reg-app.routing';
import {
  FormsModule,
  FormBuilder,
  ReactiveFormsModule,
} from '@angular/forms';
import { RegApp } from './reg-app';
import { SessionActions } from '../actions/session.actions';
import { SessionEpics } from '../epics/session.epics';
import {
  RioAboutPage,
  RioCounterPage
} from '../pages';
import { RioCounter } from '../components/counter/counter.component';
import { RioLoginModule } from '../components/login/login.module';
import { RioUiModule } from '../components/ui/ui.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { DxDataGridModule } from 'devextreme-angular';

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    routing,
    CommonModule,
    RioLoginModule,
    RioUiModule,
    RegNavigatorModule,
    NgReduxModule.forRoot(),
    DxDataGridModule
  ],
  declarations: [
    RegApp,
    RioAboutPage,
    RioCounterPage,
    RioCounter
  ],
  bootstrap: [
    RegApp
  ],
  providers: [
    DevToolsExtension,
    FormBuilder,
    NgReduxRouter,
    appRoutingProviders,
    SessionActions,
    SessionEpics
  ]
})
export class RegAppModule { }
