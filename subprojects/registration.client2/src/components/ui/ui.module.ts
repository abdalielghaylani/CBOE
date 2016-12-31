import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {RegAlert} from '../alert/alert.component';
import {RegButton} from '../button/button.component';
import {RegLogo} from '../logo/logo.component';
import {RegContainer} from '../container/container.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer
  ],
  exports: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer
  ]
})
export class RegUiModule { }
