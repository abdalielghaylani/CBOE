import { Routes } from '@angular/router';

import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegConfigurationPage,
  RegAboutPage,
  RegRecordSearchPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: '/records/temp'
}, {
  path: 'index.html',
  redirectTo: '/records/temp'
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
  path: 'search',
  component: RegRecordSearchPage
}, {
  path: 'search',
  component: RegRecordSearchPage
}, {
  path: 'search/temp',
  component: RegRecordSearchPage
}, {
  path: 'search/:id',
  component: RegRecordSearchPage
}, {
  path: 'configuration/:tableId',
  component: RegConfigurationPage
}, {
  path: 'about',
  component: RegAboutPage
}];
