import { NgRedux } from 'ng2-redux';
import { HomeActions } from './home.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('counter action creators', () => {
  let actions: HomeActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new HomeActions(mockRedux);
  });

  it('create should dispatch OPEN_CREATE action', () => {
    const expectedAction = {
      type: HomeActions.OPEN_CREATE
    };

    spyOn(mockRedux, 'dispatch');
    actions.create();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('edit should dispatch OPEN_EDIT action', () => {
    const expectedAction = {
      type: HomeActions.OPEN_EDIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.edit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('search should dispatch SEARCH action', () => {
    const expectedAction = {
      type: HomeActions.SEARCH
    };

    spyOn(mockRedux, 'dispatch');
    actions.search();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
