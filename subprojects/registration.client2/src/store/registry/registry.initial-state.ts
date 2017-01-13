import {
  IRegistryRecord,
  IRegistry,
} from './registry.types';
import { makeTypedFactory } from 'typed-immutable-record';

export const RegistryFactory = makeTypedFactory<IRegistry, IRegistryRecord>({
  temporary: false,
  rows: [],
});

export const INITIAL_STATE = RegistryFactory();
