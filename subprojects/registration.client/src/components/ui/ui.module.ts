import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RegAlert } from '../alert/alert.component';
import { RegLogo, RegLoginLogo } from '../logo';
import { RegContainer } from '../container/container.component';
import { RegPageHeader, RegSettingsPageHeader } from '../page-header';

@NgModule({
  imports: [
    CommonModule,
    RouterModule
  ],
  declarations: [
    RegAlert,
    RegLogo,
    RegContainer,
    RegLoginLogo,
    RegPageHeader,
    RegSettingsPageHeader
  ],
  exports: [
    RegAlert,
    RegLogo,
    RegContainer,
    RegLoginLogo,
    RegPageHeader,
    RegSettingsPageHeader
  ]
})
export class RegUiModule { }
