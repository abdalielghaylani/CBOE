import { NgRedux } from '@angular-redux/store';
import { RegistryActions } from './registry.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('registry action creators', () => {
  let actions: RegistryActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RegistryActions(mockRedux);
  });

  it('create should dispatch OPEN_CREATE action', () => {
    const expectedAction = {
      type: RegistryActions.OPEN_CREATE
    };

    spyOn(mockRedux, 'dispatch');
    actions.create();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('edit should dispatch OPEN_EDIT action', () => {
    const expectedAction = {
      type: RegistryActions.OPEN_EDIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.edit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('search should dispatch SEARCH action', () => {
    const expectedAction = {
      type: RegistryActions.SEARCH
    };

    spyOn(mockRedux, 'dispatch');
    actions.search();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
