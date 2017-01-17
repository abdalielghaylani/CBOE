import { Injectable } from '@angular/core';
import { Http, URLSearchParams, Headers, RequestOptions } from '@angular/http';
import { Action } from 'redux';
import { NgRedux } from 'ng2-redux';
import { IPayloadAction, RegActions, RegistryActions, RecordDetailActions } from '../actions';
import { IAppState } from '../store';
import { UPDATE_LOCATION } from 'ng2-redux-router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import * as registryUtils from '../components/registry/registry-utils';

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
            `${WS_URL}/RetrieveTemporaryRegistryRecord?id=${data.BATCHID}` :
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

  handleSaveRecord = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Save the record into a temporary storage
        // Create CreateTemporaryRegistryRecord
        let url: string = `${WS_URL}/CreateTemporaryRegistryRecord`;
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  handleSaveRecordSuccess = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  handleUpdateRecord = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.UPDATE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Update the database with the data retrieved and modified
        // Check if it is a temporary or permanent to find out the right end-point
        let temporary: boolean = registryUtils.getElementValue(payload.documentElement, 'RegNumeer/RegNumber') ? false : true;
        // Call UpdateRegistryRecord or UpdateTemporaryRegistryRecord
        let url: string = temporary ? `${WS_URL}/UpdateTemporaryRegistryRecord` : `${WS_URL}/UpdateRegistryRecord`;
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  handleUpdateRecordSuccess = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.UPDATE_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  handleRegisterRecord = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Call CreateRegistryRecord
        let url: string = `${WS_URL}/CreateRegistryRecord`;
        let data = new URLSearchParams();
        // registryUtils.fixStructureData(payload);
        data.append('xml', registryUtils.serializeData(payload));
        data.append('duplicateAction', 'N');
        return this.http.post(url, data)
          .map(result => RecordDetailActions.registerSuccessAction(result.text()))
          .catch(error => Observable.of(RecordDetailActions.registerErrorAction()));
      });
  }

  handleRegisterRecordSuccess = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // <ReturnList><ActionDuplicateTaken>N</ActionDuplicateTaken><RegID>30</RegID><RegNum>AB-000012</RegNum><BatchNumber>1</BatchNumber>
        // <BatchID>22</BatchID></ReturnList>
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  handleLoadStructure = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.LOAD_STRUCTURE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Call CreateRegistryRecord
        let url: string = `${BASE_URL}/DataConversion/ToCdxml`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        let data: string = payload;
        return this.http.post(url, { data }, options)
          .map(result => RecordDetailActions.loadStructureSuccessAction(result.json()))
          .catch(error => Observable.of(RecordDetailActions.loadStructureErrorAction()));
      });
  }
}
