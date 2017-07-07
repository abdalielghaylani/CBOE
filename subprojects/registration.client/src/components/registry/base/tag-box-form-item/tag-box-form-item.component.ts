import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTagBoxFormItem implements IFormItemTemplate {
  @Input() editMode: boolean = false;
  @Input() data: any = {};
  private value: any[];

  ngOnChanges() {
    this.value = this.data.editorOptions.value ? this.data.editorOptions.value : [];
  }

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
