import { NgRedux } from '@angular-redux/store';
import { RecordDetailActions } from './record-detail.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('record detail action creators', () => {
  let actions: RecordDetailActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RecordDetailActions(mockRedux);
  });
});