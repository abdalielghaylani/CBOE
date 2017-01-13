import { TypedRecord } from 'typed-immutable-record';

export interface IRegistry {
  temporary: boolean;
  rows: any[];
};

export interface IRegistryRecord extends TypedRecord<IRegistryRecord>, IRegistry {
};
