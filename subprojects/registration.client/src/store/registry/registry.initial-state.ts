import {
  IRecordDetail, IRecordDetailRecord,
  IRecordsData, CRecordsData,
  IRecords, IRecordsRecord,
  IRegistry, IRegistryRecord,
} from './registry.types';
import { makeTypedFactory, TypedRecord } from 'typed-immutable-record';
import { apiUrlPrefix } from '../../configuration';

const INITIAL_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>({
  temporary: false,
  data: new CRecordsData(false),
  gridColumns: [{
    dataField: 'ID',
    dataType: 'number',
    visible: false,
  }, {
    dataField: 'NAME',
    dataType: 'string',
    caption: 'Name',
  }, {
    dataField: 'CREATED',
    dataType: 'date',
    caption: 'Created',
  }, {
    dataField: 'MODIFIED',
    dataType: 'date',
    caption: 'Modified',
  }, {
    dataField: 'CREATOR',
    caption: 'Created By',
    lookup: 'users',
  }, {
    dataField: 'STRUCTURE',
    dataType: 'string',
    allowFiltering: false,
    cellTemplate: function (c, o) {
      jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.STRUCTURE}" />`).appendTo(c);
    },
    caption: 'Structure',
    width: 160,
  }, {
    dataField: 'REGNUMBER',
    dataType: 'string',
    caption: 'Reg Number',
  }, {
    dataField: 'STATUS',
    dataType: 'number',
    caption: 'Status',
  }, {
    dataField: 'APPROVED',
    dataType: 'string',
    caption: 'Approved',
  }],
  filterRow: { visible: true }
})();

const INITIAL_TEMP_RECORDS = makeTypedFactory<IRecords, IRecordsRecord>({
  temporary: true,
  data: new CRecordsData(true),
  gridColumns: [{
    dataField: 'ID',
    dataType: 'number',
    visible: false,
  }, {
    dataField: 'BATCHID',
    dataType: 'number',
    caption: 'Batch ID',
  }, {
    dataField: 'MW',
    dataType: 'number',
    caption: 'MW',
  }, {
    dataField: 'MF',
    dataType: 'string',
    caption: 'MF',
  }, {
    dataField: 'CREATED',
    dataType: 'date',
    caption: 'Created',
  }, {
    dataField: 'MODIFIED',
    dataType: 'date',
    caption: 'Modified',
  }, {
    dataField: 'CREATOR',
    caption: 'Created By',
    lookup: 'users',
  }, {
    dataField: 'STRUCTURE',
    dataType: 'string',
    allowFiltering: false,
    cellTemplate: function (c, o) {
      jQuery(`<img src="${apiUrlPrefix}StructureImage/${o.data.STRUCTURE}" />`).appendTo(c);
    },
    caption: 'Structure',
    width: 160,
  }],
  filterRow: { visible: true }
})();

export const INITIAL_RECORD_DETAIL = makeTypedFactory<IRecordDetail, IRecordDetailRecord>({
  temporary: true,
  id: -1,
  data: null,
})();

export const RegistryFactory = makeTypedFactory<IRegistry, IRegistryRecord>({
  records: INITIAL_RECORDS,
  tempRecords: INITIAL_TEMP_RECORDS,
  currentRecord: INITIAL_RECORD_DETAIL,
  structureData: null,
});

export const INITIAL_STATE = RegistryFactory();
