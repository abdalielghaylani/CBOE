import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class RecordsActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static SEARCH = 'SEARCH';
  static OPEN_RECORDS = 'OPEN_RECORDS';
  static openRecordsAction = createAction(RecordsActions.OPEN_RECORDS, (temp: boolean) => (temp));

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

  openRecords(temp: boolean) {
    this.ngRedux.dispatch(RecordsActions.openRecordsAction(temp));
  }
}
