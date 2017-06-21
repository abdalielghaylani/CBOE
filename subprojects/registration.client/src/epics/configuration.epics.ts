import { Injectable } from '@angular/core';
import { IPayloadAction, ConfigurationActions, SessionActions, IGridPullAction } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { apiUrlPrefix } from '../configuration';
import { HttpService } from '../services';

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
          .catch(error => Observable.of(ConfigurationActions.openTableErrorAction({ tableId, error })));
      });
  }
}
