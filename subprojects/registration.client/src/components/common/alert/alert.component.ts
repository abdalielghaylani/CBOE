import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-alert',
  styles: [`.alert {
            padding: 5px;
            margin-bottom: 5px;
            border: 1px solid transparent;
            border-radius: 4px;
        }`],
  template: `
    <div
      [id]="qaid"
      class="text-center alert"
      [attr.data-testid]="testid"
      [ngClass]="{
        'alert-info': status === 'info',
        'alert-warning': status === 'warning',
        'alert-success': status === 'success',
        'alert-danger': status === 'error',
        'red': status === 'info' || status === 'error'
      }">
      <ng-content></ng-content>
    </div>
  `
})
export class RegAlert {
  @Input() status = 'info';
  @Input() qaid: string;
  @Input() testid: string;
};
