import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegDataGridFormItem } from './data-grid-form-item';
import { RegDateFormItem } from './date-form-item';
import { RegDropDownFormItem } from './drop-down-form-item';
import { RegFragmentsFormItem } from './fragments-form-item';
import { RegIdListFormItem } from './id-list-form-item';
import { RegProjectsFormItem } from './projects-form-item';
import { RegStructureFormItem } from './structure-form-item';
import { RegStructureQueryFormItem } from './structure-query-form-item';
import { RegStructureImageFormItem } from './structure-image-form-item';
import { RegTagBoxFormItem } from './tag-box-form-item';
import { RegFormView } from './form-view';
import { RegFormGroupView } from './form-group-view';
import { RegFormGroupItemView } from './form-group-item-view';
import { RegSearchFormGroupView } from './search-form-group-view';
import { RegSearchFormGroupItemView } from './search-form-group-item-view';
import { RegSearchFormView } from './search-form-view';
import {
  DxCheckBoxModule,
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

export * from './data-grid-form-item';
export * from './date-form-item';
export * from './drop-down-form-item';
export * from './fragments-form-item'
export * from './id-list-form-item';
export * from './projects-form-item';
export * from './structure-form-item';
export * from './structure-image-form-item';
export * from './tag-box-form-item';
export * from './form-view';
export * from './form-group-view';
export * from './registry-base.types';
export * from './registry-search-base.types';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    DxCheckBoxModule,
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
    RegDataGridFormItem, RegDateFormItem, RegDropDownFormItem, RegIdListFormItem,
    RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem, RegStructureImageFormItem, RegTagBoxFormItem,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemView, RegSearchFormGroupItemView
  ],
  exports: [
    RegDataGridFormItem, RegDateFormItem, RegDropDownFormItem, RegIdListFormItem,
    RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem, RegStructureImageFormItem, RegTagBoxFormItem,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemView, RegSearchFormGroupItemView,
    RegCommonModule, RegCommonComponentModule
  ]
})
export class RegBaseComponentModule { }
