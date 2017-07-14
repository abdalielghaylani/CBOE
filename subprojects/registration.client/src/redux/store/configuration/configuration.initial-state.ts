import {
  CCustomTableData,
  IConfigurationRecord,
  IConfiguration,
} from './configuration.types';
import { makeTypedFactory } from 'typed-immutable-record';

export const ConfigurationFactory = makeTypedFactory<IConfiguration, IConfigurationRecord>({
  customTables: {},
  formGroups: {}
});

export const INITIAL_CONFIG_STATE = ConfigurationFactory();
