import { NgModule } from '@angular/core';
import { CommonModule, APP_BASE_HREF, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgRedux, NgReduxModule, DevToolsExtension } from '@angular-redux/store';
import { NgReduxRouter } from '@angular-redux/router';
import { routing, appRoutingProviders } from './reg-app.routing';
import { FormsModule, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RegApp } from './reg-app';
import { GridActions, SessionActions, ACTION_PROVIDERS, IAppState } from '../redux';
import { ConfigurationEpics, RegistryEpics, SessionEpics, EPIC_PROVIDERS } from '../epics';
import {
  RegRecordsPage, RegRecordDetailPage, RegLoginPage, RegAboutPage, RegRecordSearchPage,
  RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage,
  RegConfigXmlFormsPage, UnAuthorizedPage, ConfigurationPage
} from '../pages';
import {
  RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
  RegSettingsPageHeader
} from '../components';
import {
  RegRecords, RegRecordDetail, RegRecordSearch, RegQueryManagement, RegTemplates,
  RegDuplicateRecord, RegDuplicatePopup, RegBulkRegisterRecord
} from '../components';
import { RegLoginModule } from '../components/login/login.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { RegConfigurationComponentModule } from '../components';
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
    RegConfigurationComponentModule,
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
    RegDuplicateRecord,
    RegBulkRegisterRecord,
    RegDuplicatePopup,
    RegRecordSearch,
    RegRecordDetail,
    RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage, RegConfigXmlFormsPage,
    RegLoginPage,
    RegAboutPage,
    UnAuthorizedPage,
    ConfigurationPage
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
