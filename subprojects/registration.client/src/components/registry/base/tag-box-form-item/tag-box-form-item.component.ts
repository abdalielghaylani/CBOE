import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTagBoxFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean = false;
  @Input() viewModel: any = {};
  @Input() viewConfig: any;
  protected value: any[];
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  ngOnChanges() {
    this.update();
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    if (options) {
      this.value = options.value ? options.value : [];
      this.dataSource = options.dataSource;
      this.displayExpr = options.displayExpr;
      this.valueExpr = options.valueExpr;
    }
  }

  protected onValueChanged(e, d) {
    d.component.option('formData.' + d.dataField, e.value);
  }
};
