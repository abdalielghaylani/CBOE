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
import { RegLayoutComponentModule } from '../layout';
import { RegCommonComponentModule } from '../common';

@NgModule({
  imports: [
    ReactiveFormsModule,
    CommonModule,
    HttpModule,
    RegCommonComponentModule,
    RegLayoutComponentModule
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
