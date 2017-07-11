import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-form-group',
  template: `
    <div
        [attr.data-testid]="testid"
        class="py2">
      <ng-content></ng-content>
    </div>
  `
})
export class RegFormGroup {
    @Input() testid: string;
};
