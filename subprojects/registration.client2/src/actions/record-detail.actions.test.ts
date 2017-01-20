import { NgRedux } from 'ng2-redux';
import { RecordDetailActions } from './record-detail.actions';
import { } from 'jasmine';
import * as registryUtils from '../components/registry/registry.utils';

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

  it('update should dispatch UPDATE action', () => {
    const data = registryUtils.getDocument('<xmlData />');
    const expectedAction = RecordDetailActions.updateAction(data);

    spyOn(mockRedux, 'dispatch');
    actions.update(data);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
