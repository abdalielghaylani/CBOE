import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegToolModule } from './tool/tool.module';
import {
  INamedObject, IShareableObject, CShareableObject,
  FormGroupType, SubFormType,
  CDisplayInfo, CFormElement, CLayoutInfo, CCoeFormMode, CCoeForm, CCoeForms, CForm, CQueryForms, CDetailsForms, CListForms, CFormGroup,
  IFormContainer
} from './types';
import {
  getExceptionMessage, notify, notifyError, notifyException, notifySuccess,
  getFormGroup, getFormGroupData, convertToFormGroup, prepareFormGroupData,
  copyObject, copyObjectAndSet,
  IColumnConfig, getViewColumns
} from './utils';

export { RegToolModule };
export {
  INamedObject, IShareableObject, CShareableObject,
  FormGroupType, SubFormType,
  CDisplayInfo, CFormElement, CLayoutInfo, CCoeFormMode, CCoeForm, CCoeForms, CForm, CQueryForms, CDetailsForms, CListForms, CFormGroup,
  IFormContainer
};
export {
  getExceptionMessage, notify, notifyError, notifyException, notifySuccess,
  getFormGroup, getFormGroupData, convertToFormGroup, prepareFormGroupData,
  copyObject, copyObjectAndSet,
  IColumnConfig, getViewColumns
};

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    RegToolModule
  ],
  declarations: [
  ],
  exports: [
    RegToolModule
  ]
})
export class RegCommonModule { }
