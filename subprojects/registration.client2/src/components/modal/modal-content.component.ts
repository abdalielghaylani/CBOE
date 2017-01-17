import { Component } from '@angular/core';

@Component({
  selector: 'reg-modal-content',
  styles: [require('./modal.css')],
  template: `
    <div class="bg-white modal.shown relative" style="height:100%">
      <ng-content></ng-content>
    </div>
  `
})
export class RegModalContent {};
