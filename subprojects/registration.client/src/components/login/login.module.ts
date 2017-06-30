import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import {
  ReactiveFormsModule
} from '@angular/forms';
import {
  RegLoginForm,
  RegLoginModal
} from '../index';
import { RegUiModule } from '../ui/ui.module';
import { RegCommonComponentModule } from '../common';

@NgModule({
  imports: [
    ReactiveFormsModule,
    CommonModule,
    HttpModule,
    RegUiModule,
    RegCommonComponentModule
  ],
  declarations: [
    RegLoginModal,
    RegLoginForm
  ],
  exports: [
    RegLoginModal
  ]
})
export class RegLoginModule { }
