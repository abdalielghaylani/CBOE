import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-alert',
  template: `
    <div
      [id]="qaid"
      class="alert"
      [attr.data-testid]="testid"
      [ngClass]="{
        'alert-info': status === 'info',
        'alert-warning': status === 'warning',
        'alert-success': status === 'success',
        'alert-danger': status === 'error',
        'white': status === 'info' || status === 'error'
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
