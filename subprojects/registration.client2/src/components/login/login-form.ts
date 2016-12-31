import { Component, EventEmitter, Input, Output } from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators
} from '@angular/forms';

@Component({
  selector: 'reg-login-form',
  template: `
    <reg-form
      [group]="group"
      (onSubmit)="handleSubmit()">
      <reg-alert 
        qaid="qa-pending"
        testid="alert-pending"
        status='info'
        *ngIf="isPending">Loading...</reg-alert>
      <reg-alert
        qaid="qa-alert"
        testid="alert-error"
        status='error'*ngIf="hasError">
        Invalid username and password
      </reg-alert>

      <reg-form-group
        testid="login-username">
        <reg-label qaid="qa-uname-label">Username</reg-label>
        <reg-input
          qaid="qa-uname-input"
          inputType='text'
          placeholder='Username'
          [control]="username"></reg-input>
        <reg-form-error
          qaid="qa-uname-validation"
          [visible]="showNameWarning()">
          Username is required.
        </reg-form-error>
      </reg-form-group>

      <reg-form-group
        testid="login-password">
        <reg-label qaid="qa-password-label">Password</reg-label>
        <reg-input
          qaid="qa-password-input"
          inputType='password'
          placeholder='Password'
          [control]="password"></reg-input>
        <reg-form-error
          qaid="qa-password-validation"
          [visible]="showPasswordWarning()">
          Password is required.
        </reg-form-error>
      </reg-form-group>

      <reg-form-group
        testid="login-submit">
        <reg-button
          qaid="qa-login-button"
          className="mr1"
          type="submit">
          Login
        </reg-button>
        <reg-button
          qaid="qa-clear-button"
          className="bg-red"
          type="reset"
          (onClick)="reset()">
          Clear
        </reg-button>
      </reg-form-group>
    </reg-form>
  `
})
export class RegLoginForm {
  @Input() isPending: boolean;
  @Input() hasError: boolean;
  @Output() onSubmit: EventEmitter<Object> = new EventEmitter();

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
