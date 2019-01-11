import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgRedux, select } from '@angular-redux/store';
import { DxDataGridComponent } from 'devextreme-angular';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../common';
import { IAppState } from '../../redux';
import { HttpService } from '../../services';
import { ICopyActions } from './registry.types';

@Component({
  selector: 'reg-duplicate-popup',
  template: require('./duplicate-popup.component.html'),
  styles: [require('./records.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDuplicatePopup implements OnInit, OnDestroy {
  @Input() visible: boolean;
  @Input() viewModel: ICopyActions;
  @Output() popupClick: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private router: Router,
    private http: HttpService,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) { }

  ngOnInit() {
  }

  ngOnDestroy() {

  }

  protected onButtonClick(e) {
    this.popupClick.emit(e);
  }

}
