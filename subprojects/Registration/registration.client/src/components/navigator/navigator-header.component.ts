import { Input, Component } from '@angular/core';

@Component({
  selector: 'reg-navigator-header',
  template: `
    <div id="NavHeaderRegLogo" [attr.data-testid]="testid" class="navbar-header">
      <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </button>
      <div class="row">
        <ng-content></ng-content>
      </div>
    </div>
  `
})
export class RegNavigatorHeader {
  @Input() testid: string;
}
