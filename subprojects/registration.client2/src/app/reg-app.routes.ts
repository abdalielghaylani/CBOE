import { Routes } from '@angular/router';

import {
  RegRecordsPage,
  RegRecordDetailPage,
  RegTempRecordsPage,
  RegConfigurationPage,
  RegCounterPage,
  RegAboutPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: 'temp-records'
}, {
  path: 'temp-records',
  component: RegTempRecordsPage
}, {
  path: 'temp-records/:id',
  component: RegRecordDetailPage
}, {
  path: 'records',
  component: RegRecordsPage
}, {
  path: 'records/:id',
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
