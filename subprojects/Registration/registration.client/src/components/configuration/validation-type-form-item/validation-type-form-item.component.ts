import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../redux';
import { RegBaseFormItem } from '../../common';

@Component({
  selector: 'reg-validation-type-form-item-template',
  template: require('./validation-type-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegValidationFormItem extends RegBaseFormItem {
  protected items: string[] = [];

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  protected update() {
    let propertyType = (this.viewConfig.type as string).toUpperCase();
    this.items = propertyType === 'DATE' || propertyType === 'PICKLISTDOMAIN' ? ['requiredField']
      : propertyType === 'NUMBER' ? ['requiredField', 'numericRange', 'positiveInteger', 'integer', 'double', 'textLength']
        : propertyType === 'TEXT' ? ['requiredField', 'textLength', 'wordListEnumeration'] : ['requiredField', 'textLength', 'wordListEnumeration', 'notEmptyStructure', 'notEmptyStructureAndNoText'];
    this.items = this.items.concat(['custom']);
    let options = this.viewModel.editorOptions;
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
  }
}
