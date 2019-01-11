import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/mergeMap';


import { apiUrlPrefix } from '../configuration';
import { HttpService } from '../services';
import { IPayloadAction, ConfigurationActions, SessionActions, IGridPullAction } from '../redux';

@Injectable()
export class ConfigurationEpics {
  constructor(private http: HttpService) { }

  handleOpenTable = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === ConfigurationActions.OPEN_TABLE)
      .mergeMap(({ payload }) => {
        let tableId: string = payload;
        return this.http.get(`${apiUrlPrefix}custom-tables/${tableId}?configOnly=true`)
          .map(result => {
            return ConfigurationActions.openTableSuccessAction({
              tableId,
              data: result.json()
            });
          })
          .catch(error => of(ConfigurationActions.openTableErrorAction({ tableId, error })));
      });
  }
}
