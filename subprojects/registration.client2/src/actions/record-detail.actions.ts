import { Injectable } from '@angular/core';
import { NgRedux } from 'ng2-redux';
import { createAction } from 'redux-actions';
import { IRecordDetail, IAppState } from '../store';

@Injectable()
export class RecordDetailActions {
  static RETRIEVE_RECORD = 'RETRIEVE_RECORD';
  static RETRIEVE_RECORD_SUCCESS = 'RETRIEVE_RECORD_SUCCESS';
  static RETRIEVE_RECORD_ERROR = 'RETRIEVE_RECORD_ERROR';
  static SAVE = 'SAVE';
  static SAVE_SUCCESS = 'SAVE_SUCCESS';
  static SAVE_ERROR = 'SAVE_ERROR';
  static UPDATE = 'UPDATE';
  static UPDATE_SUCCESS = 'UPDATE_SUCCESS';
  static UPDATE_ERROR = 'UPDATE_ERROR';
  static REGISTER = 'REGISTER';
  static REGISTER_SUCCESS = 'REGISTER_SUCCESS';
  static REGISTER_ERROR = 'REGISTER_ERROR';
  static LOAD_STRUCTURE = 'LOAD_STRUCTURE';
  static LOAD_STRUCTURE_SUCCESS = 'LOAD_STRUCTURE_SUCCESS';
  static LOAD_STRUCTURE_ERROR = 'LOAD_STRUCTURE_ERROR';
  static retrieveRecordAction = createAction(RecordDetailActions.RETRIEVE_RECORD,
    (temporary: boolean, id: number) => ({ temporary, id }));
  static retrieveRecordSuccessAction = createAction(RecordDetailActions.RETRIEVE_RECORD_SUCCESS);
  static retrieveRecordErrorAction = createAction(RecordDetailActions.RETRIEVE_RECORD_ERROR);
  static saveAction = createAction(RecordDetailActions.SAVE);
  static saveSuccessAction = createAction(RecordDetailActions.SAVE_SUCCESS);
  static saveErrorAction = createAction(RecordDetailActions.SAVE_ERROR);
  static updateAction = createAction(RecordDetailActions.UPDATE);
  static updateSuccessAction = createAction(RecordDetailActions.UPDATE_SUCCESS);
  static updateErrorAction = createAction(RecordDetailActions.UPDATE_ERROR);
  static registerAction = createAction(RecordDetailActions.REGISTER);
  static registerSuccessAction = createAction(RecordDetailActions.REGISTER_SUCCESS);
  static registerErrorAction = createAction(RecordDetailActions.REGISTER_ERROR);
  static loadStructureAction = createAction(RecordDetailActions.LOAD_STRUCTURE);
  static loadStructureSuccessAction = createAction(RecordDetailActions.LOAD_STRUCTURE_SUCCESS);
  static loadStructureErrorAction = createAction(RecordDetailActions.LOAD_STRUCTURE_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  retrieveRecord(temporary: boolean, id: number) {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordAction(temporary, id));
  }

  retrieveRecordSuccess(data: IRecordDetail) {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordSuccessAction(data));
  }

  retrieveRecordError() {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordErrorAction());
  }

  save(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.saveAction(data));
  }

  update(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.updateAction(data));
  }

  register(data: Document) {
    this.ngRedux.dispatch(RecordDetailActions.registerAction(data));
  }

  loadStructure(data: string) {
    this.ngRedux.dispatch(RecordDetailActions.loadStructureAction(data));
  }

  loadStructureSuccess(data: string) {
    this.ngRedux.dispatch(RecordDetailActions.loadStructureSuccessAction(data));
  }

  loadStructureError(data: string) {
    this.ngRedux.dispatch(RecordDetailActions.loadStructureErrorAction(data));
  }
}
