import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-sidebar',
  template: `
  <div [id]="id" [ngClass]="cssClass">
  <ul>
    <ng-content></ng-content>
  </ul>
  </div>
  `
})
export class RegSidebar {
  @Input() id: string;
  @Input() cssClass: string;
};
