import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import {
  RegForm,
  RegFormGroup,
  RegFormError,
  RegInput,
  RegLabel
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
    RegInput
  ],
  exports: [
    RegForm,
    RegFormGroup,
    RegFormError,
    RegLabel,
    RegInput
  ]
})
export class RegFormModule { }
