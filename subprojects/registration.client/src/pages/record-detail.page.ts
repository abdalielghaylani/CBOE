import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from '@angular-redux/store';
import { RecordDetailActions, IRecordDetail } from '../redux';
import { RegContainer, RegRecordDetail } from '../components';

@Component({
  selector: 'record-detail-page',
  providers: [RecordDetailActions],
  template: `
    <reg-container testid="records">
      <reg-record-detail
        [temporary]="temporary"
        [template]="template"
        [id]="id">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  private temporary: boolean;
  private template: boolean;
  private id: number;
  
  constructor(private router: Router, private actions: RecordDetailActions) {
    let urlSegments = router.url.split('/');
    this.temporary = !!urlSegments.find(s => s === 'temp');
    let id = urlSegments[urlSegments.length - 1];
    this.template = !!urlSegments.find(s => s === 'new') && id !== 'new';
    this.id = id === 'new' ? -1 : +id;
  }
}
