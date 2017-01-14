import { NgModule } from '@angular/core';
import { CommonModule, APP_BASE_HREF, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgRedux, NgReduxModule, DevToolsExtension } from 'ng2-redux';
import { NgReduxRouter } from 'ng2-redux-router';
import { routing, appRoutingProviders } from './reg-app.routing';
import { FormsModule, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RegApp } from './reg-app';
import { CounterActions, GridActions, SessionActions, ACTION_PROVIDERS } from '../actions';
import { ConfigurationEpics, RegistryEpics, SessionEpics, EPIC_PROVIDERS } from '../epics';
import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegConfigurationPage,
  RegAboutPage,
  RegCounterPage
} from '../pages';
import { RegConfiguration, RegCounter, RegRecords, RegRecordDetail, RegStructureImage } from '../components';
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
    RegRecordsPage,
    RegRecordDetailPage,
    RegRecords,
    RegRecordDetail,
    RegStructureImage,
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
    appRoutingProviders,
    { provide: APP_BASE_HREF, useValue: '/Registration.Server' },
    { provide: LocationStrategy, useClass: PathLocationStrategy }
  ]
  .concat(ACTION_PROVIDERS)
  .concat(EPIC_PROVIDERS)
})
export class RegAppModule { }
