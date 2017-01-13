import { ConfigurationEpics } from './configuration.epics';
import { RegistryEpics } from './registry.epics';
import { SessionEpics } from './session.epics';

export const EPIC_PROVIDERS = [ ConfigurationEpics, RegistryEpics, SessionEpics ];
export { ConfigurationEpics, RegistryEpics, SessionEpics };
