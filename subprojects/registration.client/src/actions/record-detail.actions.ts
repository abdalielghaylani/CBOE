import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IRecordDetail, IAppState } from '../store';

@Injectable()
export class RecordDetailActions {
  static CLEAR_RECORD = 'CLEAR_RECORD';
  static RETRIEVE_RECORD = 'RETRIEVE_RECORD';
  static RETRIEVE_RECORD_SUCCESS = 'RETRIEVE_RECORD_SUCCESS';
  static RETRIEVE_RECORD_ERROR = 'RETRIEVE_RECORD_ERROR';
  static SAVE_RECORD = 'SAVE_RECORD';
  static SAVE_RECORD_SUCCESS = 'SAVE_RECORD_SUCCESS';
  static SAVE_RECORD_ERROR = 'SAVE_RECORD_ERROR';
  static UPDATE_RECORD = 'UPDATE_RECORD';
  static UPDATE_RECORD_SUCCESS = 'UPDATE_RECORD_SUCCESS';
  static UPDATE_RECORD_ERROR = 'UPDATE_RECORD_ERROR';
  static LOAD_STRUCTURE = 'LOAD_STRUCTURE';
  static LOAD_STRUCTURE_SUCCESS = 'LOAD_STRUCTURE_SUCCESS';
  static LOAD_STRUCTURE_ERROR = 'LOAD_STRUCTURE_ERROR';
  static clearRecordAction = createAction(RecordDetailActions.CLEAR_RECORD);
  static retrieveRecordAction = createAction(RecordDetailActions.RETRIEVE_RECORD,
    (temporary: boolean, template: boolean, id: number) => ({ temporary, template, id }));
  static retrieveRecordSuccessAction = createAction(RecordDetailActions.RETRIEVE_RECORD_SUCCESS);
  static retrieveRecordErrorAction = createAction(RecordDetailActions.RETRIEVE_RECORD_ERROR);
  static saveRecordAction = createAction(RecordDetailActions.SAVE_RECORD,
    (temporary: boolean, id: number, recordDoc: Document, saveToPermanent: boolean) => ({ temporary, id, recordDoc, saveToPermanent }));
  static saveRecordSuccessAction = createAction(RecordDetailActions.SAVE_RECORD_SUCCESS);
  static saveRecordErrorAction = createAction(RecordDetailActions.SAVE_RECORD_ERROR);
  static loadStructureAction = createAction(RecordDetailActions.LOAD_STRUCTURE);
  static loadStructureSuccessAction = createAction(RecordDetailActions.LOAD_STRUCTURE_SUCCESS);
  static loadStructureErrorAction = createAction(RecordDetailActions.LOAD_STRUCTURE_ERROR);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  clearRecord() {
    this.ngRedux.dispatch(RecordDetailActions.clearRecordAction());
  }

  retrieveRecord(temporary: boolean, template: boolean, id: number) {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordAction(temporary, template, id));
  }

  retrieveRecordSuccess(data: IRecordDetail) {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordSuccessAction(data));
  }

  retrieveRecordError() {
    this.ngRedux.dispatch(RecordDetailActions.retrieveRecordErrorAction());
  }

  saveRecord(temporary: boolean, id: number, recordDoc: Document, saveToPermanent: boolean = false) {
    this.ngRedux.dispatch(RecordDetailActions.saveRecordAction(temporary, id, recordDoc, saveToPermanent));
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
