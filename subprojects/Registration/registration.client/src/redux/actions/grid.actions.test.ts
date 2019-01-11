import { NgRedux, dispatch } from '@angular-redux/store';
import { GridActions } from './grid.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super();
  }
  dispatch = () => undefined;
  configureStore = () => undefined;
  configureSubStore = () => undefined;
  provideStore = () => undefined;
  replaceReducer = () => undefined;
  select = () => undefined;
  subscribe = () => undefined;
  getState = () => undefined;
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

  it('addRecord should dispatch ADD_RECORD action', () => {
    const expectedAction = GridActions.addRecordAction('test');

    spyOn(mockRedux, 'dispatch');
    actions.addRecord('test');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('editRecord should dispatch EDIT_RECORD action', () => {
    const expectedAction = GridActions.editRecordAction('test', '123');

    spyOn(mockRedux, 'dispatch');
    actions.editRecord('test', '123');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('deleteRecord should dispatch DELETE_RECORD action', () => {
    const expectedAction = GridActions.deleteRecordAction('test', '123');

    spyOn(mockRedux, 'dispatch');
    actions.deleteRecord('test', '123');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('getRecords should dispatch GET_RECORDS action', () => {
    const expectedAction = GridActions.getRecordsAction('test', 0, 20);

    spyOn(mockRedux, 'dispatch');
    actions.getRecords('test', 0, 20);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });

  it('getAllRecords should dispatch GET_ALL_RECORDS action', () => {
    const expectedAction = GridActions.getAllRecordsAction('test');

    spyOn(mockRedux, 'dispatch');
    actions.getAllRecords('test');

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
});
