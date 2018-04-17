import { APP_BASE_HREF } from '@angular/common';
import {
  Routes,
  RouterModule
} from '@angular/router';
import { REG_APP_ROUTES } from './reg-app.routes';
import { production, basePath } from '../configuration';

const appRoutes: Routes = REG_APP_ROUTES;

export const appRoutingProviders: any[] = [{ provide: APP_BASE_HREF, useValue: basePath }];

export const routing = RouterModule.forRoot(appRoutes);
