import {
  IConfigurationRecord,
  IConfiguration,
} from './configuration.types';
import { makeTypedFactory } from 'typed-immutable-record';

export const ConfigurationFactory = makeTypedFactory<IConfiguration, IConfigurationRecord>({
  tableId: '',
  rows: []
});

export const INITIAL_STATE = ConfigurationFactory();
