import {
  Component,
  Input
} from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'reg-input',
  styles: [require('./input.css')],
  template: `
    <input
      [id]="qaid"
      [type]="inputType"
      [class]="classname"
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
  @Input() classname: string;
};
