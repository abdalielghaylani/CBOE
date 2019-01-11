import { Input, Component } from '@angular/core';

@Component({
  selector: 'reg-navigator-items',
  template: `
    <div [attr.data-testid]="testid" class="navbar-collapse collapse">
      <ul class="nav navbar-nav navbar-right">
        <ng-content></ng-content>
      </ul>
    </div>
  `
})
export class RegNavigatorItems {
  @Input() testid: string;
}
