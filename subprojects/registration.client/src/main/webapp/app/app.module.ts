import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {routerConfig} from './app.routes';
import {WebApp} from './app.component';
import {FormsModule} from '@angular/forms';
import {BrowserModule} from '@angular/platform-browser';
import {HttpModule} from '@angular/http';
import {RecordsComponent} from '../record/records.component';
import {ProjectsComponent} from '../project/projects.component';
import {LocationStrategy, HashLocationStrategy} from '@angular/common';
import {RecordService} from "../record/record.service";
import {ProjectService} from "../project/project.service";
import {DevExtremeModule} from 'devextreme-angular';

@NgModule({
    declarations: [WebApp, RecordsComponent, ProjectsComponent],
    imports     : [BrowserModule, FormsModule, HttpModule, RouterModule.forRoot(routerConfig), DevExtremeModule],
    providers   : [RecordService, ProjectService, {provide: LocationStrategy, useClass: HashLocationStrategy}],
    bootstrap   : [WebApp]
})
export class AppModule {}