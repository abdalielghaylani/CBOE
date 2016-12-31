import {
  Component,
  Input
} from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'reg-input',
  template: `
    <input
      [id]="qaid"
      [type]="inputType"
      class="block col-12 mb1 input"
      [attr.placeholder]="placeholder"
      [formControl]="control"
    />
  `
})
export class RegInput {
  @Input() inputType = 'text';
  @Input() placeholder = '';
  @Input() control: FormControl = new FormControl();
  @Input() qaid: string;
};
