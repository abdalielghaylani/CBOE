import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'reg-login-modal',
  template: `
    <reg-modal>
      <reg-modal-content>
        <reg-login-form
          [isPending]="isPending"
          [hasError]="hasError"
          (onSubmit)="handleSubmit($event)">
        </reg-login-form>
      </reg-modal-content>
    </reg-modal>
  `
})
export class RegLoginModal {
  @Input() isPending: boolean;
  @Input() hasError: boolean;
  @Output() onSubmit: EventEmitter<Object> = new EventEmitter();

  handleSubmit(login) {
    this.onSubmit.emit(login);
  }
};
