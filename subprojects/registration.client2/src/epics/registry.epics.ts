import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Action } from 'redux';
import { NgRedux } from 'ng2-redux';
import { IPayloadAction, RegistryActions, GridActions } from '../actions';
import { IAppState } from '../store';
import { UPDATE_LOCATION } from 'ng2-redux-router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';

const BASE_URL = '/Registration.Server/api';
const WS_URL = '/Registration.Server/Webservices/COERegistrationServices.asmx';

@Injectable()
export class RegistryEpics {
  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleOpenRecords = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === RegistryActions.OPEN_RECORDS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let temporary: boolean = payload;
        return this.http.get(`${BASE_URL}/RegistryRecords` + (payload ? '/Temp' : ''))
          .map(result => RegistryActions.openRecordsSuccessAction(temporary, result.json()))
          .catch(error => Observable.of(RegistryActions.openRecordsErrorAction()));
      });
  }

  handleRetrieveRecord = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number }>>) => {
    return action$.filter(({ type }) => type === RegistryActions.RETRIEVE_RECORD)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let records = payload.temporary ?
          this.ngRedux.getState().registry.tempRecords :
          this.ngRedux.getState().registry.records;
        let data = records.rows.find(r => r[Object.keys(r)[0]] === payload.id);
        let url = payload.id < 0 ?
          `${WS_URL}/RetrieveNewRegistryRecord` :
          payload.temporary ?
            `${WS_URL}/RetrieveTemporaryRegistryRecord?id=${payload.id}` :
            `${WS_URL}/RetrieveRegistryRecord?regNum=${data.REGNUMBER}`;
        return this.http.get(url)
          .map(result => RegistryActions.retrieveRecordSuccessAction(payload.temporary, payload.id, result.text()))
          .catch(error => Observable.of(RegistryActions.retrieveRecordErrorAction()));
      });
  }

  handleRetrieveRecordSuccess = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number, data: string }>>) => {
    return action$.filter(({ type }) => type === RegistryActions.RETRIEVE_RECORD_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return payload.id < 0 ?
          Observable.of({ type: UPDATE_LOCATION, payload: 'records/new' }) :
          payload.temporary ?
            Observable.of({ type: UPDATE_LOCATION, payload: `records/temp/${payload.id}` }) :
            Observable.of({ type: UPDATE_LOCATION, payload: `records/${payload.id}` });
      });
  }
}
