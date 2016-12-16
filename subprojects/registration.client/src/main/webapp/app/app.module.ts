import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {routerConfig} from './app.routes';
import {WebApp} from './app.component';
import {FormsModule} from '@angular/forms';
import {BrowserModule} from '@angular/platform-browser';
import {HttpModule} from '@angular/http';
import {ProjectsComponent} from './project/projects.component';
import {LocationStrategy, HashLocationStrategy} from '@angular/common';
import {ProjectService} from "./project/project.service";
import {DxButtonModule} from 'devextreme-angular';
import 'devextreme/dist/css/dx.common.css';
import 'devextreme/dist/css/dx.light.css';

@NgModule({
    declarations: [WebApp, ProjectsComponent],
    imports     : [BrowserModule, FormsModule, HttpModule, RouterModule.forRoot(routerConfig), DxButtonModule],
    providers   : [ProjectService, {provide: LocationStrategy, useClass: HashLocationStrategy}],
    bootstrap   : [WebApp]
})
export class AppModule {}