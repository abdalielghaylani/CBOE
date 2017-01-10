import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {RegAlert} from '../alert/alert.component';
import {RegButton} from '../button/button.component';
import {RegLogo,RegLoginLogo} from '../logo';
import {RegContainer} from '../container/container.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer,
    RegLoginLogo
  ],
  exports: [
    RegAlert,
    RegButton,
    RegLogo,
    RegContainer,
    RegLoginLogo
  ]
})
export class RegUiModule { }
