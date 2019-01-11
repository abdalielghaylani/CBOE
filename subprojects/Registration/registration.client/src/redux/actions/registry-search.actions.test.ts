import { NgRedux } from '@angular-redux/store';
import { RegistrySearchActions } from './registry-search.actions';
import { } from 'jasmine';
import { IQueryData } from '../store';

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

describe('registry action creators', () => {
  let actions: RegistrySearchActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RegistrySearchActions(mockRedux);
  });

  it('search should dispatch RETRIEVE_QUERY_FORM action', () => {
    const data = { temporary: true, id: 1 };
    const expectedAction = {
      type: RegistrySearchActions.RETRIEVE_QUERY_FORM,
      payload: data
    };

    spyOn(mockRedux, 'dispatch');
    actions.retrieveQueryForm(true, 1);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
  
});
