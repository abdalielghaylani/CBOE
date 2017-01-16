import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, RegistryActions, GridActions } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';

const BASE_URL = '/Registration.Server/api';

@Injectable()
export class RegistryEpics {
  constructor(private http: Http) { }

  handleOpenRecords = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === RegistryActions.OPEN_RECORDS)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let temporary: boolean = payload;
        return this.http.get(`${BASE_URL}/RegistryRecords` + (payload ? '/Temp' : ''))
          .map(result => RegistryActions.openRecordsSuccessAction(temporary, result.json()))
          .catch(error => Observable.of(RegistryActions.openRecordsErrorAction()));
      });
  }
}
