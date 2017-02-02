import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { IPayloadAction, ConfigurationActions, SessionActions, IGridPullAction } from '../actions';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/catch';
import { basePath } from '../configuration';

const BASE_URL = `${basePath}api`;

@Injectable()
export class ConfigurationEpics {
  constructor(private http: Http) { }

  handleOpenTable = (action$: Observable<IPayloadAction>) => {
    return action$.filter(({ type }) => type === ConfigurationActions.OPEN_TABLE)
      .mergeMap<IPayloadAction>(({ payload }) => {
        let tableId: string = payload.tableId;
        return this.http.get(`${BASE_URL}/CustomTables/` + tableId)
          .map(result => {
            return result.url.indexOf('index.html') > 0
              ? SessionActions.logoutUserAction()
              : ConfigurationActions.openTableSuccessAction(result.json());
          })
          .catch(error => { return Observable.of(ConfigurationActions.openTableErrorAction()); });
      });
  }
}
