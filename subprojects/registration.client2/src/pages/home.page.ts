import { Component, Inject, ApplicationRef } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { select } from 'ng2-redux';
import { HomeActions } from '../actions';
import { RegContainer, RegHome } from '../components';

@Component({
  selector: 'home-page',
  providers: [ HomeActions ],
  template: `
    <reg-container testid="home">
      <reg-home
        (create)="actions.create()"
        (edit)="actions.edit()"
        (search)="actions.search()">
      </reg-home>
    </reg-container>
  `
})
export class RegHomePage {
  constructor(private actions: HomeActions) {}
}
