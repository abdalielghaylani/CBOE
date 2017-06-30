import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-button',
  styles: [require('./button.css')],
  template: require('./button.component.html')
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
