import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegAlert } from '../alert/alert.component';
import { RegButton } from '../button/button.component';
import { RegLogo, RegLoginLogo } from '../logo';
import { RegContainer } from '../container/container.component';
import { RegPageHeader } from '../page-header/page-header.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer,
    RegLoginLogo,
    RegPageHeader
  ],
  exports: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer,
    RegLoginLogo,
    RegPageHeader
  ]
})
export class RegUiModule { }
