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
  RegRecordsPage, RegBulkRecordsPage, RegRecordDetailPage, RegLoginPage, RegAboutPage, RegRecordSearchPage,
  RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage,
  RegConfigXmlFormsPage, UnAuthorizedPage, ConfigurationPage, RegRecordPrintPage
} from '../pages';
import {
  RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
  RegSettingsPageHeader
} from '../components';
import { RegLoginModule } from '../components/login/login.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { RegConfigurationComponentModule } from '../components';
import { RegistryModule } from '../components/registry';
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
  DxValidatorModule,
  DxFileUploaderModule
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
    RegistryModule,
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
    DxValidatorModule,
    DxFileUploaderModule
  ],
  declarations: [
    RegApp,
    RegRecordsPage,
    RegBulkRecordsPage,
    RegRecordSearchPage,
    RegRecordDetailPage,
    RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage, RegConfigXmlFormsPage,
    RegLoginPage,
    RegAboutPage,
    UnAuthorizedPage,
    ConfigurationPage,
    RegRecordPrintPage
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
