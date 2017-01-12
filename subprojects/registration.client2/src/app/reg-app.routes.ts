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
  redirectTo: 'records'
}, {
  path: 'records',
  component: RegRecordsPage
}, {
  path: 'review',
  component: RegRecordsPage
}, {
  path: 'review/:id',
  component: RegRecordsPage
}, {
  path: 'registry',
  component: RegRecordsPage
}, {
  path: 'registry/:id',
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
