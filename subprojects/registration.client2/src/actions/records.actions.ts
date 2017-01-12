import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { IAppState } from '../store';

@Injectable()
export class RecordsActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';

  constructor(private ngRedux: NgRedux<IAppState>) { }

  create() {
    this.ngRedux.dispatch({ type: RecordsActions.OPEN_CREATE });
  }

  edit() {
    this.ngRedux.dispatch({ type: RecordsActions.OPEN_EDIT });
  }

  search() {
    this.ngRedux.dispatch({ type: RecordsActions.SEARCH });
  }

}
