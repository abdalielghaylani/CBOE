import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../item-templates.css')],
})
export class RegTagBoxFormItem {
  @Input() private disabled: boolean = false;
  @Input() private name: string;
  @Input() private data: any;
  @Input() private lookup: any[] = [];

  private get value(): any[] {
    return this.data.editorOptions.value ? this.data.editorOptions.value : [];
  }

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
