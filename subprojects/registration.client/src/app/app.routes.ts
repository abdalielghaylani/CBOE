import { Routes } from '@angular/router';

import {ProjectsComponent} from '../components/project/projects.component';
import {RecordsComponent} from '../components/record/records.component';

export const routerConfig: Routes = [
    { path: '', redirectTo: 'messages', pathMatch: 'full' },
    { path: 'records', component: RecordsComponent },
    { path: 'projects', component: ProjectsComponent }
];
