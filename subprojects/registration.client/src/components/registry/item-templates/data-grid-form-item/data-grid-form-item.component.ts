import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-data-grid-form-item-template',
  template: require('./data-grid-form-item.component.html'),
  styles: [require('../item-templates.css')],
})
export class RegDataGridFormItem {
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
