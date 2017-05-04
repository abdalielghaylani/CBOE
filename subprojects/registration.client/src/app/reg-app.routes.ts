import { Routes } from '@angular/router';

import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegConfigurationPage,
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
  path: 'configuration/:tableId',
  component: RegConfigurationPage
}, {
  path: 'login',
  component: RegLoginPage
}];
