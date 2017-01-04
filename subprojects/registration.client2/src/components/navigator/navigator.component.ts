import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-navigator',
  template: `
  <div
    [attr.data-testid]="testid"
    class="navbar navbar-default navbar-static-top">
    <ng-content></ng-content>
  </div>
  `
})
export class RegNavigator {
  @Input() testid: string;
};
