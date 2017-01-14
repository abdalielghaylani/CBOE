import { Routes } from '@angular/router';

import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegConfigurationPage,
  RegCounterPage,
  RegAboutPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: '/records/temp'
}, {
  path: 'records/temp',
  component: RegRecordsPage
}, {
  path: 'records',
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
  path: 'counter',
  component: RegCounterPage
}, {
  path: 'about',
  component: RegAboutPage
}];
