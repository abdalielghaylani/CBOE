import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordsActions } from '../actions';
import { RegContainer, RegTempRecords } from '../components';

@Component({
  selector: 'temp-records-page',
  providers: [ RecordsActions ],
  template: `
    <reg-container testid="home">
      <reg-temp-records
        (create)="actions.create()"
        (edit)="actions.edit()"
        (search)="actions.search()">
      </reg-temp-records>
    </reg-container>
  `
})
export class RegTempRecordsPage {
  constructor(private actions: RecordsActions) {}
}
