import { Routes } from '@angular/router';

import {
  RegHomePage,
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
  path: 'configuration/:tableName',
  component: RegConfigurationPage
}, {
  path: 'counter',
  component: RegCounterPage
}, {
  path: 'about',
  component: RegAboutPage
}];
