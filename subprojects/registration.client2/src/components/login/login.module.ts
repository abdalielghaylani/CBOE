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
import { RegModalModule } from '../modal/modal.module';
import { RegFormModule } from '../form/form.module';

@NgModule({
  imports: [
    ReactiveFormsModule,
    CommonModule,
    HttpModule,
    RegUiModule,
    RegModalModule,
    RegFormModule
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
