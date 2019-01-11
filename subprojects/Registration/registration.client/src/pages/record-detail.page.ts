import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
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
        [bulkreg]="bulkreg"
        [template]="template"
        [id]="id"
        [useCurrent]="useCurrent">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  private temporary: boolean;
  private template: boolean;
  private bulkreg: boolean;
  private id: number;
  private useCurrent: boolean;

  constructor(private router: Router, private actions: RecordDetailActions) {
    let urlSegments = router.url.split('/');
    this.temporary = !!urlSegments.find(s => s === 'temp') || !!urlSegments.find(s => s === 'new');
    this.bulkreg = !!urlSegments.find(s => s === 'bulkreg');
    this.useCurrent = !!urlSegments.find(s => s === 'current');
    let id;
    if (this.useCurrent) {
      id = urlSegments[urlSegments.length - 2];
    } else {
      id = urlSegments[urlSegments.length - 1];
    }
    this.template = !!urlSegments.find(s => s === 'new') && id !== 'new';

    this.id = id.startsWith('new') ? -1 : +id;
  }
}
