import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { HomeDetailActions } from '../actions';
import { RegContainer, RegHomeDetail } from '../components';

@Component({
  selector: 'home-detail-page',
  providers: [ HomeDetailActions ],
  template: `
    <reg-container testid="home">
      <reg-home-detail
        (submit)="actions.submit()">
      </reg-home-detail>
    </reg-container>
  `
})
export class RegHomeDetailPage {
  constructor(private actions: HomeDetailActions) {}
}
