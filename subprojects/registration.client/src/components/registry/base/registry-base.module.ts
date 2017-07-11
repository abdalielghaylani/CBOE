import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegDataGridFormItem } from './data-grid-form-item';
import { RegTagBoxFormItem } from './tag-box-form-item';
import { RegFormView } from './form-view';
import { RegFormGroupView } from './form-group-view';
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
  DxTagBoxModule,
  DxTextBoxModule,
  DxValidatorModule
} from 'devextreme-angular';
import { RegCommonModule } from '../../../common';

export * from './data-grid-form-item';
export * from './tag-box-form-item';
export * from './form-view';
export * from './form-group-view';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
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
    DxTagBoxModule,
    DxTextAreaModule,
    DxListModule,
    DxTextBoxModule,
    DxValidatorModule,
    RegCommonModule
  ],
  declarations: [
    RegDataGridFormItem,
    RegTagBoxFormItem,
    RegFormView, RegFormGroupView
  ],
  exports: [
    RegCommonModule,
    RegDataGridFormItem,
    RegTagBoxFormItem,
    RegFormView
  ]
})
export class RegBaseComponentModule { }
