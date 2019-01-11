import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { select } from '@angular-redux/store';
import { RegistrySearchActions } from '../redux';
import { RegContainer, RegRecords } from '../components';

@Component({
  selector: 'records-search-page',
  providers: [RegistrySearchActions],
  template: `
    <reg-container testid="records">
      <reg-record-search
        [temporary]="temporary"
        [id]="id">
      </reg-record-search>
    </reg-container>
  `
})
export class RegRecordSearchPage {
  private temporary: boolean;
  private id: number;

  constructor(private router: Router, private actions: RegistrySearchActions) {
    let urlSegments = router.url.split('/');
    this.temporary = !!urlSegments.find(s => s === 'temp');
    let id = urlSegments[urlSegments.length - 1];
    this.id = id === 'new' ? -1 : +id;
  }
}
