import { NgModule } from '@angular/core';
import { CommonModule, APP_BASE_HREF, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgRedux, NgReduxModule, DevToolsExtension } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { routing, appRoutingProviders } from './reg-app.routing';
import { FormsModule, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RegApp } from './reg-app';
import { GridActions, SessionActions, ACTION_PROVIDERS } from '../actions';
import { ConfigurationEpics, RegistryEpics, SessionEpics, EPIC_PROVIDERS } from '../epics';
import {
  RegRecordsPage, RegRecordDetailPage, RegLoginPage, RegAboutPage, RegRecordSearchPage,
  RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage, RegConfigXmlFormsPage
} from '../pages';
import {
  RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
  RegSettingsPageHeader
} from '../components';
import { RegRecords, RegRecordDetail, RegRecordSearch, RegStructureImage, RegQueryManagement, RegTemplates } from '../components';
import { RegLoginModule } from '../components/login/login.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { HttpService, AuthGuard } from '../services';
import { IAppState } from '../store';
import { RegCommonComponentModule } from '../components';
import { RegLayoutComponentModule } from '../components';
import { RegBaseComponentModule } from '../components/registry/base/';
import {
  DxCheckBoxModule,
  DxRadioGroupModule,
  DxDataGridModule,
  DxSelectBoxModule,
  DxNumberBoxModule,
  DxFormModule,
  DxPopupModule,
  DxLoadIndicatorModule,
  DxLoadPanelModule,
  DxScrollViewModule,
  DxTextAreaModule,
  DxListModule,
  DxTextBoxModule,
  DxValidatorModule
} from 'devextreme-angular';

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    routing,
    CommonModule,
    RegCommonComponentModule,
    RegLayoutComponentModule,
    RegLoginModule,
    RegNavigatorModule,
    RegBaseComponentModule,
    NgReduxModule,
    DxCheckBoxModule,
    DxRadioGroupModule,
    DxDataGridModule,
    DxSelectBoxModule,
    DxNumberBoxModule,
    DxFormModule,
    DxPopupModule,
    DxLoadIndicatorModule,
    DxLoadPanelModule,
    DxScrollViewModule,
    DxTextAreaModule,
    DxListModule,
    DxTextBoxModule,
    DxValidatorModule
  ],
  declarations: [
    RegApp,
    RegRecordsPage,
    RegRecordSearchPage,
    RegRecordDetailPage,
    RegRecords,
    RegQueryManagement,
    RegTemplates,
    RegRecordSearch,
    RegRecordDetail,
    RegStructureImage,
    RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage, RegConfigXmlFormsPage,
    RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms, RegSettingsPageHeader,
    RegLoginPage,
    RegAboutPage,
  ],
  bootstrap: [
    RegApp
  ],
  providers: [
    DevToolsExtension,
    FormBuilder,
    NgReduxRouter,
    AuthGuard,
    appRoutingProviders
  ]
    .concat(ACTION_PROVIDERS)
    .concat(EPIC_PROVIDERS)
    .concat([{
      provide: HttpService,
      useFactory: (backend: XHRBackend, options: RequestOptions, redux: NgRedux<IAppState>) => {
        return new HttpService(backend, options, redux);
      },
      deps: [XHRBackend, RequestOptions, NgRedux]
    }])
})
export class RegAppModule { }
