import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router } from '@angular/router';
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
        [temporary]="temporary"
        (submit)="actions.submit()">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  @Input() id: number = -1;
  @Input() temporary: boolean = false;

  constructor(private router: Router, private actions: RecordDetailActions) {
  }
}
