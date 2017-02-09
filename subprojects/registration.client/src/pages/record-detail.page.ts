import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from '@angular-redux/store';
import { RecordDetailActions } from '../actions';
import { RegContainer, RegRecordDetail } from '../components';
import { IRecordDetail } from '../store';

@Component({
  selector: 'record-detail-page',
  providers: [RecordDetailActions],
  template: `
    <reg-container testid="records">
      <reg-record-detail
        [temporary]="temporary"
        [id]="id">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  private temporary: boolean;
  private id: number;
  
  constructor(private router: Router, private actions: RecordDetailActions) {
    let urlSegments = router.url.split('/');
    this.temporary = !!urlSegments.find(s => s === 'temp');
    let id = urlSegments[urlSegments.length - 1];
    this.id = id === 'new' ? -1 : +id;
  }
}
