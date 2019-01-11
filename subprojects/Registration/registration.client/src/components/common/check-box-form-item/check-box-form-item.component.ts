import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-check-box-form-item-template',
  template: require('./check-box-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegCheckBoxFormItem extends RegBaseFormItem {

  deserializeValue(value: any): any {
    return value === 'T' ? true : false;
  }

  serializeValue(value: any): any {
    return value ? 'T' : 'F';
  }

  protected update() {
    super.update();
    let options = this.viewModel.editorOptions;
    this.readOnly = options.readOnly;
    // set default value
    let isDefaultValueSet: boolean = false;
    if (!options.value) {
      if (this.editMode && options.defaultValue) {
        options.value = options.defaultValue;
        isDefaultValueSet = true;
      }
    }
    this.value = options && options.value ? this.deserializeValue(options.value) : false;

    // initialized default value, so update view model also
    if (isDefaultValueSet) {
      this.updateViewModel();
    }
  }
}
