import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { createAction } from 'redux-actions';
import { IRecordDetail, IRecordSaveData, IAppState } from '../store';

@Injectable()
export class RecordDetailActions {
  static CLEAR_RECORD = 'CLEAR_RECORD';
  static RETRIEVE_RECORD = 'RETRIEVE_RECORD';
  static RETRIEVE_RECORD_SUCCESS = 'RETRIEVE_RECORD_SUCCESS';
  static RETRIEVE_RECORD_ERROR = 'RETRIEVE_RECORD_ERROR';
  static SAVE_RECORD = 'SAVE_RECORD';
  static SAVE_RECORD_SUCCESS = 'SAVE_RECORD_SUCCESS';
  static SAVE_RECORD_ERROR = 'SAVE_RECORD_ERROR';
  static CREATE_DUPLICATE_RECORD = 'CREATE_DUPLICATE_RECORD';
  static CREATE_DUPLICATE_RECORD_SUCCESS = 'CREATE_DUPLICATE_RECORD_SUCCESS';
  static CREATE_DUPLICATE_RECORD_ERROR = 'CREATE_DUPLICATE_RECORD_ERROR';
  static CLEAR_DUPLICATE_RECORD = 'CLEAR_DUPLICATE_RECORD';
  static LOAD_DUPLICATE_RECORD_SUCCESS = 'LOAD_DUPLICATE_RECORD_SUCCESS';
  static UPDATE_RECORD = 'UPDATE_RECORD';
  static UPDATE_RECORD_SUCCESS = 'UPDATE_RECORD_SUCCESS';
  static UPDATE_RECORD_ERROR = 'UPDATE_RECORD_ERROR';
  static LOAD_STRUCTURE = 'LOAD_STRUCTURE';
  static LOAD_STRUCTURE_SUCCESS = 'LOAD_STRUCTURE_SUCCESS';
  static LOAD_STRUCTURE_ERROR = 'LOAD_STRUCTURE_ERROR';
  static CLEAR_SAVE_RESPONSE = 'CLEAR_SAVE_RESPONSE';
  static clearRecordAction = createAction(RecordDetailActions.CLEAR_RECORD);
  static clearDuplicateRecordAction = createAction(RecordDetailActions.CLEAR_DUPLICATE_RECORD);
  static retrieveRecordAction = createAction(RecordDetailActions.RETRIEVE_RECORD,
    (temporary: boolean, template: boolean, id: number) => ({ temporary, template, id }));
  static retrieveRecordSuccessAction = createAction(RecordDetailActions.RETRIEVE_RECORD_SUCCESS);
  static retrieveRecordErrorAction = createAction(RecordDetailActions.RETRIEVE_RECORD_ERROR);
  static saveRecordAction = createAction(RecordDetailActions.SAVE_RECORD, (saveData: IRecordSaveData) => (saveData));
  static saveRecordSuccessAction = createAction(RecordDetailActions.SAVE_RECORD_SUCCESS);
  static saveRecordErrorAction = createAction(RecordDetailActions.SAVE_RECORD_ERROR);
  static duplicateRecordAction = createAction(RecordDetailActions.CREATE_DUPLICATE_RECORD,
    (data: IRecordDetail, duplicateAction: string, regNo: string) => ({ data, duplicateAction, regNo }));
  static duplicateRecordSuccessAction = createAction(RecordDetailActions.CREATE_DUPLICATE_RECORD_SUCCESS,
    (payload: any) => (payload));
  static loadDuplicateRecordSuccessAction = createAction(RecordDetailActions.LOAD_DUPLICATE_RECORD_SUCCESS);
  static duplicateRecordErrorAction = createAction(RecordDetailActions.CREATE_DUPLICATE_RECORD_ERROR);
  static loadStructureAction = createAction(RecordDetailActions.LOAD_STRUCTURE);
  static loadStructureSuccessAction = createAction(RecordDetailActions.LOAD_STRUCTURE_SUCCESS);
  static loadStructureErrorAction = createAction(RecordDetailActions.LOAD_STRUCTURE_ERROR);
  static clearSaveResponseAction = createAction(RecordDetailActions.CLEAR_SAVE_RESPONSE);

  constructor(private ngRedux: NgRedux<IAppState>) { }

  clearRecord() {
    this.ngRedux.dispatch(RecordDetailActions.clearRecordAction());
  }

  clearDuplicateRecord() {
    this.ngRedux.dispatch(RecordDetailActions.clearDuplicateRecordAction());
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

  createDuplicate(data: IRecordDetail, duplicateAction: string, regNo: string) {
    this.ngRedux.dispatch(RecordDetailActions.duplicateRecordAction(data, duplicateAction, regNo ));
  }

  saveRecord(saveData: IRecordSaveData) {
    this.ngRedux.dispatch(RecordDetailActions.saveRecordAction(saveData));
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

  clearSaveResponse() {
    this.ngRedux.dispatch(RecordDetailActions.clearSaveResponseAction());
  }
}
