import { NgRedux } from '@angular-redux/store';
import { RegistrySearchActions } from './registry-search.actions';
import { } from 'jasmine';

class MockRedux extends NgRedux<any> {
  constructor() {
    super(null);
  }
  dispatch: () => {};
}

describe('registry action creators', () => {
  let actions: RegistrySearchActions;
  let mockRedux: NgRedux<any>;

  beforeEach(() => {
    mockRedux = new MockRedux();
    actions = new RegistrySearchActions(mockRedux);
  });

  it('search should dispatch SEARCH_RECORDS action', () => {
    const expectedAction = {
      type: RegistrySearchActions.SEARCH_RECORDS
    };

    spyOn(mockRedux, 'dispatch');
    actions.searchRecords(true, false, 0);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
  
});
