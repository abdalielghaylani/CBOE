import { NgRedux } from 'ng2-redux';
import { ConfigurationActions } from './configuration.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('counter action creators', () => {
  let actions: ConfigurationActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new ConfigurationActions(mockRedux);
  });

  it('create should dispatch OPEN_CREATE action', () => {
    const expectedAction = {
      type: ConfigurationActions.OPEN_CREATE
    };

    spyOn(mockRedux, 'dispatch');
    actions.create();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('edit should dispatch OPEN_EDIT action', () => {
    const expectedAction = {
      type: ConfigurationActions.OPEN_EDIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.edit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
