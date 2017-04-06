import { ConfigurationEpics } from './configuration.epics';
import { RegistryEpics } from './registry.epics';
import { RegistrySearchEpics } from './registry-search.epics';
import { SessionEpics } from './session.epics';

export const EPIC_PROVIDERS = [ConfigurationEpics, RegistryEpics, SessionEpics, RegistrySearchEpics];
export { ConfigurationEpics, RegistryEpics, SessionEpics, RegistrySearchEpics };
