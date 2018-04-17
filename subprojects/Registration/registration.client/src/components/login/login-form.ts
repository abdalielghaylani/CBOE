import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RegLogo } from '../layout';

import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';

@Component({
  selector: 'reg-login-form',
  styles: [require('./login.css')],
  template: require('./login-form.component.html')
})
export class RegLoginForm {
  @Input() isPending: boolean;
  @Input() hasError: boolean;
  @Output() onSubmit: EventEmitter<Object> = new EventEmitter();

  private RegistrationLogo = RegLogo.Image;
  private BannerImage = require('../../assets/pe_background_dd.png');

  // needed to be public to allow access from fixture tests
  username: FormControl;
  password: FormControl;
  group: FormGroup;

  constructor(private builder: FormBuilder) {
    this.reset();
  }

  showNameWarning() {
    return this.username.touched
      && !this.username.valid
      && this.username.hasError('required');
  }

  showPasswordWarning() {
    return this.password.touched
      && !this.password.valid
      && this.password.hasError('required');
  }

  handleSubmit() {
    this.password.markAsTouched();
    this.username.markAsTouched();

    if (this.password.value && this.username.value) {
      this.onSubmit.emit(this.group.value);
    }
  }

  reset() {
    this.username = new FormControl('', Validators.required);
    this.password = new FormControl('', Validators.required);
    this.hasError = false;
    this.isPending = false;
    this.group = this.builder.group({
      username: this.username,
      password: this.password
    });
  }
};
