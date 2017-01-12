import { TypedRecord } from 'typed-immutable-record';

export interface IConfiguration {
  tableId: string;
  rows: any[];
  customTables: any[];
};

export interface IConfigurationRecord extends TypedRecord<IConfigurationRecord>, IConfiguration {
};
