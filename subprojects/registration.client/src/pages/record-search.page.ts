import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RegContainer, RegRecords } from '../components';

@Component({
  selector: 'records-search-page',
  template: `
    <reg-container testid="records">
      <reg-record-search>
      </reg-record-search>
    </reg-container>
  `
})
export class RegRecordSearchPage {

  constructor(private router: Router) {

  }
}
