import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-container',
  template: require('./container.component.html')
})
export class RegContainer {
  @Input() testid: string;
};
