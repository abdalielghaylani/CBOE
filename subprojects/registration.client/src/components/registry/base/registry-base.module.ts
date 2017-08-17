import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegFragmentsFormItem } from './fragments-form-item';
import { RegIdListFormItem } from './id-list-form-item';
import { RegProjectsFormItem } from './projects-form-item';
import { RegStructureFormItem } from './structure-form-item';
import { RegStructureQueryFormItem } from './structure-query-form-item';
import { RegFormView } from './form-view';
import { RegFormGroupView } from './form-group-view';
import { RegFormGroupItemView } from './form-group-item-view';
import { RegSearchFormGroupView } from './search-form-group-view';
import { RegSearchFormGroupItemView } from './search-form-group-item-view';
import { RegSearchFormView } from './search-form-view';
import { RegRecordDetailBase } from './record-detail-base';
import {
  DxBoxModule,
  DxButtonModule,
  DxCheckBoxModule,
  DxDropDownBoxModule,
  DxRadioGroupModule,
  DxDataGridModule,
  DxDateBoxModule,
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
import { RegCommonComponentModule } from '../../common';

export * from './fragments-form-item'
export * from './id-list-form-item';
export * from './projects-form-item';
export * from './structure-form-item';
export * from './form-view';
export * from './form-group-view';
export * from './registry-base.types';
export * from './record-detail-base';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    DxBoxModule,
    DxButtonModule,
    DxCheckBoxModule,
    DxDropDownBoxModule,
    DxRadioGroupModule,
    DxDataGridModule,
    DxDateBoxModule,
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
    RegCommonModule,
    RegCommonComponentModule
  ],
  declarations: [
    RegIdListFormItem, RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemView, RegSearchFormGroupItemView,
    RegRecordDetailBase
  ],
  exports: [
    RegIdListFormItem, RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemView, RegSearchFormGroupItemView,
    RegRecordDetailBase,
    RegCommonModule, RegCommonComponentModule
  ]
})
export class RegBaseComponentModule { }
