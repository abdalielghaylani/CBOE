import { Routes } from '@angular/router';

import {
  RegHomePage,
  RegHomeDetailPage,
  RegConfigurationPage,
  RegCounterPage,
  RegAboutPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: 'home'
}, {
  path: 'home',
  component: RegHomePage
}, {
  path: 'home/:regId',
  component: RegHomeDetailPage
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
