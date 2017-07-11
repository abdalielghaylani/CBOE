import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import { NgReduxModule } from '@angular-redux/store';
import {
  DxCheckBoxModule,
  DxRadioGroupModule,
  DxDataGridModule,
  DxSelectBoxModule,
  DxNumberBoxModule,
  DxFormModule,
  DxPopupModule,
  DxLoadIndicatorModule,
  DxLoadPanelModule,
  DxScrollViewModule,
  DxTextAreaModule,
  DxListModule,
  DxTextBoxModule,
  DxValidatorModule
} from 'devextreme-angular';
import {
  RegQueryManagement,
  RegRecordDetail,
  RegRecordSearch,
  RegRecords,
  RegTemplates
} from './index';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgReduxModule,
    DxCheckBoxModule,
    DxRadioGroupModule,
    DxDataGridModule,
    DxSelectBoxModule,
    DxNumberBoxModule,
    DxFormModule,
    DxPopupModule,
    DxLoadIndicatorModule,
    DxLoadPanelModule,
    DxScrollViewModule,
    DxTextAreaModule,
    DxListModule,
    DxTextBoxModule,
    DxValidatorModule
  ],
  declarations: [
    RegQueryManagement,
    RegRecordDetail,
    RegRecordSearch,
    RegRecords,
    RegTemplates
  ],
  exports: [
    RegQueryManagement,
    RegRecordDetail,
    RegRecordSearch,
    RegRecords,
    RegTemplates
  ]
})
export class RegistryModule { }
