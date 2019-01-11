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
import { RegCommonComponentModule } from '../common';
import { RegLayoutComponentModule } from '../layout';
import { RegConfigAddins } from './config-addins.component';
import { RegConfigForms } from './config-forms.component';
import { RegConfigProperties } from './config-properties.component';
import { RegConfigSettings } from './config-settings.component';
import { RegConfigTables } from './config-tables.component';
import { RegConfigXmlForms } from './config-xml-forms.component';
import { RegSettingsPageHeader } from './settings-page-header.component';
import { RegStructureFragmentFormItem } from './structure-fragment-form-item.component';
import { RegSettingValueFormItem } from './setting-value-form-item';
import { RegValidationFormItem } from './validation-type-form-item';
import { RegAddinEventsListItem } from './addin-events.component';

export * from './config-addins.component';
export * from './config-forms.component';
export * from './config-properties.component';
export * from './config-settings.component';
export * from './config-tables.component';
export * from './config-xml-forms.component';
export * from './config.types';
export * from './settings-page-header.component';
export * from './setting-value-form-item';
export * from './validation-type-form-item';
export * from './addin-events.component';

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
    RegCommonComponentModule,
    RegLayoutComponentModule,
    DxFileUploaderModule
  ],
  declarations: [
    RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
    RegSettingsPageHeader, RegStructureFragmentFormItem,
    RegSettingValueFormItem, RegValidationFormItem, RegAddinEventsListItem
  ],
  exports: [
    RegConfigAddins, RegConfigForms, RegConfigProperties, RegConfigSettings, RegConfigTables, RegConfigXmlForms,
    RegSettingsPageHeader, RegStructureFragmentFormItem,
    RegSettingValueFormItem, RegValidationFormItem, RegAddinEventsListItem,
    RegCommonComponentModule, RegLayoutComponentModule
  ]
})
export class RegConfigurationComponentModule { }
