import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IAppState } from '../store';

@Injectable()
export class ConfigurationActions {
  static CUSTOM_TABLES_SUCCESS = 'CUSTOM_TABLES_SUCCESS';
  static CUSTOM_TABLES_ERROR = 'CUSTOM_TABLES_ERROR';  
  static OPEN_TABLE = 'OPEN_TABLE';
  static OPEN_TABLE_SUCCESS = 'OPEN_TABLE_SUCCESS';
  static OPEN_TABLE_ERROR = 'OPEN_TABLE_ERROR';
  static OPEN_CREATE = 'OPEN_CREATE';
  static OPEN_EDIT = 'OPEN_EDIT';
  static customTablesSuccessAction = createAction(ConfigurationActions.CUSTOM_TABLES_SUCCESS);
  static customTablesErrorAction = createAction(ConfigurationActions.CUSTOM_TABLES_ERROR);
  static openTableAction = createAction(ConfigurationActions.OPEN_TABLE, tableId => ({ tableId }));
  static openTableSuccessAction = createAction(ConfigurationActions.OPEN_TABLE_SUCCESS);
  static openTableErrorAction = createAction(ConfigurationActions.OPEN_TABLE_ERROR);

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
