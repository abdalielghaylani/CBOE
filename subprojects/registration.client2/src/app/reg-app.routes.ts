import { Routes } from '@angular/router';

import {
  RegCounterPage,
  RegAboutPage
} from '../pages';

export const REG_APP_ROUTES: Routes = [{
  pathMatch: 'full',
  path: '',
  redirectTo: 'counter'
}, {
  path: 'counter',
  component: RegCounterPage
}, {
  path: 'about',
  component: RegAboutPage
}];
