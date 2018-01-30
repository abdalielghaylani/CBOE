import { NgRedux } from '@angular-redux/store';
import { RegistrySearchActions } from './registry-search.actions';
import { } from 'jasmine';
import { IQueryData } from '../store';

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
    const searchCriteria = `<?xml version="1.0" encoding="UTF-8"?><searchCriteria xmlns="COE.SearchCriteria"></searchCriteria>`;
    const data: IQueryData = { temporary: true, searchCriteria };
    const expectedAction = {
      // type: RegistrySearchActions.SEARCH_RECORDS,
      payload: data
    };

    spyOn(mockRedux, 'dispatch');
    // actions.searchRecords(data);

    expect(mockRedux.dispatch).toHaveBeenCalled();
    expect(mockRedux.dispatch).toHaveBeenCalledWith(expectedAction);
  });
  
});
