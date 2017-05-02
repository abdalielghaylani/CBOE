import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-container',
  template: `
  <div [attr.data-testid]="testid"
    class="regcontainer  border-light background-white pb2">
    <ng-content></ng-content>
  <div>
  `
})
export class RegContainer {
  @Input() testid: string;
};
