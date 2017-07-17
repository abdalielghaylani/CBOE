import { Action } from 'redux';
import { ConfigurationActions } from './configuration.actions';
import { GridActions, IGridBaseAction, IGridPullAction, IGridRecordAction } from './grid.actions';
import { RegActions } from './reg.actions';
import { RegistryActions } from './registry.actions';
import { RecordDetailActions } from './record-detail.actions';
import { SessionActions } from './session.actions';
import { RegistrySearchActions } from './registry-search.actions';

export interface IPayloadAction extends Action {
  payload?: any;
}

export const ACTION_PROVIDERS = [GridActions, RegActions, RegistryActions, SessionActions, RegistrySearchActions];
export {
  ConfigurationActions,
  GridActions, IGridBaseAction, IGridPullAction, IGridRecordAction,
  RegActions, RegistryActions, RecordDetailActions,
  SessionActions, RegistrySearchActions
};
