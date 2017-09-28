import { Injectable } from '@angular/core';
import { URLSearchParams, Headers, RequestOptions } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { NgRedux } from '@angular-redux/store';
import { Action, MiddlewareAPI } from 'redux';
import { createAction } from 'redux-actions';
import { Epic, ActionsObservable, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import * as registryUtils from '../components/registry/registry.utils';
import { apiUrlPrefix } from '../configuration';
import { notify, notifySuccess } from '../common';
import { IPayloadAction, RegActions, RegistryActions, RecordDetailActions, SessionActions } from '../redux';
import { IRecordDetail, IRegistry, IRegistryRetrievalQuery, IRecordSaveData, IAppState, CSaveResponseData, IRecordData, IRegisterRecordList } from '../redux';
import { IResponseData } from '../components';
import { HttpService } from '../services';

@Injectable()
export class RegistryEpics {
  constructor(private http: HttpService) { }

  handleRegistryActions: Epic = (action$: ActionsObservable, store: MiddlewareAPI<any>) => {
    return combineEpics(
      this.handleOpenRecords,
      this.handleRetrieveRecord,
      this.handleSaveRecord,
      this.createDuplicateRecord,
      this.handleLoadStructure,
      this.bulkRegisterRecord,
    )(action$, store);
  }

  private handleOpenRecords: Epic = (action$: Observable<ReduxActions.Action<IRegistryRetrievalQuery>>) => {
    return action$.filter(({ type }) => type === RegistryActions.OPEN_RECORDS)
      .mergeMap(({ payload }) => {
        let url = `${apiUrlPrefix}${payload.temporary ? 'temp-' : ''}records`;
        let params = '';
        if (payload.skip) { params += `?skip=${payload.skip}`; }
        if (payload.take) { params += `${params ? '&' : '?'}count=${payload.take}`; }
        if (payload.sort) { params += `${params ? '&' : '?'}sort=${payload.sort}`; }
        url += params;
        return this.http.get(url)
          .map(result => {
            return RegistryActions.openRecordsSuccessAction(payload.temporary, result.json());
          })
          .catch(error => Observable.of(RegistryActions.openRecordsErrorAction(error)));
      });
  }

  private handleRetrieveRecord: Epic = (action$: Observable<ReduxActions.Action<{ temporary: boolean, template: boolean, id: number }>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.RETRIEVE_RECORD)
      .mergeMap(({ payload }) => {
        let url = apiUrlPrefix;
        if (!payload.template) {
          url += `${payload.temporary ? 'temp-' : ''}records/${payload.id}`;
        } else {
          url += `templates/${payload.id}`;
        }
        return this.http.get(url)
          .map(result => {
            return RecordDetailActions.retrieveRecordSuccessAction({
              temporary: payload.temporary,
              id: payload.id,
              data: result.json().data,
              isLoggedInUserOwner: result.json().isLoggedInUserOwner,
              isLoggedInUserSuperVisor: result.json().isLoggedInUserSuperVisor
            } as IRecordDetail);
          })
          .catch(error => Observable.of(RecordDetailActions.retrieveRecordErrorAction(error)));
      });
  }

  private handleSaveRecord: Epic = (action$: Observable<ReduxActions.Action<IRecordSaveData>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.SAVE_RECORD)
      .mergeMap(({ payload }) => {
        let id = payload.id;
        let createRecordAction = id < 0 || payload.saveToPermanent;
        let temporary = !payload.saveToPermanent && (id < 0 || payload.temporary);
        let record = registryUtils.serializeData(payload.recordDoc);
        payload.recordData.data = record;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return (createRecordAction
          ? this.http.post(`${apiUrlPrefix}${temporary ? 'temp-' : ''}records`, payload.recordData, options)
          : this.http.put(`${apiUrlPrefix}${temporary ? 'temp-' : ''}records/${id}`, payload.recordData, options))
          .map(result => {
            let responseData = result.json() as IResponseData;
            if ((responseData.data) && (responseData.data.DuplicateRecords || responseData.data.copyActions)) {
              return RecordDetailActions.loadDuplicateRecordSuccessAction(responseData.data);
            } else {
              let actionType = payload.saveToPermanent ? 'registered' : 'saved';
              let newId = payload.saveToPermanent ? responseData.regNumber : responseData.id;
              let regNum = ` (${payload.saveToPermanent ? 'Reg Number' : 'ID'}: ${newId})`;
              let message = `The record was ${actionType} in the ${temporary ? 'temporary' : ''} registry`
                + `${createRecordAction ? regNum : ''} successfully!`;
              notifySuccess(message, 5000);
              if ((payload.recordData.redirectToRecordsView === undefined || payload.recordData.redirectToRecordsView) && createRecordAction) {
                return createAction(UPDATE_LOCATION)(`records${temporary ? '/temp' : ''}`);
              }
              return RecordDetailActions.saveRecordSuccessAction(new CSaveResponseData(id, temporary));
            }
          })
          .catch(error => Observable.of(RecordDetailActions.saveRecordErrorAction(new CSaveResponseData(id, temporary, error))));
      });
  }

  private createDuplicateRecord: Epic = (action$: Observable<ReduxActions.Action<{ data: IRecordSaveData, duplicateAction: string, regNo: string }>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.CREATE_DUPLICATE_RECORD)
      .mergeMap(({ payload }) => {
        let data = { data: registryUtils.serializeData(payload.data.recordDoc), duplicateAction: payload.duplicateAction, regNo: payload.regNo };
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return (
          this.http.post(`${apiUrlPrefix}/records/duplicate`, data, options))
          .map(result => {
            let responseData = result.json() as IResponseData;
            let newId = responseData.regNumber;
            let message = `The record was registered in the registry`
              + `${newId} successfully!`;
            notifySuccess(message, 5000);
            return createAction(UPDATE_LOCATION)(`records`);
          })
          .catch(error => Observable.of(RecordDetailActions.duplicateRecordErrorAction(error)));
      });
  }

  private bulkRegisterRecord: Epic = (action$: Observable<ReduxActions.Action<IRegisterRecordList>>) => {
    return action$.filter(({ type }) => type === RegistryActions.BULK_REGISTER_RECORD)
      .mergeMap(({ payload }) => {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return (
          this.http.post(`${apiUrlPrefix}/records/bulk-register`, payload, options))
          .map(result => {
            let responseData = result.json() as IResponseData;
            let message = `The records was registered in the registry successfully!`;
            notifySuccess(message, 5000);
            return createAction(UPDATE_LOCATION)(`records`);
          })
          .catch(error => Observable.of(RegistryActions.bulkRegisterRecordErrorAction(error)));
      });
  }

  private handleLoadStructure: Epic = (action$: Observable<ReduxActions.Action<string>>) => {
    return action$.filter(({ type }) => type === RecordDetailActions.LOAD_STRUCTURE)
      .mergeMap(({ payload }) => {
        // Call CreateRegistryRecord
        let url: string = `${apiUrlPrefix}DataConversion/ToCdxml`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        let data: string = payload;
        return this.http.post(url, { data }, options)
          .map(result => {
            return RecordDetailActions.loadStructureSuccessAction(result.json());
          })
          .catch(error => Observable.of(RecordDetailActions.loadStructureErrorAction(error)));
      });
  }
}
