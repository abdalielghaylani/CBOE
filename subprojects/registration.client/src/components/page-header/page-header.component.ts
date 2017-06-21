import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'reg-page-header',
  template: require('./page-header.component.html')
})
export class RegPageHeader {
  @Input() testid: string;
  @Input() id: string;
};
