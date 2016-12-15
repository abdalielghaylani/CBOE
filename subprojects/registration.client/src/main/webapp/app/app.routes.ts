import { Routes } from '@angular/router';

import {ProjectsComponent} from './project/projects.component';

export const routerConfig: Routes = [
    { path: '', redirectTo: 'messages', pathMatch: 'full' },
    { path: 'projects', component: ProjectsComponent }
];
