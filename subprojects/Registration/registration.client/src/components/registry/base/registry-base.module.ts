import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegFragmentsFormItem } from './fragments-form-item';
import { RegIdListFormItem } from './id-list-form-item';
import { RegProjectsFormItem } from './projects-form-item';
import { RegStructureFormItem } from './structure-form-item';
import { RegStructureQueryFormItem, RegStructureQueryOptions } from './structure-query-form-item';
import { RegFormView } from './form-view';
import { RegFormGroupView } from './form-group-view';
import { RegFormGroupItemBase } from './form-group-item-base';
import { RegFormGroupItemView } from './form-group-item-view';
import { RegSearchFormGroupView } from './search-form-group-view';
import { RegSearchFormGroupItemView } from './search-form-group-item-view';
import { RegSearchFormView } from './search-form-view';
import { RegSearchExport } from './search-export';
import { RegRecordDetailBase } from './record-detail-base';
import { RegBatchCreator } from './batch-creator';
import { RegBatchEditor } from './batch-editor';
import { RegBatchMover } from './batch-mover';
import { RegBatchSelector } from './batch-selector';
import { RegBulkRegisterPopup } from './bulk-register-popup';
import { RegUrlFormItem } from './url-form-item';
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
  DxValidatorModule,
  DxFileUploaderModule
} from 'devextreme-angular';
import { RegCommonModule } from '../../../common';
import { RegCommonComponentModule } from '../../common';
import { InventoryContainersFormGroup } from './inventory-containers-form-group-view/inventory-containers-form-group-view.component';
import { InventoryContainersFormGroupItemView } from './inventory-containers-form-group-item-view/inventory-containers-form-group-item-view.component';

export * from './fragments-form-item';
export * from './id-list-form-item';
export * from './projects-form-item';
export * from './structure-form-item';
export * from './form-view';
export * from './form-group-view';
export * from './registry-base.types';
export * from './record-detail-base';
export * from './batch-creator';
export * from './batch-editor';
export * from './batch-mover';
export * from './batch-selector';
export * from './search-export';
export * from './search-form-view';
export * from './structure-query-form-item';
export * from './url-form-item';

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
    RegCommonComponentModule,
    DxFileUploaderModule
  ],
  declarations: [
    RegIdListFormItem, RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem, RegStructureQueryOptions,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemBase, RegFormGroupItemView, RegSearchFormGroupItemView,
    RegRecordDetailBase, RegBatchCreator, RegBatchEditor, RegBatchMover, RegBatchSelector, RegBulkRegisterPopup, RegSearchExport,
    InventoryContainersFormGroup, InventoryContainersFormGroupItemView, RegUrlFormItem
  ],
  exports: [
    RegIdListFormItem, RegFragmentsFormItem, RegProjectsFormItem, RegStructureFormItem, RegStructureQueryFormItem, RegStructureQueryOptions,
    RegFormView, RegSearchFormView, RegFormGroupView, RegSearchFormGroupView, RegFormGroupItemBase, RegFormGroupItemView, RegSearchFormGroupItemView,
    RegRecordDetailBase, RegBatchCreator, RegBatchEditor, RegBatchMover, RegBatchSelector, RegBulkRegisterPopup,
    RegCommonModule, RegCommonComponentModule, RegSearchExport, InventoryContainersFormGroup, InventoryContainersFormGroupItemView, RegUrlFormItem
  ]
})
export class RegBaseComponentModule { }
