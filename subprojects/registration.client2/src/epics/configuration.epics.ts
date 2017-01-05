import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, ConfigurationActions, GridActions, IGridPullAction } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';

const BASE_URL = '/api';

@Injectable()
export class ConfigurationEpics {
  constructor(private http: Http) { }

  getRecords = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === ConfigurationActions.OPEN_TABLE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let tableId: string = payload.tableId;
        return this.http.get(`${BASE_URL}/` + tableId)
          .map(result => ConfigurationActions.openTableSuccessAction(result.json()._embedded[tableId]))
          .catch(error => Observable.of(ConfigurationActions.openTableErrorAction()));
      });
  }
}
