import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'reg-header',
  template: require('./header.component.html')
})
export class RegHeader {
  @Input() testid: string;
  @Input() id: string;
};
