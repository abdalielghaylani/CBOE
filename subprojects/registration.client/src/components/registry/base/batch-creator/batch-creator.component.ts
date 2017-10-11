import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, OnChanges, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IBatch } from '../../../common';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../../../common';
import { CViewGroupContainer } from '../registry-base.types';

@Component({
  selector: 'reg-batch-creator',
  template: require('./batch-creator.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBatchCreator implements OnChanges {
  @Input() viewModel: IBatch[] = [];
  @Input() viewConfig: CViewGroupContainer;
  @Output() onCreated = new EventEmitter<any>();
  private formVisible: boolean = false;
  private items: any[];
  private formData: any;
  private colCount: number = 5;

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) {
    this.update();
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    this.items = this.viewConfig != null ? this.viewConfig.getItems('edit') : [];
    if (this.items.find(i => i.itemType === 'group') != null) {
      this.colCount = 1;
    }
    let validItems = [];
    this.items.forEach(i => {
      if (i.itemType === 'group') {
        validItems = validItems.concat(i.items.filter(ix => !ix.itemType || ix.itemType !== 'empty'));
      } else if (i.itemType !== 'empty') {
        validItems.push(i);
      }
    });
    this.formData = {};
    validItems.forEach(i => {
      this.formData[i.dataField] = undefined;
    });
  }

  protected onValueUpdated(e) {
  }

  protected showForm(e) {
    this.formVisible = true;
  }

  protected createBatch(e) {
    this.formVisible = false;
    this.onCreated.emit(this.formData);
  }

  protected cancel(e) {
    this.formVisible = false;    
  }
};
