import { Injectable } from '@angular/core';
import { URLSearchParams, Headers, RequestOptions } from '@angular/http';
import { UPDATE_LOCATION } from '@angular-redux/router';
import { NgRedux } from '@angular-redux/store';
import { Action, MiddlewareAPI } from 'redux';
import { createAction } from 'redux-actions';
import { Epic, ActionsObservable, combineEpics, StateObservable } from 'redux-observable';
import { Observable, of } from 'rxjs';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/map';
import * as registryUtils from '../components/registry/registry.utils';
import { apiUrlPrefix } from '../configuration';
import { notify, notifySuccess, notifyError, PrivilegeUtils } from '../common';
import { IPayloadAction, RegActions, RegistryActions, RecordDetailActions, SessionActions } from '../redux';
import { IRecordDetail, IRegistry, IRegistryRetrievalQuery, IRecordSaveData, IAppState, CSaveResponseData, IRecordData, IRegisterRecordList } from '../redux';
import { IResponseData } from '../components';
import { HttpService } from '../services';

@Injectable()
export class RegistryEpics {
  constructor(private http: HttpService, private ngRedux: NgRedux<IAppState>) { }

  handleRegistryActions: Epic = (action$: ActionsObservable<any>, state$: StateObservable<any>, dependencies: any) => {
    return combineEpics(
      this.handleRetrieveRecord,
      this.handleSaveRecord,
      this.createDuplicateRecord,
      this.handleLoadStructure,
      this.bulkRegisterRecord,
    )(action$, state$, dependencies);
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
              isLoggedInUserSuperVisor: result.json().isLoggedInUserSuperVisor,
              inventoryContainers: result.json().inventoryContainers
            } as IRecordDetail);
          })
          .catch(error => of(RecordDetailActions.retrieveRecordErrorAction(error)));
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
        if (!temporary) {
          sessionStorage.setItem('registerRecordData', JSON.stringify(payload.recordData));
        }
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return (createRecordAction
          ? this.http.post(`${apiUrlPrefix}${temporary ? 'temp-' : ''}records`, payload.recordData, options)
          : this.http.put(`${apiUrlPrefix}${temporary ? 'temp-' : ''}records/${id}`, payload.recordData, options))
          .map(result => {
            let responseData = result.json() as IResponseData;
            if ((responseData.data) && (responseData.data.TotalDuplicateCount || responseData.data.copyActions)) {
              return RecordDetailActions.loadDuplicateRecordSuccessAction(responseData.data);
            } else {
              let actionType = payload.saveToPermanent ? 'registered' : 'saved';
              let recordId = responseData.id;
              let newId = payload.saveToPermanent ? responseData.regNumber : responseData.id;
              let regNum = ` (${payload.saveToPermanent ? 'Reg Number' : 'ID'}: ${newId})`;
              let message = `The record was ${actionType} in the ${temporary ? 'temporary' : ''} registry`
                + `${createRecordAction ? regNum : ''} successfully!`;
              if (sessionStorage.previousRoutes === 'temp-register' && payload.saveToPermanent) {
                  sessionStorage.setItem('previousRoutes', 'temp-register-success');
              }
              notifySuccess(message, 5000);
              let searchTempPrivilege = true;
              if (temporary) {
                searchTempPrivilege = PrivilegeUtils.hasSearchTempPrivilege(this.ngRedux.getState().session.lookups.userPrivileges);
              }
              if (searchTempPrivilege && createRecordAction) {
                return createAction(UPDATE_LOCATION)(`records${temporary ? '/temp' : ''}/${recordId}`);
              } else if (searchTempPrivilege === false) {
                // use case: user does not have SEARCH_TEMP privilege
                // update the redux store( currentRecord)
                // navigate to record detail view and view will be generated using the record data from redux store
                this.ngRedux.dispatch(createAction(UPDATE_LOCATION)(`records/temp/${recordId}/current`));
                return RecordDetailActions.retrieveRecordSuccessAction({
                  temporary: true,
                  id: recordId,
                  data: responseData.data.data,
                  isLoggedInUserOwner: true,
                  isLoggedInUserSuperVisor: false
                } as IRecordDetail);
              }
              return RecordDetailActions.saveRecordSuccessAction(new CSaveResponseData(id, temporary));
            }
          })
          .catch(error => of(RecordDetailActions.saveRecordErrorAction(new CSaveResponseData(id, temporary, error))));
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
            let recordId = responseData.id;
            let newRegNo = responseData.regNumber;
            let message = `The record was registered in the registry`
              + `${newRegNo} successfully!`;
            notifySuccess(message, 5000);

            this.ngRedux.dispatch(RecordDetailActions.duplicateRecordSuccessAction(new CSaveResponseData(recordId, false, null, true)));
            return createAction(UPDATE_LOCATION)(`records/${recordId}`);
          })
          .catch(error => of(RecordDetailActions.duplicateRecordErrorAction(error)));
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
            if ((responseData.data) && (responseData.data.records)) {
              return RegistryActions.bulkRegisterRecordSuccessAction(responseData.data.records);
            }
            return null;
          })
          .catch(error => of(RegistryActions.bulkRegisterRecordErrorAction(error)));
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
          .catch(error => of(RecordDetailActions.loadStructureErrorAction(error)));
      });
  }
}
