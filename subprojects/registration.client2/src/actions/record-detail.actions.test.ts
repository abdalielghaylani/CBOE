import { NgRedux } from 'ng2-redux';
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

  it('submit should dispatch SUBMIT action', () => {
    const expectedAction = {
      type: RecordDetailActions.SUBMIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.submit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
