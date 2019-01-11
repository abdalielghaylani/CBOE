import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { select } from '@angular-redux/store';
import { RegContainer, RegRecords } from '../components';
import { RegistryActions, IRegistry } from '../redux';

@Component({
  selector: 'bulk-records-page',
  providers: [RegistryActions],
  template: `<reg-container testid="records" >
    <reg-bulk-register-record></reg-bulk-register-record>
    </reg-container>
  `
})
export class RegBulkRecordsPage {
  private bulkreg: boolean;

  constructor(private router: Router, private actions: RegistryActions) {
    this.bulkreg = (router.url.match(/.*\/bulkreg.*/g) || []).length > 0;
  }
}
