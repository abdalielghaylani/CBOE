import {
  Routes,
  RouterModule
} from '@angular/router';
import { REG_APP_ROUTES } from './reg-app.routes';

const appRoutes: Routes = REG_APP_ROUTES;

export const appRoutingProviders: any[] = [];

export const routing = RouterModule.forRoot(appRoutes);
