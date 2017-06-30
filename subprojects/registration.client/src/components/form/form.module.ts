import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RegForm } from './form';
import { RegFormGroup } from './form-group';
import { RegFormError } from './form-error';
import { RegInput } from './input';
import { RegInputLogin } from './input-login';
import { RegLabel } from './label';
import { RegInputGroup } from './input-group';

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
