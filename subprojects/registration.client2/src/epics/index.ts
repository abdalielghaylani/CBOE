import { ConfigurationEpics } from './configuration.epics';
import { RegistryEpics } from './registry.epics';
import { RouterEpics } from './router.epics';
import { SessionEpics } from './session.epics';

export const EPIC_PROVIDERS = [ ConfigurationEpics, RegistryEpics, RouterEpics, SessionEpics ];
export { ConfigurationEpics, RegistryEpics, RouterEpics, SessionEpics };
