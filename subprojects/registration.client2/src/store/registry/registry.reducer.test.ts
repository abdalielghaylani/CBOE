import { Iterable } from 'immutable';
import { RegistryFactory } from './registry.initial-state';
import { registryReducer } from './registry.reducer';
import { RegistryActions, RecordDetailActions } from '../../actions';
import { IRegistryRecord } from './registry.types';

describe('registry reducer', () => {
  let initState: IRegistryRecord;

  beforeEach(() => {
    initState = RegistryFactory();
  });

  it('should have an immutable initial state', () => {
    expect(Iterable.isIterable(initState)).toBe(true);
  });

  it('should do nothing on OPEN_RECORDS_ERROR', () => {
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsErrorAction()
    );
    expect(nextState).toEqual(initState);
  });

  it('should clear rows on OPEN_RECORDS', () => {
    const previousValue = initState.temporary;
    const firstState = registryReducer(
      initState,
      RegistryActions.openRecordsAction(true)
    );
    expect(firstState.tempRecords.rows).toEqual([]);
    const secondState = registryReducer(
      firstState,
      RegistryActions.openRecordsAction(false)
    );
    expect(secondState.records.rows).toEqual([]);
  });

  it('should send data on OPEN_RECORDS_SUCCESS', () => {
    const data = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const firstState = registryReducer(
      initState,
      RegistryActions.openRecordsSuccessAction(true, data)
    );
    expect(firstState.tempRecords.rows).toEqual(data);
    const data2 = data.concat({ c1: 'v31', c2: 'v32' });
    const secondState = registryReducer(
      firstState,
      RegistryActions.openRecordsSuccessAction(false, data2)
    );
    expect(secondState.tempRecords.rows).toEqual(data);
    expect(secondState.records.rows).toEqual(data2);
  });

  it('should set record data on RETRIEVE_RECORD_SUCCESS', () => {
    const id = 100;
    const data = '<xml>encoded-temp-cdxml-data</xml>';
    const firstState = registryReducer(
      initState,
      RegistryActions.retrieveRecordSuccessAction(true, id, data)
    );
    expect(firstState.temporary).toEqual(true);
    expect(firstState.currentId).toEqual(id);
    expect(firstState.data).toEqual(data);
    const id2 = 101;
    const data2 = '<xml>encoded-cdxml-data</xml>';
    const secondState = registryReducer(
      firstState,
      RegistryActions.retrieveRecordSuccessAction(false, id2, data2)
    );
    expect(secondState.temporary).toEqual(false);
    expect(secondState.currentId).toEqual(id2);
    expect(secondState.data).toEqual(data2);
  });

  it('should set structureData on LOAD_STRUCTURE_SUCCESS', () => {
    const data = '<CDXML/>';
    const payload = { data: data };
    const nextState = registryReducer(
      initState,
      RecordDetailActions.loadStructureSuccessAction(payload)
    );
    expect(nextState.structureData).toEqual(data);
  });
});
