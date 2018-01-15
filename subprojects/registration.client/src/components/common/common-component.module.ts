import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
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
import { RegCommonModule } from '../../common';
import { RegServicesModule } from '../../services';
import { ChemDrawWeb } from './chemdraw-web';
import { RegAlert } from './alert';
import { RegButton } from './button';
import { RegForm, RegFormGroup, RegFormError, RegInput, RegInputLogin, RegLabel, RegInputGroup } from './form';
import { RegModal, RegModalContent } from './modal';
import { RegBaseColumnItem } from './base-column-item';
import { RegBaseFormItem } from './base-form-item';
import { RegCheckBoxFormItem } from './check-box-form-item';
import { RegDataGridFormItem } from './data-grid-form-item';
import { RegDateFormItem } from './date-form-item';
import { RegDropDownColumnItem } from './drop-down-column-item';
import { RegDropDownFormItem } from './drop-down-form-item';
import { RegStructureBaseFormItem } from './structure-base-form-item';
import { RegStructureColumnItem } from './structure-column-item';
import { RegStructureImage } from './structure-image';
import { RegStructureImageFormItem } from './structure-image-form-item';
import { RegStructureImageColumnItem } from './structure-image-column-item';
import { RegTagBoxFormItem } from './tag-box-form-item';
import { RegTextFormItem } from './text-form-item';
import { RegFileUploaderFormItem } from './file-uploader-form-item';
import { CStructureImageService } from './structure-image.service';

export * from './common.types';
export * from './base-column-item';
export * from './base-form-item';
export * from './check-box-form-item';
export * from './data-grid-form-item';
export * from './date-form-item';
export * from './drop-down-column-item';
export * from './drop-down-form-item';
export * from './structure-base-form-item';
export * from './structure-column-item';
export * from './structure-image';
export * from './structure-image-form-item';
export * from './structure-image-column-item';
export * from './tag-box-form-item';
export * from './text-form-item';
export * from './file-uploader-form-item';
export * from './chemdraw-web';
export * from './alert';
export * from './button';
export * from './form';
export * from './modal';

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
    RegServicesModule,
    DxFileUploaderModule
  ],
  declarations: [
    ChemDrawWeb,
    RegAlert, RegButton,
    RegForm, RegFormGroup, RegFormError, RegInput, RegInputLogin, RegLabel, RegInputGroup,
    RegModal, RegModalContent,
    RegBaseColumnItem, RegDropDownColumnItem, RegStructureColumnItem,
    RegBaseFormItem, RegDataGridFormItem, RegDateFormItem, RegDropDownFormItem, RegTagBoxFormItem, RegTextFormItem,
    RegStructureBaseFormItem, RegStructureImageFormItem, RegStructureImageColumnItem, RegCheckBoxFormItem,
    RegStructureImage, RegFileUploaderFormItem
  ],
  exports: [
    ChemDrawWeb,
    RegAlert, RegButton,
    RegForm, RegFormGroup, RegFormError, RegInput, RegInputLogin, RegLabel, RegInputGroup,
    RegModal, RegModalContent,
    RegBaseColumnItem, RegDropDownColumnItem, RegStructureColumnItem,
    RegBaseFormItem, RegDataGridFormItem, RegDateFormItem, RegDropDownFormItem, RegTagBoxFormItem, RegTextFormItem,
    RegStructureBaseFormItem, RegStructureImageFormItem, RegStructureImageColumnItem, RegCheckBoxFormItem,
    RegStructureImage, RegFileUploaderFormItem,
    RegCommonModule, RegServicesModule
  ],
  providers: [
    CStructureImageService
  ]
})
export class RegCommonComponentModule { }
