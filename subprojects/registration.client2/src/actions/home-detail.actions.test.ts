import { NgRedux } from 'ng2-redux';
import { HomeDetailActions } from './home-detail.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('counter action creators', () => {
  let actions: HomeDetailActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new HomeDetailActions(mockRedux);
  });

  it('submit should dispatch SUBMIT action', () => {
    const expectedAction = {
      type: HomeDetailActions.SUBMIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.submit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
