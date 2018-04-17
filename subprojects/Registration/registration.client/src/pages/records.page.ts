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
    <reg-container testid="records" *ngIf="!reload">
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
  private reload: boolean;
  private hitListId: number = 0;

  constructor(private router: Router, private actions: RegistryActions) {
    const url = router.url;
    this.temporary = (url.match(/.*\/temp.*/g) || []).length > 0;
    this.restore = (url.match(/.*\/restore.*/g) || []).length > 0;
    // For fixing issue :- View/Component doesn't refresh/reload if we try to reload/navigate to current route.
    // Added route(record/temp/reload) for force reloading (records/temp) route.
    this.reload = (url.match(/.*\/reload.*/g) || []).length > 0;
    if (this.reload) {
      this.router.navigate(['records/temp']);
    }
    if (this.restore) {
      let urlSegments = router.url.split('/');
      this.hitListId = Number(urlSegments[urlSegments.length - 1]);
    } else {
      const m = url.match(/\/hits\/(marked|\d+)/);
      if (m && m.length > 1 && m[1] !== 'marked') {
        this.hitListId = +m[1];
      }
    }
  }
}
