import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-text-form-item-template',
  template: require('./text-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTextFormItem extends RegBaseFormItem {

  protected update() {
    super.update();
    let options = this.viewModel.editorOptions;
    // set default value
    let isDefaultValueSet: boolean = false;
    if (!options.value) {
      if (this.editMode && options.defaultValue) {
        options.value = options.defaultValue;
        isDefaultValueSet = true;
      }
    }
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;

    // initialized default value, so update view model also
    if (isDefaultValueSet) {
      this.updateViewModel();
    }
  }
};
