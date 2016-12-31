import { Component } from '@angular/core';

@Component({
  selector: 'reg-modal-content',
  styles: [require('./modal.css')],
  template: `
    <div class="p2 z2 bg-white modal.shown relative">
      <ng-content></ng-content>
    </div>
  `
})
export class RegModalContent {};
