import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordDetailActions } from '../actions';
import { RegContainer, RegRecordDetail } from '../components';

@Component({
  selector: 'record-detail-page',
  providers: [ RecordDetailActions ],
  template: `
    <reg-container testid="records">
      <reg-record-detail
        (submit)="actions.submit()">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  constructor(private actions: RecordDetailActions) {}
}
