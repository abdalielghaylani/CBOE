import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';
import { IConfigInfo } from '../../../common';

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
    const info: IConfigInfo = options.configInfo;
    if (!this.editMode && this.value && info != null && info.fieldConfig != null && info.fieldConfig.Mask) {
      const value = Number(this.value);
      if (!isNaN(value)) {
        let mask: any = info.fieldConfig.Mask;
        if (typeof mask !== 'string' && mask.__text) {
          mask = mask.__text;
        }
        if (mask) {
          mask = mask.trim();
          const decimalIndex = mask.indexOf('.');
          this.value = value.toFixed(decimalIndex > 0 ? mask.length - decimalIndex - 1 : 0);
        }
      }
    }

    // initialized default value, so update view model also
    if (isDefaultValueSet) {
      this.updateViewModel();
    }
  }
}
