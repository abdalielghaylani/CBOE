import { TypedRecord } from 'typed-immutable-record';

export interface IConfiguration {
  customTables: any;
  formGroups: any;
};

export interface IConfigurationRecord extends TypedRecord<IConfigurationRecord>, IConfiguration {
};
