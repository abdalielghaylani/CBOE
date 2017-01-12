import { Action } from 'redux';
import { ConfigurationActions } from './configuration.actions';
import { CounterActions } from './counter.actions';
import { GridActions, IGridBaseAction, IGridPullAction, IGridRecordAction } from './grid.actions';
import { RecordsActions } from './records.actions';
import { RecordDetailActions } from './record-detail.actions';
import { SessionActions } from './session.actions';

export interface IPayloadAction extends Action {
  payload?: any;
}

export const ACTION_PROVIDERS = [CounterActions, GridActions, SessionActions];
export {
  ConfigurationActions,
  CounterActions,
  GridActions, IGridBaseAction, IGridPullAction, IGridRecordAction,
  RecordsActions, RecordDetailActions,
  SessionActions
};
