import { Routes } from '@angular/router';
import { AuthGuard } from '../services';

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
  path: 'records/new/:templateId',
  component: RegRecordDetailPage
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
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigPropertiesPage
}, {
  path: 'configuration/forms',
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigFormsPage
}, {
  path: 'configuration/addins',
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigAddinsPage
}, {
  path: 'configuration/xml-forms',
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigXmlFormsPage
}, {
  path: 'configuration/settings',
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigSettingsPage
}, {
  path: 'configuration/:tableId',
  data: { privilege: 'CONFIG_REG' },
  canActivate: [AuthGuard],
  component: RegConfigTablesPage
}, {
  path: 'login',
  component: RegLoginPage
}];
