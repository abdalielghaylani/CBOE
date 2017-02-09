import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from '@angular-redux/store';
import { RegistryActions } from '../actions';
import { RegContainer, RegRecords } from '../components';
import { IRegistry } from '../store';

@Component({
  selector: 'records-page',
  providers: [RegistryActions],
  template: `
    <reg-container testid="records">
      <reg-records
        [temporary]="temporary">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  private temporary: boolean;

  constructor(private router: Router, private actions: RegistryActions) {
    this.temporary = (router.url.match(/.*\/temp.*/g) || []).length > 0;
  }
}
