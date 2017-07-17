import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTagBoxFormItem implements IFormItemTemplate {
  @Input() editMode: boolean = false;
  @Input() data: any = {};
  protected value: any[];
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  ngOnChanges() {
    this.update();
  }

  protected update() {
    this.value = this.data.editorOptions.value ? this.data.editorOptions.value : [];
    this.dataSource = this.data.editorOptions.dataSource;
    this.displayExpr = this.data.editorOptions.displayExpr;
    this.valueExpr = this.data.editorOptions.valueExpr;
  }

  private onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
