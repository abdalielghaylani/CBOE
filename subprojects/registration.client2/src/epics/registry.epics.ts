import { Injectable } from '@angular/core';
import { Http, URLSearchParams, Headers, RequestOptions } from '@angular/http';
import { Action, MiddlewareAPI } from 'redux';
import { Epic, ActionsObservable, combineEpics } from 'redux-observable';
import { NgRedux } from 'ng2-redux';
import { IPayloadAction, RegActions, RegistryActions, RecordDetailActions, SessionActions } from '../actions';
import { IRecordDetail, IAppState } from '../store';
import { UPDATE_LOCATION } from 'ng2-redux-router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import * as registryUtils from '../components/registry/registry.utils';

const BASE_URL = '/Registration.Server/api';
const WS_URL = '/Registration.Server/Webservices/COERegistrationServices.asmx';

@Injectable()
export class RegistryEpics {
  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleRegistryActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleOpenRecords,
      this.handleRetrieveRecord,
      this.handleRetrieveRecordSuccess,
      this.handleSaveRecord,
      this.handleSaveRecordSuccess,
      this.handleUpdateRecord,
      this.handleUpdateRecordSuccess,
      this.handleRegisterRecord,
      this.handleRegisterRecordSuccess,
      this.handleLoadStructure,
    )(action$, store);
  }

  private handleOpenRecords: Epic = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === RegistryActions.OPEN_RECORDS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let temporary: boolean = payload;
        return this.http.get(`${BASE_URL}/RegistryRecords` + (payload ? '/Temp' : ''))
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RegistryActions.openRecordsSuccessAction(temporary, result.json());
          })
          .catch(error => Observable.of(RegistryActions.openRecordsErrorAction(error)));
      });
  }

  private handleRetrieveRecord: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number }>>) => {
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
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RegistryActions.retrieveRecordSuccessAction({
                temporary: payload.temporary,
                id: payload.id,
                data: result.text()
              } as IRecordDetail);
          })
          .catch(error => Observable.of(RegistryActions.retrieveRecordErrorAction(error)));
      });
  }

  private handleRetrieveRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number, data: string }>>) => {
    return action$.filter(({ type }) => type === RegistryActions.RETRIEVE_RECORD_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return payload.id < 0 ?
          Observable.of({ type: UPDATE_LOCATION, payload: 'records/new' }) :
          payload.temporary ?
            Observable.of({ type: UPDATE_LOCATION, payload: `records/temp/${payload.id}` }) :
            Observable.of({ type: UPDATE_LOCATION, payload: `records/${payload.id}` });
      });
  }

  private handleSaveRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Save the record into a temporary storage
        // Create CreateTemporaryRegistryRecord
        let url: string = `${WS_URL}/CreateTemporaryRegistryRecord`;
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleSaveRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleUpdateRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
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

  private handleUpdateRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.UPDATE_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleRegisterRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Call CreateRegistryRecord
        let url: string = `${WS_URL}/CreateRegistryRecord`;
        let data = new URLSearchParams();
        // registryUtils.fixStructureData(payload);
        data.append('xml', registryUtils.serializeData(payload));
        data.append('duplicateAction', 'N');
        return this.http.post(url, data)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RecordDetailActions.registerSuccessAction(result.text());
          })
          .catch(error => Observable.of(RecordDetailActions.registerErrorAction(error)));
      });
  }

  private handleRegisterRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER_SUCCESS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // <ReturnList><ActionDuplicateTaken>N</ActionDuplicateTaken><RegID>30</RegID><RegNum>AB-000012</RegNum><BatchNumber>1</BatchNumber>
        // <BatchID>22</BatchID></ReturnList>
        let response: Document = registryUtils.getDocument(registryUtils.getDocument(payload).documentElement.textContent);
        let regNumber = registryUtils.getElementValue(response.documentElement, 'RegNum');
        return regNumber ?
          Observable.of({ type: UPDATE_LOCATION, payload: `records` }) :
          Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleLoadStructure: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.LOAD_STRUCTURE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        // Call CreateRegistryRecord
        let url: string = `${BASE_URL}/DataConversion/ToCdxml`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        let data: string = payload;
        return this.http.post(url, { data }, options)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RecordDetailActions.loadStructureSuccessAction(result.json());
          })
          .catch(error => Observable.of(RecordDetailActions.loadStructureErrorAction(error)));
      });
  }
}
