import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseColumnItem } from '../../common';

@Component({
  selector: 'reg-setting-value-form-item-template',
  template: require('./setting-value-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSettingValueFormItem extends RegBaseColumnItem {
  protected items: string[];

  protected update() {
    super.update();
    let options = this.viewModel.key;
    if (options.controlType === 'PICKLIST') {
      this.items = (options.allowedValues as string).split('|').map(s => s.trim());
    }
  }

  private onValueChanged(e) {
    this.viewModel.setValue(this.value);
  }
}
