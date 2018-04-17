import { NgRedux } from '@angular-redux/store';
import { ConfigurationActions } from './configuration.actions';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('configuration action creators', () => {
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

  it('openTableSuccess should dispatch OPEN_TABLE_SUCCESS action', () => {
    const data = { test: 'test ' };
    const expectedAction = ConfigurationActions.openTableSuccessAction(data);

    spyOn(mockRedux, 'dispatch');
    actions.openTableSuccess(data);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('openTableError error should dispatch OPEN_TABLE_ERROR action', () => {
    const expectedAction = ConfigurationActions.openTableErrorAction();

    spyOn(mockRedux, 'dispatch');
    actions.openTableError();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
