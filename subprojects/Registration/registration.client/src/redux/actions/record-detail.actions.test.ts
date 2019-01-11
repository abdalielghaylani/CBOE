import { NgRedux } from '@angular-redux/store';
import { RecordDetailActions } from './record-detail.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super();
  }
  dispatch = () => undefined;
  configureStore = () => undefined;
  configureSubStore = () => undefined;
  provideStore = () => undefined;
  replaceReducer = () => undefined;
  select = () => undefined;
  subscribe = () => undefined;
  getState = () => undefined;
}

describe('record detail action creators', () => {
  let actions: RecordDetailActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RecordDetailActions(mockRedux);
  });
});
