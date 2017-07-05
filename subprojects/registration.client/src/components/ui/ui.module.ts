import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RegLayoutComponentModule } from '../layout';
import { RegSettingsPageHeader } from '../page-header';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    RegLayoutComponentModule
  ],
  declarations: [
    RegSettingsPageHeader
  ],
  exports: [
    RegSettingsPageHeader
  ]
})
export class RegUiModule { }
