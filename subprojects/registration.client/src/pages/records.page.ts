import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from '@angular-redux/store';
import { RegContainer, RegRecords } from '../components';
import { RegistryActions, IRegistry } from '../redux';

@Component({
  selector: 'records-page',
  providers: [RegistryActions],
  template: `
    <reg-container testid="records">
      <reg-records
        [temporary]="temporary"
        [restore]="restore">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  private temporary: boolean;
  private restore: boolean;

  constructor(private router: Router, private actions: RegistryActions) {
    this.temporary = (router.url.match(/.*\/temp.*/g) || []).length > 0;
    this.restore = (router.url.match(/.*\/restore.*/g) || []).length > 0;
  }
}
