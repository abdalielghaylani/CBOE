import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-page-header',
  template: `
    <div class="page-header condensed">
    <div class="row padding-sm">
    <div class="col-md-6">
      <h4 
      class="yellow text-relaxed pull-left" 
      [attr.data-testid]="testid" 
      [id]="id">
      <ng-content></ng-content>
      </h4>
    </div>
    </div>
    </div>
  `
})
export class RegPageHeader {
  @Input() testid: string;
  @Input() id: string;
};
