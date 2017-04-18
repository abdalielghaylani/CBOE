import { Injectable } from '@angular/core';
import { Http, URLSearchParams, Headers, RequestOptions } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { NgRedux } from '@angular-redux/store';
import { Action, MiddlewareAPI } from 'redux';
import { createAction } from 'redux-actions';
import { Epic, ActionsObservable, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { notify, notifySuccess } from '../common';
import { IPayloadAction, RegActions, RegistryActions, RecordDetailActions, SessionActions } from '../actions';
import { IRecordDetail, IAppState } from '../store';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import * as registryUtils from '../components/registry/registry.utils';
import { basePath } from '../configuration';

const BASE_URL = `${basePath}api`;
const WS_URL = `/COERegistration/Webservices/COERegistrationServices.asmx`;
const WS_ENVELOPE = `<soap12:Envelope
xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
xmlns:xsd="http://www.w3.org/2001/XMLSchema"
xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
<soap12:Header>
  <COECredentials xmlns="CambridgeSoft.COE.Registration.Services.Web"><AuthenticationTicket>{token}</AuthenticationTicket></COECredentials>
</soap12:Header>
<soap12:Body>
  <{method} xmlns="CambridgeSoft.COE.Registration.Services.Web"><xml>{xml}</xml>{additional}</{method}>
</soap12:Body>
</soap12:Envelope>`;

@Injectable()
export class RegistryEpics {
  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleRegistryActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleOpenRecords,
      this.handleRetrieveRecord,
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
      .mergeMap(({ payload }) => {
        let temporary: boolean = payload;
        return this.http.get(`${BASE_URL}/RegistryRecords` + (payload ? '/Temp' : ''))
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RegistryActions.openRecordsSuccessAction(result.json());
          })
          .catch(error => Observable.of(RegistryActions.openRecordsErrorAction(error)));
      });
  }

  private handleRetrieveRecord: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, id: number }>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.RETRIEVE_RECORD)
      .mergeMap(({ payload }) => {
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
              : RecordDetailActions.retrieveRecordSuccessAction({
                temporary: payload.temporary,
                id: payload.id,
                data: result.text()
              } as IRecordDetail);
          })
          .catch(error => Observable.of(RecordDetailActions.retrieveRecordErrorAction(error)));
      });
  }

  private handleSaveRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE_RECORD)
      .mergeMap(a => {
        // Convert CDXML to encoded-CDX first
        const structPath = 'ComponentList/Component/Compound/BaseFragment/Structure/Structure';
        let data = registryUtils.getElementValue(a.payload.documentElement, structPath);
        let url: string = `${BASE_URL}/DataConversion/FromCdxml`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(url, { data }, options)
          .map(result => {
            if (result.url.indexOf('index.html') > 0) {
              return SessionActions.logoutUserAction();
            } else {
              registryUtils.setElementValue(a.payload.documentElement, structPath, result.json().data);
              return RecordDetailActions.saveRecordSuccessAction(a.payload);
            }
          })
          .catch(error => Observable.of(RecordDetailActions.saveRecordErrorAction(error)));
      });
  }

  private handleSaveRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE_RECORD_SUCCESS)
      .mergeMap(a => {
        // Save the record into a temporary storage
        // Create CreateTemporaryRegistryRecord
        const params = {
          token: this.ngRedux.getState().session.token,
          method: 'CreateTemporaryRegistryRecord',
          xml: registryUtils.serializeData(a.payload).encodeHtml(),
          additional: ''
        };
        let data = WS_ENVELOPE.format(params);
        let headers = new Headers({ 'Content-Type': 'application/soap+xml', charset: 'utf-8' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(WS_URL, data, options)
          .map(result => {
            if (result.url.indexOf('index.html') > 0) {
              return SessionActions.logoutUserAction();
            } else {
              notifySuccess('The record was saved in the temporary registry successfully!', 5000);
              return createAction(UPDATE_LOCATION)(`records/temp`);
            }
          })
          .catch(error => Observable.of(RecordDetailActions.saveRecordErrorAction(error)));
      });
  }

  private handleUpdateRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.UPDATE_RECORD)
      .mergeMap(({ payload }) => {
        // Update the database with the data retrieved and modified
        // Check if it is a temporary or permanent to find out the right end-point
        let temporary: boolean = registryUtils.getElementValue(payload.documentElement, 'RegNumeer/RegNumber') ? false : true;
        // Call UpdateRegistryRecord or UpdateTemporaryRegistryRecord
        let url: string = temporary ? `${WS_URL}/UpdateTemporaryRegistryRecord` : `${WS_URL}/UpdateRegistryRecord`;
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleUpdateRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.UPDATE_RECORD_SUCCESS)
      .mergeMap(({ payload }) => {
        return Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleRegisterRecord: Epic = (action$: Observable<ReduxActions.Action<Document>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER_RECORD)
      .mergeMap(({ payload }) => {
        // This should come from the user/configuration
        const duplicateAction = 'N';
        const params = {
          token: this.ngRedux.getState().session.token,
          method: 'CreateRegistryRecord',
          xml: registryUtils.serializeData(payload).encodeHtml(),
          additional: `<duplicateAction>${duplicateAction}</duplicateAction>`
        };
        let data = WS_ENVELOPE.format(params);
        let headers = new Headers({ 'Content-Type': 'application/soap+xml', charset: 'utf-8' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(WS_URL, data, options)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : RecordDetailActions.registerRecordSuccessAction(result.text());
          })
          .catch(error => Observable.of(RecordDetailActions.registerRecordErrorAction(error)));
      });
  }

  private handleRegisterRecordSuccess: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.REGISTER_RECORD_SUCCESS)
      .mergeMap(({ payload }) => {
        // <ReturnList><ActionDuplicateTaken>N</ActionDuplicateTaken><RegID>30</RegID><RegNum>AB-000012</RegNum><BatchNumber>1</BatchNumber>
        // <BatchID>22</BatchID></ReturnList>
        let response: Document = registryUtils.getDocument(registryUtils.getDocument(payload).documentElement.textContent);
        let regNumber = registryUtils.getElementValue(response.documentElement, 'RegNum');
        notify(regNumber ? `${regNumber} was created successfully` : 'Registration failed!', regNumber ? 'success' : 'error', 5000);
        return regNumber ?
          Observable.of({ type: UPDATE_LOCATION, payload: `records` }) :
          Observable.of({ type: RegActions.IGNORE_ACTION });
      });
  }

  private handleLoadStructure: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.LOAD_STRUCTURE)
      .mergeMap(({ payload }) => {
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
