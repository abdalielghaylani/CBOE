import { Iterable } from 'immutable';
import { RegistryFactory } from './registry.initial-state';
import { registryReducer } from './registry.reducer';
import { RegistryActions, RecordDetailActions } from '../../actions';
import { IRegistryRecord, IRecordDetail } from './registry.types';

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

  it('should clear record rows on OPEN_RECORDS(false)', () => {
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsAction(false)
    );
    expect(nextState.records.rows).toEqual([]);
  });

  it('should clear temp record rows on OPEN_RECORDS(true)', () => {
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsAction(true)
    );
    expect(nextState.tempRecords.rows).toEqual([]);
  });

  it('should set record rows on OPEN_RECORDS_SUCCESS(false)', () => {
    const data = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsSuccessAction(false, data)
    );
    expect(nextState.records.rows).toEqual(data);
  });

  it('should set temp record rows on OPEN_RECORDS_SUCCESS(true)', () => {
    const data = [{ c1: 'v11', c2: 'v12' }, { c1: 'v21', c2: 'v22' }];
    const nextState = registryReducer(
      initState,
      RegistryActions.openRecordsSuccessAction(true, data)
    );
    expect(nextState.tempRecords.rows).toEqual(data);
  });

  it('should set record data on RETRIEVE_RECORD_SUCCESS', () => {
    const id = 100;
    const data = '<xml>encoded-temp-cdxml-data</xml>';
    const firstState = registryReducer(
      initState,
      RecordDetailActions.retrieveRecordSuccessAction({
        temporary: true,
        id: id,
        data: data
      } as IRecordDetail)
    );
    expect(firstState.currentRecord.temporary).toEqual(true);
    expect(firstState.currentRecord.id).toEqual(id);
    expect(firstState.currentRecord.data).toEqual(data);
    const id2 = 101;
    const data2 = '<xml>encoded-cdxml-data</xml>';
    const secondState = registryReducer(
      firstState,
      RecordDetailActions.retrieveRecordSuccessAction({
        temporary: false,
        id: id2,
        data: data2
      } as IRecordDetail)
    );
    expect(secondState.currentRecord.temporary).toEqual(false);
    expect(secondState.currentRecord.id).toEqual(id2);
    expect(secondState.currentRecord.data).toEqual(data2);
  });

  it('should clear the structure in the store on LOAD_STRUCTURE', () => {
    let prevState = initState.update('structureData', () => '<CDXML></CDXML>');
    const nextState = registryReducer(
      prevState,
      RecordDetailActions.loadStructureAction()
    );
    expect(nextState.structureData).toEqual(null);
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

  it('should ignore error cases', () => {
    const nextState = registryReducer(
      initState,
      RecordDetailActions.loadStructureErrorAction('error')
    );
    expect(nextState).toEqual(initState);
  });
});
