import { NgRedux } from 'ng2-redux';
import { GridActions } from './grid.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('grid action creators', () => {
  let actions: GridActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new GridActions(mockRedux);
  });

  it('configure should dispatch CONFIGURE action', () => {
    const expectedAction = GridActions.configureAction('test');

    spyOn(mockRedux, 'dispatch');
    actions.configure('test');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('add-record should dispatch ADD_RECORD action', () => {
    const expectedAction = GridActions.addRecordAction('test');

    spyOn(mockRedux, 'dispatch');
    actions.addRecord('test');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('edit-record should dispatch EDIT_RECORD action', () => {
    const expectedAction = GridActions.editRecordAction('test', '123');

    spyOn(mockRedux, 'dispatch');
    actions.editRecord('test', '123');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('delete-record should dispatch DELETE_RECORD action', () => {
    const expectedAction = GridActions.deleteRecordAction('test', '123');

    spyOn(mockRedux, 'dispatch');
    actions.deleteRecord('test', '123');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
