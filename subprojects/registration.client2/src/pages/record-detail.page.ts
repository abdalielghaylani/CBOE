import { Component, Inject, ApplicationRef, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { RecordDetailActions } from '../actions';
import { RegContainer, RegRecordDetail } from '../components';
import { IRegistry, IConfiguration } from '../store';

@Component({
  selector: 'record-detail-page',
  providers: [ RecordDetailActions ],
  template: `
    <reg-container testid="records">
      <reg-record-detail
        [temporary]="temporary"
        [id]="id"
        (submit)="actions.submit()">
      </reg-record-detail>
    </reg-container>
  `
})
export class RegRecordDetailPage {
  private sub: any;
  @Input() id: number = -1;
  @Input() temporary: boolean = false;
  @select(s => s.registry.records) records$: Observable<IRegistry>;
  @select() configuration$: Observable<IConfiguration>;

  constructor(private router: Router, private route: ActivatedRoute, private actions: RecordDetailActions) {
    this.temporary = (router.url.match(/.*\/temp.*/g) || []).length > 0;
  }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      let paramLabel = 'id';
      this.id = params[paramLabel];
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }  
}
