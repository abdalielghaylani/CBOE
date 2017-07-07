import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IFormItemTemplate } from '../item-templates.types';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../item-templates.css')],
})
export class RegTagBoxFormItem implements IFormItemTemplate {
  @Input() editMode: boolean = false;
  @Input() data: any = {};

  private get value(): any[] {
    return this.data.editorOptions.value ? this.data.editorOptions.value : [];
  }

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
