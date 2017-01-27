import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {RegModal} from './modal.component';
import {RegModalContent} from './modal-content.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    RegModal,
    RegModalContent
  ],
  exports: [
    RegModal,
    RegModalContent
  ]
})
export class RegModalModule { }
