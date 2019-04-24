import DxForm from 'devextreme/ui/form';
import * as dxDialog from 'devextreme/ui/dialog';
import { DxFormComponent } from 'devextreme-angular';
import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, OnChanges
} from '@angular/core';

import { notifyError, notifyException, notifySuccess } from '../../../../common';
import { IBatch } from '../../../common';

@Component({
  selector: 'reg-batch-mover',
  template: require('./batch-mover.component.html'),
  styles: [` /deep/ .dx-dialog > .dx-overlay-wrapper {
      z-index: 2000 !important;
  }`,
    require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBatchMover implements OnInit, OnChanges {
  @Input() viewModel: IBatch[] = [];
  @Output() onMoved = new EventEmitter<any>();
  @Input() batchId: Number;
  @Input() defaultShow: boolean = false;
  private formVisible: boolean = false;
  private moveBatchForm: DxForm;
  private moveBatchData: BatchData;

  private moveBatchItems = [{
    dataField: 'batchId',
    label: { text: 'Batch ID' },
    dataType: 'string',
    editorType: 'dxTextBox',
    readOnly: 'true',
    disabled: 'true'
  },
  {
    dataField: 'targetRegNum',
    label: { text: 'Target Registry Number' },
    dataType: 'string',
    editorType: 'dxTextBox',
    validationRules: [{ type: 'required', message: 'Registry Number is required' }]
  }];

  constructor(
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) {
  }

  ngOnInit() {
    this.moveBatchData = new BatchData(this.batchId);
  }

  ngOnChanges() {
    if (this.defaultShow) {
      this.showForm(null);
    } else {
      this.cancel(null);
    }

  }

  private onBatchMoveFormInit(e) {
    this.moveBatchForm = e.component as DxForm;
  }

  private showForm(e) {
    this.moveBatchData.targetRegNum = '';
    this.moveBatchData.batchId = this.batchId;
    this.formVisible = true;
  }

  private cancel(e) {
    if (this.moveBatchForm) {
      this.moveBatchForm.resetValues();
    }
    this.formVisible = false;
  }

  private moveBatch(e) {
    let res: any = this.moveBatchForm.validate();
    if (!res.isValid) {
      return false;
    }

    let dialogResult = dxDialog.confirm(
      `Are you sure you want to move the batch?`,
      'Confirm move batch');
    dialogResult.done(response => {
      if (response) {
        this.moveBatchData.batchId = this.batchId;
        this.onMoved.emit(this.moveBatchData);
      }
      if (this.moveBatchForm) {
        this.moveBatchForm.resetValues();
        this.moveBatchForm.updateData('batchId', this.batchId);
      }
    });
    return true;
  }
}

class BatchData {
  targetRegNum: string;
  batchId: Number;
  constructor(batchId: Number) {
    this.targetRegNum = '';
    this.batchId = batchId;
  }
}
