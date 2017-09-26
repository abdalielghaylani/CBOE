import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { IBatch } from '../../../common';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../../../common';

@Component({
  selector: 'reg-batch-selector',
  template: require('./batch-selector.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBatchSelector {
  @Input() viewModel: IBatch[] = [];
  @Output() onSelected = new EventEmitter<any>();
  private selectorVisible: boolean = false;
  private columns: any[] = [
    { dataField: 'BatchID', caption: 'ID', width: 60 },
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

  protected showSelector(e) {
    this.selectorVisible = true;
  }

  protected selectBatch(e) {
    let value = e.values[e.columns.findIndex(c => c.dataField === 'BatchID')];
    this.selectorVisible = false;
    this.onSelected.emit(value);
  }
};
