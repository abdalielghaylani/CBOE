import { TypedRecord } from 'typed-immutable-record';

export interface IConfiguration {
  tableId: string;
};

export interface IConfigurationRecord extends TypedRecord<IConfigurationRecord>, IConfiguration {
};
