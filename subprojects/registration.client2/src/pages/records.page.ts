import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RegistryActions } from '../actions';
import { RegContainer, RegRecords } from '../components';
import { IRegistry } from '../store';

@Component({
  selector: 'records-page',
  providers: [RegistryActions],
  template: `
    <reg-container testid="records">
      <reg-records
        [registry]="boundRecords$ | async"
        (create)="actions.create()"
        (edit)="actions.edit()"
        (search)="actions.search()">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  @select(s => s.registry.records) private records$: Observable<IRegistry>;
  @select(s => s.registry.tempRecords) private tempRecords$: Observable<IRegistry>;
  private boundRecords$: Observable<IRegistry>;

  constructor(private router: Router, private actions: RegistryActions) {
    this.boundRecords$ = (router.url.match(/.*\/temp.*/g) || []).length > 0 ? this.tempRecords$ : this.records$;
  }
}
