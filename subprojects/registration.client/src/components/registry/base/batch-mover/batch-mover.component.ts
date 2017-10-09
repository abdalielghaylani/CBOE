import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IBatch } from '../../../common';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../../../common';

@Component({
  selector: 'reg-batch-mover',
  template: require('./batch-mover.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBatchMover {
  @Input() viewModel: IBatch[] = [];
  @Output() onMoved = new EventEmitter<any>();
  private formVisible: boolean = false;
  private columns: any[] = [
    { dataField: 'BatchID', caption: 'Batch ID', width: 80 },
    { dataField: 'BatchNumber', caption: 'Batch Number', width: 100 },
    { dataField: 'FullRegNumber', caption: 'Full Reg Number' },
    { dataField: 'DateCreated', caption: 'Date Created', dataType: 'date' },
    { dataField: 'DateLastModified', caption: 'Last Modification Date', dataType: 'date' },
    { dataField: 'PersonCreated', caption: 'Created By' }
  ];

  constructor(
    private ngRedux: NgRedux<IAppState>,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) {
    this.columns.filter(c => c.dataField.startsWith('Person')).forEach(c =>
      c.lookup = {
        dataSource: this.ngRedux.getState().session.lookups.users,
        displayExpr: 'USERID',
        valueExpr: 'PERSONID'
      }
    );
  }

  protected showForm(e) {
    this.formVisible = true;
    // Must show a dialog to pick a permanent record to move the current batch to.
    // Upon selecting a record and confirmation, batch should be moved to the selected record.
    }

  protected moveBatch(e) {
    // Move the bach
    // Close popup
    this.formVisible = false;
    // Inform container
    this.onMoved.emit(e);
  }
};
