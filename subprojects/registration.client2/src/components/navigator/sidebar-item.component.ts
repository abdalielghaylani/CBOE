import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-sidebar-item',
  template: `
  <li>
    <ng-content></ng-content>
  </li>
  `
})
export class RegSidebarItem {

};
