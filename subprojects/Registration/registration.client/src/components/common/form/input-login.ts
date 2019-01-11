import {
  Component,
  Input
} from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'reg-input-login',
  styles: [require('./input.css')],
  template: `
  <div class="input-group">
  <span class="input-group-addon" [id]="groupid" style="background-color: #58A618;">
  <ng-content></ng-content>
  </span>
  <input 
      [id]="qaid"
      [type]="inputType"
      [class]="classname"
      [attr.placeholder]="placeholder"
      [formControl]="control"
      >
  </div>`
})
export class RegInputLogin {
  @Input() inputType = 'text';
  @Input() placeholder = '';
  @Input() control: FormControl = new FormControl();
  @Input() qaid: string;
  @Input() classname: string;
  @Input() groupid: string;
  @Input() groupname: string;
}
