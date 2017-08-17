import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';
import * as moment from 'moment-es6';

@Component({
  selector: 'reg-date-form-item-template',
  template: require('./date-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDateFormItem extends RegBaseFormItem {
  protected value: Date;
  protected type: string = 'date';

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
};
