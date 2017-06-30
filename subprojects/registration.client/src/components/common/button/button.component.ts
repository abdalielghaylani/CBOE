import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-button',
  styles: [require('./button.css')],
  template: `
    <button
      [attr.data-testid]="testid"
      [id]="qaid"
      (click)="handleClick($event)"
      type="{{type || 'button'}}"
      class="btn btn-block {{className}}">
      <ng-content></ng-content>
    </button>
  `
})
export class RegButton {
  @Input() className: string;
  @Input() type: string;
  @Input() qaid: string;
  @Input() testid: string;
  @Output() onClick = new EventEmitter<any>();

  handleClick(event) {
    this.onClick.emit(event);
  }
};
