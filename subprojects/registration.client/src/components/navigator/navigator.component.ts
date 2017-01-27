import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-navigator',
  styles: [require('./navbar.css')],
  template: `
  <div
    [attr.data-testid]="testid"
    [ngClass]="cssClass">
    <ng-content></ng-content>
  </div>
  `
})
export class RegNavigator {
  @Input() testid: string;
  @Input() cssClass: string;
};
