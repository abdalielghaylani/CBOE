import { Injectable } from '@angular/core';
import { Route } from '@angular/router';
import { Http } from '@angular/http';
import { AsyncPipe } from '@angular/common';
import { select, NgRedux } from 'ng2-redux';
import { UPDATE_LOCATION } from 'ng2-redux-router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { IPayloadAction, RegActions, RegistryActions } from '../actions';
import { IRegistry, IAppState } from '../store';

const BASE_URL = '/Registration.Server/Webservices/COERegistrationServices.asmx';

@Injectable()
export class RouterEpics {
  @select(s => s.registry.records) records$: Observable<IRegistry>;
  @select(s => s.registry.tempRecords) tempRecords$: Observable<IRegistry>;

  constructor(private http: Http, private ngRedux: NgRedux<IAppState>) { }

  handleUpdateLocation = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === UPDATE_LOCATION)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let path: string = (payload as string).toLowerCase();
        let startIndex: number = path.indexOf('/records');
        if (startIndex >= 0) {
          let params: string[] = path.substring(startIndex).split('/');
          let temporary = params[2] === 'temp';
          if ((temporary && params.length === 4) || (!temporary && params.length === 3)) {
            let id = params[2] === 'new' ? -1 : +params[params.length - 1];
            if (id >= 0) {
              let records = temporary ?
                this.ngRedux.getState().registry.tempRecords :
                this.ngRedux.getState().registry.records;
              let data = records.rows.find(r => r[Object.keys(r)[0]] === id);
              let url = temporary ?
                `${BASE_URL}/RetrieveTemporaryRegistryRecord?id=${id}` :
                `${BASE_URL}/RetrieveRegistryRecord?regNum=${data.REGNUMBER}`;
              return this.http.get(url)
                .map(result => RegistryActions.retrieveRecordSuccessAction(temporary, result.text()))
                .catch(error => Observable.of(RegistryActions.retrieveRecordErrorAction()));
            }
          }
        }
        return Observable.of(RegActions.ignoreAction());
      });
  }
}
