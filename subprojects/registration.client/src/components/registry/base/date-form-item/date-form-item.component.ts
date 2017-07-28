import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IFormItemTemplate } from '../registry-base.types';
import * as moment from 'moment-es6';

@Component({
  selector: 'reg-date-form-item-template',
  template: require('./date-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDateFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any = {};
  @Input() viewConfig: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
  protected value: Date;
  protected readOnly: boolean;
  protected type: string = 'date';

  constructor(private ngRedux: NgRedux<IAppState>) {
  }

  ngOnChanges() {
    this.update();
  }

  deserializeValue(value: string): Date {
    // Consider the date coming from server as UTC time
    let dateValue = moment.default(`${value} +0000`, 'YYYY-MM-DD hh:mm:ss A Z');
    return dateValue.toDate();
  }

  serializeValue(value: Date): string  {
    if (this.type === 'date') {
      // Change to midday GMT
      value.setUTCHours(12);
    }
    // Format UTC date/time without time-zone information
    let formatted = moment.default(value).utcOffset(0).format('YYYY-MM-DD hh:mm:ss A');
    return formatted;
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
