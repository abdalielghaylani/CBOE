import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { select } from '@angular-redux/store';
import { RecordDetailActions, IRecordDetail } from '../redux';
import { RegContainer, RegRecordPrint } from '../components';

@Component({
  selector: 'record-print-page',
  template: `
    <reg-container testid="records">
      <reg-record-print
        [temporary]="temporary">
      </reg-record-print>
    </reg-container>
  `
})
export class RegRecordPrintPage {
  private temporary: boolean;

  constructor(private router: Router) {
    this.temporary = (router.url.match(/.*\/temp.*/g) || []).length > 0;
  }
}
