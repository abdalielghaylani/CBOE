import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder } from '@angular/forms';
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
  DxValidatorModule,
  DxFileUploaderModule
} from 'devextreme-angular';
import { RegRecords } from './records.component';
import { RegRecordDetail } from './record-detail.component';
import { RegRecordSearch } from './record-search.component';
import { RegQueryManagement } from './query-management.component';
import { RegTemplates } from './templates.component';
import { RegDuplicateRecord } from './registry-duplicate.component';
import { RegBulkRegisterRecord } from './record-bulk-register.component';
import { RegDuplicatePopup } from './duplicate-popup.component';
import { RegRecordPrint } from './record-print.component';
import { IResponseData, ITemplateData, CTemplateData } from './registry.types';
import { RegBaseComponentModule } from './base';
import { RegLayoutComponentModule } from '../layout';

export * from './base';
export {
  RegRecords, RegRecordDetail, RegRecordSearch, RegQueryManagement, RegTemplates, RegDuplicateRecord,
  RegDuplicatePopup, RegBulkRegisterRecord, RegRecordPrint
};
export { IResponseData, ITemplateData, CTemplateData };

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
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
    DxValidatorModule,
    DxFileUploaderModule,
    RegLayoutComponentModule,
    RegBaseComponentModule
  ],
  declarations: [
    RegQueryManagement,
    RegRecordDetail,
    RegRecordSearch,
    RegRecords,
    RegTemplates,
    RegDuplicateRecord,
    RegBulkRegisterRecord,
    RegRecordPrint,
    RegDuplicatePopup
  ],
  exports: [
    RegQueryManagement,
    RegRecordDetail,
    RegRecordSearch,
    RegRecords,
    RegTemplates,
    RegDuplicateRecord,
    RegBulkRegisterRecord,
    RegRecordPrint,
    RegDuplicatePopup,
    RegBaseComponentModule,
    RegLayoutComponentModule
  ],
  providers: [
    FormBuilder
  ]
})
export class RegistryModule { }
