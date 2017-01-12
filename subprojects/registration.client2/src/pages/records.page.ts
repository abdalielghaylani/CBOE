import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordsActions } from '../actions';
import { RegContainer, RegRecords } from '../components';

@Component({
  selector: 'records-page',
  providers: [ RecordsActions ],
  template: `
    <reg-container testid="home">
      <reg-records
        (create)="actions.create()"
        (edit)="actions.edit()"
        (search)="actions.search()">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  constructor(private actions: RecordsActions) {}
}
