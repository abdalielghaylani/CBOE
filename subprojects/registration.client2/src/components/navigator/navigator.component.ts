import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-navigator',
  template: `
    <nav
      [attr.data-testid]="testid"
      class="flex items-center p1 bg-white border-bottom">
      <ng-content></ng-content>
    </nav>
  `
})
export class RegNavigator {
  @Input() testid: string;
};
