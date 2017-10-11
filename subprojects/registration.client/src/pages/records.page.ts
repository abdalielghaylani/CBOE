import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { select } from '@angular-redux/store';
import { RegContainer, RegRecords } from '../components';
import { RecordDetailActions, RegistryActions, IRegistry } from '../redux';

@Component({
  selector: 'records-page',
  providers: [RegistryActions, RecordDetailActions],
  template: `
    <reg-container testid="records">
      <reg-records
        [temporary]="temporary"
        [restore]="restore"
        [hitListId]="hitListId">
      </reg-records>
    </reg-container>
  `
})
export class RegRecordsPage {
  private temporary: boolean;
  private restore: boolean;
  private hitListId: number = 0;

  constructor(private router: Router, private actions: RegistryActions) {
    this.temporary = (router.url.match(/.*\/temp.*/g) || []).length > 0;
    this.restore = (router.url.match(/.*\/restore.*/g) || []).length > 0;

    if (this.restore) {
      let urlSegments = router.url.split('/');
      this.hitListId = Number(urlSegments[urlSegments.length - 1]);
    }
  }
}
