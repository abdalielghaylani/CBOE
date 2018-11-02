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
  RegRecordSearchPage,
  UnAuthorizedPage,
  ConfigurationPage,
  RegBulkRecordsPage
} from '../pages';
import { RegRecordPrintPage } from '../pages/record-print.page';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: '/login'
}, {
  path: 'index.html',
  redirectTo: '/login'
}, {
  path: 'records/temp/reload',
  component: RegRecordsPage
}, {
  path: 'records/temp',
  component: RegRecordsPage
}, {
  path: 'records/temp/marked',
  component: RegRecordsPage
}, {
  path: 'records/temp/hits/:hitListId/marked',
  component: RegRecordsPage
}, {
  path: 'records/temp/hits/:hitListId',
  component: RegRecordsPage
}, {
  path: 'records',
  component: RegRecordsPage
}, {
  path: 'records/marked',
  component: RegRecordsPage
}, {
  path: 'records/hits/:hitListId/marked',
  component: RegRecordsPage
}, {
  path: 'records/hits/:hitListId',
  component: RegRecordsPage
}, {
  path: 'records/restore',
  component: RegRecordsPage
}, {
  path: 'records/bulkreg',
  component: RegBulkRecordsPage
}, {
  path: 'records/print',
  component: RegRecordPrintPage
}, {
  path: 'records/temp/restore/:id',
  component: RegRecordsPage
}, {
  path: 'records/restore/:id',
  component: RegRecordsPage
}, {
  path: 'records/temp/bulkreg/:id',
  component: RegRecordDetailPage
}, {
  path: 'records/bulkreg/:id',
  component: RegRecordDetailPage
}, {
  path: 'records/new/:templateId',
  component: RegRecordDetailPage
}, {
  path: 'records/new',
  component: RegRecordDetailPage
}, {
  path: 'records/:id',
  canActivate: [AuthGuard],
  component: RegRecordDetailPage
}, {
  path: 'records/temp/:id',
  component: RegRecordDetailPage
}, {
  path: 'records/temp/:id/current',
  component: RegRecordDetailPage
}, {
  path: 'configuration',
  component: ConfigurationPage
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
},
{
  path: 'unauthorized',
  component: UnAuthorizedPage
}
];
