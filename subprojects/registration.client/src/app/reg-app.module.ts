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
import { RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms } from '../components';
import { RegRecords, RegRecordDetail, RegRecordSearch, RegStructureImage, RegQueryManagement } from '../components';
import { RegLoginModule } from '../components/login/login.module';
import { RegUiModule } from '../components/ui/ui.module';
import { RegNavigatorModule } from '../components/navigator/navigator.module';
import { RegFooterModule } from '../components/footer/footer.module';
import { ToolModule } from '../common';
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
  DxScrollViewModule
} from 'devextreme-angular';

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
    DxScrollViewModule
  ],
  declarations: [
    RegApp,
    RegRecordsPage,
    RegRecordSearchPage,
    RegRecordDetailPage,
    RegRecords,
    RegQueryManagement,
    RegRecordSearch,
    RegRecordDetail,
    RegStructureImage,
    RegConfigAddinsPage, RegConfigFormsPage, RegConfigPropertiesPage, RegConfigSettingsPage, RegConfigTablesPage, RegConfigXmlFormsPage,
    RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
    RegLoginPage,
    RegAboutPage
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
