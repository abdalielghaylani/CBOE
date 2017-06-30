import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-identifiers',
  template: require('./reg-identifiers.component.html'),
  styles: [require('./item-templates.css')],
})
export class RegIdentifers {
  @Input() testId: string;
};
