import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordsActions } from '../actions';
import { RegContainer, RegRecords } from '../components';

@Component({
  selector: 'records-page',
  providers: [RecordsActions],
  template: `
    <reg-container testid="records">
      <reg-records
        [temporary]="temporary"
        (create)="actions.create()"
        (edit)="actions.edit()"
        (search)="actions.search()">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  @Input() temporary: boolean = false;

  constructor(private router: Router, private actions: RecordsActions) {
    this.temporary = (router.url.match(/.*temp-records.*/g) || []).length > 0;
  }
}
