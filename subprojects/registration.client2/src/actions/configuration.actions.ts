import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { IAppState } from '../store';

@Injectable()
export class ConfigurationActions {
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';

  constructor(private ngRedux: NgRedux<IAppState>) { }

  create() {
    this.ngRedux.dispatch({ type: ConfigurationActions.OPEN_CREATE });
  }

  edit() {
    this.ngRedux.dispatch({ type: ConfigurationActions.OPEN_EDIT });
  }
}
