import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordDetailActions } from '../actions';
import { RegContainer, RegRecordDetail } from '../components';
import { IRecordDetail } from '../store';

@Component({
  selector: 'record-detail-page',
  providers: [RecordDetailActions],
  template: `
    <reg-container testid="records">
      <reg-record-detail>
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  constructor(private router: Router, private actions: RecordDetailActions) { }
}
