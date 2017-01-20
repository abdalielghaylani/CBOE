import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-navigator',
  styles: [require('./navbar.css')],
  template: `
  <div
    [attr.data-testid]="testid"
    class="navbar navbar-default navbar-fixed-top navbar-top">
    <ng-content></ng-content>
  </div>
  `
})
export class RegNavigator {
  @Input() testid: string;
};
