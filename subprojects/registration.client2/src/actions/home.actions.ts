import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { IAppState } from '../store';

@Injectable()
export class HomeActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';

  constructor(private ngRedux: NgRedux<IAppState>) { }

  create() {
    this.ngRedux.dispatch({ type: HomeActions.OPEN_CREATE });
  }

  edit() {
    this.ngRedux.dispatch({ type: HomeActions.OPEN_EDIT });
  }

  search() {
    this.ngRedux.dispatch({ type: HomeActions.SEARCH });
  }

}
