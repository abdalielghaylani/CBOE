import { NgRedux } from 'ng2-redux';
import { RecordsActions } from './records.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('counter action creators', () => {
  let actions: RecordsActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RecordsActions(mockRedux);
  });

  it('create should dispatch OPEN_CREATE action', () => {
    const expectedAction = {
      type: RecordsActions.OPEN_CREATE
    };

    spyOn(mockRedux, 'dispatch');
    actions.create();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('edit should dispatch OPEN_EDIT action', () => {
    const expectedAction = {
      type: RecordsActions.OPEN_EDIT
    };

    spyOn(mockRedux, 'dispatch');
    actions.edit();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('search should dispatch SEARCH action', () => {
    const expectedAction = {
      type: RecordsActions.SEARCH
    };

    spyOn(mockRedux, 'dispatch');
    actions.search();

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
