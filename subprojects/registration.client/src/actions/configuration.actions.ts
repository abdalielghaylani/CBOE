import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class ConfigurationActions {
  static OPEN_TABLE = 'OPEN_TABLE';
  static OPEN_TABLE_SUCCESS = 'OPEN_TABLE_SUCCESS';
  static OPEN_TABLE_ERROR = 'OPEN_TABLE_ERROR';
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static LOAD_FORMGROUP = 'LOAD_FORMGROUP';
  static LOAD_FORMGROUP_SUCCESS = 'LOAD_FORMGROUP_SUCCESS';
  static openTableAction = createAction(ConfigurationActions.OPEN_TABLE);
  static openTableSuccessAction = createAction(ConfigurationActions.OPEN_TABLE_SUCCESS);
  static openTableErrorAction = createAction(ConfigurationActions.OPEN_TABLE_ERROR);
  static loadFormGroupAction = createAction(ConfigurationActions.LOAD_FORMGROUP);
  static loadFormGroupSuccessAction = createAction(ConfigurationActions.LOAD_FORMGROUP_SUCCESS);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  openTable(tableId: string) {
    this.ngRedux.dispatch(ConfigurationActions.openTableAction(tableId));
  }

  openTableSuccess(data: any) {
    this.ngRedux.dispatch(ConfigurationActions.openTableSuccessAction(data));
  }

  openTableError() {
    this.ngRedux.dispatch(ConfigurationActions.openTableErrorAction());
  }

  create() {
    this.ngRedux.dispatch({ type: ConfigurationActions.OPEN_CREATE });
  }

  edit() {
    this.ngRedux.dispatch({ type: ConfigurationActions.OPEN_EDIT });
  }
}
