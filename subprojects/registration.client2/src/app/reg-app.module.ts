import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgRedux, NgReduxModule, DevToolsExtension } from 'ng2-redux';
import { NgReduxRouter } from 'ng2-redux-router';
import { routing, appRoutingProviders } from './reg-app.routing';
import { FormsModule, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RegApp } from './reg-app';
import { CounterActions, GridActions, SessionActions, ACTION_PROVIDERS } from '../actions';
import { ConfigurationEpics, SessionEpics, EPIC_PROVIDERS } from '../epics';
import {
  RegHomePage,
  RegHomeDetailPage,
  RegConfigurationPage,
  RegAboutPage,
  RegCounterPage
} from '../pages';
import { RegConfiguration, RegCounter, RegHome, RegHomeDetail } from '../components';
import { RegLoginModule } from '../components/login/login.module';
import { RegUiModule } from '../components/ui/ui.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { RegFooterModule } from '../components/footer/footer.module';
import { ToolModule } from '../common';
import { DxDataGridModule } from 'devextreme-angular';

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    routing,
    CommonModule,
    RegLoginModule,
    RegUiModule,
    RegNavigatorModule,
    RegFooterModule,
    ToolModule,
    NgReduxModule.forRoot(),
    DxDataGridModule
  ],
  declarations: [
    RegApp,
    RegHomePage,
    RegHomeDetailPage,
    RegHome,
    RegHomeDetail,
    RegConfigurationPage,
    RegConfiguration,
    RegAboutPage,
    RegCounterPage,
    RegCounter
  ],
  bootstrap: [
    RegApp
  ],
  providers: [
    DevToolsExtension,
    FormBuilder,
    NgReduxRouter,
    appRoutingProviders
  ]
  .concat(ACTION_PROVIDERS)
  .concat(EPIC_PROVIDERS)
})
export class RegAppModule { }
