import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-tag-box-form-item-template',
  template: require('./tag-box-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTagBoxFormItem extends RegBaseFormItem {
  protected value: any[];
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  protected update() {
    let options = this.viewModel.editorOptions;
    if (options) {
      this.value = options.value ? this.deserializeValue(options.value) : [];
      this.dataSource = options.dataSource;
      this.displayExpr = options.displayExpr;
      this.valueExpr = options.valueExpr;
    }
  }
};
