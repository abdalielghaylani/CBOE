import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, ConfigurationActions, SessionActions, IGridPullAction } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { apiUrlPrefix } from '../configuration';

@Injectable()
export class ConfigurationEpics {
  constructor(private http: Http) { }

  handleOpenTable = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === ConfigurationActions.OPEN_TABLE)
      .mergeMap(({ payload }) => {
        let tableId: string = payload;
        return this.http.get(`${apiUrlPrefix}custom-tables/` + tableId)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : ConfigurationActions.openTableSuccessAction({
                tableId,
                data: result.json()
              });
          })
          .catch(error => Observable.of(ConfigurationActions.openTableErrorAction({ tableId, error })));
      });
  }
}
