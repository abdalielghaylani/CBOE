import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IFormItemTemplate } from '../registry-base.types';
import * as moment from 'moment-es6';

@Component({
  selector: 'reg-text-form-item-template',
  template: require('./text-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegTextFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any = {};
  @Input() viewConfig: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
  protected value: string;
  protected readOnly: boolean;

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  deserializeValue(value: string): string {
    return value;
  }

  serializeValue(value: string): string  {
    return value;
  }

  protected update() {
    let options = this.viewModel.editorOptions;
    this.value = options && options.value ? this.deserializeValue(options.value) : undefined;
    this.readOnly = !this.editMode || options.readOnly;
  }

  protected onValueChanged(e, d) {
    if (e.previousValue !== e.value) {
      let value = this.serializeValue(e.value);
      d.component.option('formData.' + d.dataField, value);
      this.onValueUpdated(this);
    }
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }
};
