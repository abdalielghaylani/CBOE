import { Routes } from '@angular/router';

import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegConfigAddinsPage,
  RegConfigFormsPage,
  RegConfigPropertiesPage,
  RegConfigSettingsPage,
  RegConfigTablesPage,
  RegConfigXmlFormsPage,
  RegLoginPage,
  RegRecordSearchPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: '/login'
}, {
  path: 'index.html',
  redirectTo: '/login'
}, {
  path: 'records/temp',
  component: RegRecordsPage
}, {
  path: 'records',
  component: RegRecordsPage
}, {
  path: 'records/restore',
  component: RegRecordsPage
}, {
  path: 'records/new',
  component: RegRecordDetailPage
}, {
  path: 'records/:id',
  component: RegRecordDetailPage
}, {
  path: 'records/temp/:id',
  component: RegRecordDetailPage
}, {
  path: 'configuration/properties',
  component: RegConfigPropertiesPage
}, {
  path: 'configuration/forms',
  component: RegConfigFormsPage
}, {
  path: 'configuration/addins',
  component: RegConfigAddinsPage
}, {
  path: 'configuration/xml-forms',
  component: RegConfigXmlFormsPage
}, {
  path: 'configuration/settings',
  component: RegConfigSettingsPage
}, {
  path: 'configuration/:tableId',
  component: RegConfigTablesPage
}, {
  path: 'login',
  component: RegLoginPage
}];
