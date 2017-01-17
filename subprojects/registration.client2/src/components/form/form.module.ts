import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import {
  RegForm,
  RegFormGroup,
  RegFormError,
  RegInput,
  RegInputLogin,
  RegLabel,
  RegInputGroup
} from './index';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  declarations: [
    RegForm,
    RegFormGroup,
    RegFormError,
    RegLabel,
    RegInput,
    RegInputLogin,
    RegInputGroup
  ],
  exports: [
    RegForm,
    RegFormGroup,
    RegFormError,
    RegLabel,
    RegInput,
    RegInputLogin,
    RegInputGroup
  ]
})
export class RegFormModule { }
