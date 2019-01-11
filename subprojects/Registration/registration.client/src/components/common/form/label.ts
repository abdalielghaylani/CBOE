import { Component, Input } from '@angular/core';

@Component({
  selector: 'reg-label',
  template: `
    <label [id]="qaid">
      <ng-content></ng-content>
    </label>
  `
})
export class RegLabel {
  @Input() qaid: string;
}
