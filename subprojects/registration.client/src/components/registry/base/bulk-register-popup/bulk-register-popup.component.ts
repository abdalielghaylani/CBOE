import {
  Component, EventEmitter, Input, Output,
  OnChanges, OnInit, OnDestroy,
  ChangeDetectorRef, ChangeDetectionStrategy, ViewChild, ViewEncapsulation
} from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import { IRegMarkedPopupModel } from '../../registry.types';

@Component({
  selector: 'reg-bulk-register-popup',
  template: require('./bulk-register-popup.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBulkRegisterPopup {
  @Input() viewModel: IRegMarkedPopupModel;
  @Output() registerMarked = new EventEmitter<any>();
  constructor(protected changeDetector: ChangeDetectorRef) {
  }

  register(event) {
    this.registerMarked.emit(this.viewModel);
  }

  cancelRegisterMarked() {
    this.viewModel.isVisible = false;
  }

}
